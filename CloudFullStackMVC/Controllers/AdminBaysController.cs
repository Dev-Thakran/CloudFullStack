using System.Text;
using CloudFullStackMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudFullStackMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBaysController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminBaysController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("SpotSafeApi");
        }

        // GET: AdminBays/Create?lotId=1
        public IActionResult Create(int lotId)
        {
            return View(new Bay { ParkingLotId = lotId, IsActive = true });
        }

        // POST: AdminBays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bay bay)
        {
            if (!ModelState.IsValid) return View(bay);

            var content = new StringContent(JsonConvert.SerializeObject(bay), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Bay", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "The API could not save the bay.");
                return View(bay);
            }

            TempData["Success"] = $"Bay {bay.BayCode} was added.";
            return RedirectToAction("Details", "AdminLots", new { id = bay.ParkingLotId });
        }

        // GET: AdminBays/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Bay/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var bay = JsonConvert.DeserializeObject<Bay>(await response.Content.ReadAsStringAsync());
            return View(bay);
        }

        // POST: AdminBays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bay bay)
        {
            if (id != bay.Id) return BadRequest();
            if (!ModelState.IsValid) return View(bay);

            var content = new StringContent(JsonConvert.SerializeObject(bay), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Bay/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "The API could not update the bay.");
                return View(bay);
            }

            TempData["Success"] = $"Bay {bay.BayCode} was updated.";
            return RedirectToAction("Details", "AdminLots", new { id = bay.ParkingLotId });
        }

        // GET: AdminBays/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"api/Bay/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var bay = JsonConvert.DeserializeObject<Bay>(await response.Content.ReadAsStringAsync());
            return View(bay);
        }

        // POST: AdminBays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int parkingLotId)
        {
            var response = await _httpClient.DeleteAsync($"api/Bay/{id}");

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode ? "The bay was deleted." : "The bay could not be deleted.";

            return RedirectToAction("Details", "AdminLots", new { id = parkingLotId });
        }
    }
}