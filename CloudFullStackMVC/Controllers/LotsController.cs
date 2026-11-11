using CloudFullStackMVC.Models;
using CloudFullStackMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace CloudFullStackMVC.Controllers
{
    public class LotsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _weatherClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LotsController(IHttpClientFactory factory, IConfiguration configuration)
        {
            _httpClient = factory.CreateClient("SpotSafeApi");
            _weatherClient = factory.CreateClient("OpenMeteo");
            _httpClientFactory = factory;
            _configuration = configuration;
        }

        // GET: Lots?search=queen&sortOrder=name_desc
        public async Task<IActionResult> Index(string? search, string? sortOrder)
        {
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;

            // Filtering: done by the API (in the database)
            string url = string.IsNullOrEmpty(search) ? "api/ParkingLot" : $"api/ParkingLot?search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load parking lots.";
                return View(new List<ParkingLot>());
            }

            var lots = JsonConvert.DeserializeObject<List<ParkingLot>>(await response.Content.ReadAsStringAsync()) ?? new();

            // Sorting: done here with LINQ
            lots = sortOrder switch
            {
                "name_desc" => lots.OrderByDescending(l => l.Name).ToList(),
                "address" => lots.OrderBy(l => l.Address).ToList(),
                "address_desc" => lots.OrderByDescending(l => l.Address).ToList(),
                _ => lots.OrderBy(l => l.Name).ToList()   // default: name A–Z
            };

            return View(lots);
        }

        // GET: Lots/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // 1. Lot + bays from API (SQL Server)
            var response = await _httpClient.GetAsync($"api/ParkingLot/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var lot = JsonConvert.DeserializeObject<ParkingLot>(await response.Content.ReadAsStringAsync())!;

            var model = new LotDetailsViewModel { Lot = lot };

            // 2. Current weather at the lot's location from Open-Meteo
            try
            {
                string url = $"v1/forecast?latitude={lot.Latitude}&longitude={lot.Longitude}&current=temperature_2m,precipitation,weather_code,wind_speed_10m";

                var weatherResponse = await _weatherClient.GetAsync(url);
                if (weatherResponse.IsSuccessStatusCode)
                {
                    var weather = JsonConvert.DeserializeObject<WeatherResponse>(
                        await weatherResponse.Content.ReadAsStringAsync());
                    model.Weather = weather?.Current;

                    if (model.Weather == null)
                        ViewBag.WeatherError = "Got a response, but no 'current' data in it.";
                }
                else
                {
                    ViewBag.WeatherError = $"{(int)weatherResponse.StatusCode}: {await weatherResponse.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.WeatherError = ex.Message;
            }

            return View(model);
        }

        // POST: Lots/Book
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int bayId, int lotId, DateTime start, DateTime end)
        {
            // Google users have no API token until they set a password
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("ApiToken")))
            {
                TempData["NeedsPassword"] = true;
                return RedirectToAction("Details", new { id = lotId });
            }
            // The form gives NZ local time, but the API stores UTC
            var nz = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");

            var booking = new
            {
                bayId = bayId,
                driverId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                startUtc = TimeZoneInfo.ConvertTimeToUtc(start, nz),
                endUtc = TimeZoneInfo.ConvertTimeToUtc(end, nz)
            };

            string json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Booking", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? "Your session has expired. Please log out and log in again."
                    : await response.Content.ReadAsStringAsync();
                return RedirectToAction("Details", new { id = lotId });
            }

            TempData["Success"] = "Booking confirmed!";
            return RedirectToAction("Details", new { id = lotId });
        }

        // GET: Lots/Availability
        public async Task<IActionResult> Availability(int lotId, DateTime? start, DateTime? end)
        {
            var model = new AvailabilityViewModel { LotId = lotId, Start = start, End = end };

            // First visit: just show the form
            if (start == null || end == null)
            {
                return View(model);
            }

            model.Searched = true;

            // The driver enters NZ time, but the function expects UTC
            var nz = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            string startUtc = TimeZoneInfo.ConvertTimeToUtc(start.Value, nz).ToString("yyyy-MM-ddTHH:mm");
            string endUtc = TimeZoneInfo.ConvertTimeToUtc(end.Value, nz).ToString("yyyy-MM-ddTHH:mm");

            // Read the function URL from appsettings.json
            string url = $"{_configuration["AzureFunctions:GetAvailableBays"]}?lotId={lotId}&start={startUtc}&end={endUtc}";

            HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    // Shows the function's own message
                    ViewBag.Error = await response.Content.ReadAsStringAsync();
                    return View(model);
                }

                string json = await response.Content.ReadAsStringAsync();
                model.AvailableBays = JsonConvert.DeserializeObject<List<Bay>>(json) ?? new();
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "The availability service isn't reachable right now. Is the Functions project running?";
            }

            return View(model);
        }

        // GET: Lots/Quote?bayId=1&hours=2
        [HttpGet]
        public async Task<IActionResult> Quote(int bayId, decimal hours)
        {
            string url = $"{_configuration["AzureFunctions:GetBookingQuote"]}?bayId={bayId}&hours={hours.ToString(CultureInfo.InvariantCulture)}";

            HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync(url);
                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Pass the function's message back to the browser, e.g. "Bay 999 was not found."
                    return StatusCode((int)response.StatusCode, body);
                }

                // Pass the function's JSON straight through to the browser
                return Content(body, "application/json");
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "The price service isn't available right now.");
            }
        }
    }
}