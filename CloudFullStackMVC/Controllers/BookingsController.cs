using System.Security.Claims;
using CloudFullStackMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudFullStackMVC.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly HttpClient _client;

        public BookingsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("SpotSafeApi");
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("ApiToken")))
            {
                ViewBag.NeedsPassword = true;
                return View(new List<Booking>());
            }

            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _client.GetAsync($"api/Booking/driver/{driverId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load your bookings.";
                return View(new List<Booking>());
            }

            string json = await response.Content.ReadAsStringAsync();
            var bookings = JsonConvert.DeserializeObject<List<Booking>>(json) ?? new();

            // Convert UTC times to NZ time for display
            var nz = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            foreach (var b in bookings)
            {
                b.StartUtc = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(b.StartUtc, DateTimeKind.Utc), nz);
                b.EndUtc = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(b.EndUtc, DateTimeKind.Utc), nz);
            }

            return View(bookings);
        }

        // POST: Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _client.PutAsync($"api/Booking/{id}/cancel", null);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode ? "Booking cancelled." : "Could not cancel the booking.";

            return RedirectToAction(nameof(Index));
        }
    }
}