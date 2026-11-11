using System.Security.Claims;
using System.Text;
using CloudFullStackMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudFullStackMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminLotsController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminLotsController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("SpotSafeApi");
        }

        // GET: AdminLots
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/ParkingLot");
            var lots = JsonConvert.DeserializeObject<List<ParkingLot>>(
                await response.Content.ReadAsStringAsync()) ?? new();
            return View(lots);
        }

        // GET: AdminLots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminLots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParkingLot lot)
        {
            ModelState.Remove(nameof(ParkingLot.OwnerId));   // set below, not by the form

            if (!ModelState.IsValid)
            {
                return View(lot);   // server-side validation failed: show the form again
            }

            lot.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var content = new StringContent(JsonConvert.SerializeObject(lot), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/ParkingLot", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "The API could not save the lot.");
                return View(lot);
            }

            TempData["Success"] = $"'{lot.Name}' was created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminLots/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/ParkingLot/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var lot = JsonConvert.DeserializeObject<ParkingLot>(await response.Content.ReadAsStringAsync());
            return View(lot);
        }

        // POST: AdminLots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ParkingLot lot)
        {
            if (id != lot.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(lot);
            }

            lot.Bays = new();   // only update the lot itself, not its bays

            var content = new StringContent(JsonConvert.SerializeObject(lot), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/ParkingLot/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "The API could not update the lot.");
                return View(lot);
            }

            TempData["Success"] = $"'{lot.Name}' was updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminLots/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"api/ParkingLot/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var lot = JsonConvert.DeserializeObject<ParkingLot>(await response.Content.ReadAsStringAsync());
            return View(lot);
        }

        // POST: AdminLots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ParkingLot/{id}");

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "The lot was deleted." : "The lot could not be deleted.";

            return RedirectToAction(nameof(Index));
        }

        // GET: AdminLots/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/ParkingLot/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var lot = JsonConvert.DeserializeObject<ParkingLot>(await response.Content.ReadAsStringAsync());
            return View(lot);
        }
    }
}