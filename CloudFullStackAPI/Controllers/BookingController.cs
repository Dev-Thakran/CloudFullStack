using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class BookingController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;
    public BookingController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }

    // GET: api/Booking
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBooking()
    {
        return await _context.Booking
            .Include(b => b.Bay)
                .ThenInclude(bay => bay.ParkingLot)
            .ToListAsync();
    }

    // GET: api/Booking/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _context.Booking.FindAsync(id);

        if (booking == null)
        {
            return NotFound();
        }

        return booking;
    }

    // POST: api/Booking
    [HttpPost]
    public async Task<ActionResult<Booking>> PostBooking(Booking booking)
    {
        // 1. The bay must exist
        var bay = await _context.Bay.FindAsync(booking.BayId);
        if (bay == null)
        {
            return NotFound("Bay not found.");
        }

        // 2. End must be after start
        if (booking.EndUtc <= booking.StartUtc)
        {
            return BadRequest("End time must be after start time.");
        }

        // 3. No overlapping (non-cancelled) booking on the same bay
        bool overlap = await _context.Booking.AnyAsync(b =>
            b.BayId == booking.BayId &&
            b.Status != "Cancelled" &&
            b.StartUtc < booking.EndUtc &&
            booking.StartUtc < b.EndUtc);

        if (overlap)
        {
            return Conflict("This bay is already booked for that time.");
        }

        // 4. Server decides these, not the client
        booking.DriverId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        booking.Status = "Confirmed";
        booking.CreatedUtc = DateTime.UtcNow;
        booking.ConfirmationCode = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper();

        decimal hours = (decimal)(booking.EndUtc - booking.StartUtc).TotalHours;
        booking.TotalCost = Math.Round(hours * bay.HourlyRate, 2);

        _context.Booking.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
    }

    // DELETE: api/Booking/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<IActionResult> DeleteBooking(int? id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        _context.Booking.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Booking/5/cancel
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (booking.DriverId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        booking.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // GET: api/Booking/driver/abc-123
    [HttpGet("driver/{driverId}")]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsForDriver(string driverId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (driverId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();   // 403
        }
        return await _context.Booking
            .Include(b => b.Bay)
            .ThenInclude(bay => bay.ParkingLot)
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.StartUtc)
            .ToListAsync();
    }
}
