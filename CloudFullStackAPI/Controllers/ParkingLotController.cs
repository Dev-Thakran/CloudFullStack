using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ParkingLotController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;
    public ParkingLotController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }


    // GET: api/ParkingLot/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingLot>> GetParkingLot(int id)
    {
        var parkingLot = await _context.ParkingLot
            .Include(l => l.Bays)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (parkingLot == null)
        {
            return NotFound();
        }

        return parkingLot;
    }

    // PUT: api/ParkingLot/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<IActionResult> PutParkingLot(int? id, ParkingLot parkinglot)
    {
        if (id != parkinglot.Id)
        {
            return BadRequest();
        }

        _context.Entry(parkinglot).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ParkingLotExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ParkingLot
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<ActionResult<ParkingLot>> PostParkingLot(ParkingLot parkinglot)
    {
        _context.ParkingLot.Add(parkinglot);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetParkingLot", new { id = parkinglot.Id }, parkinglot);
    }

    // DELETE: api/ParkingLot/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<IActionResult> DeleteParkingLot(int? id)
    {
        var parkinglot = await _context.ParkingLot.FindAsync(id);
        if (parkinglot == null)
        {
            return NotFound();
        }

        _context.ParkingLot.Remove(parkinglot);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ParkingLotExists(int? id)
    {
        return _context.ParkingLot.Any(e => e.Id == id);
    }

    // GET: api/ParkingLot?search=queen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParkingLot>>> GetParkingLot(string? search)
    {
        var lots = from l in _context.ParkingLot
                   select l;

        if (!string.IsNullOrEmpty(search))
        {
            lots = lots.Where(l => l.Name.Contains(search) || l.Address.Contains(search));
        }

        return await lots.ToListAsync();
    }
}
