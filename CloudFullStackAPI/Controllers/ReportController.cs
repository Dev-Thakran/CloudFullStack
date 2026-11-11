using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;

    public ReportController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }

    // GET: api/Report/lot-revenue
    [HttpGet("lot-revenue")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<LotReportRow>>> GetLotRevenue()
    {
        var report = await (
            from lot in _context.ParkingLot
            join bay in _context.Bay on lot.Id equals bay.ParkingLotId
            join booking in _context.Booking on bay.Id equals booking.BayId
            where booking.Status != "Cancelled"
            group booking by new { lot.Id, lot.Name } into g
            select new LotReportRow
            {
                LotId = g.Key.Id,
                LotName = g.Key.Name,
                TotalBookings = g.Count(),
                TotalRevenue = g.Sum(b => b.TotalCost)
            })
            .OrderByDescending(r => r.TotalRevenue)
            .ToListAsync();

        return report;
    }
}