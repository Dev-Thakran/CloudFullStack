using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BayController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;
    public BayController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }

    // GET: api/Bay
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bay>>> GetBay()
    {
        return await _context.Bay.Include(b => b.ParkingLot).ToListAsync();
    }

    // GET: api/Bay/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Bay>> GetBay(int id)
    {
        var bay = await _context.Bay.FindAsync(id);

        if (bay == null)
        {
            return NotFound();
        }

        return bay;
    }

    // PUT: api/Bay/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<IActionResult> PutBay(int? id, Bay bay)
    {
        if (id != bay.Id)
        {
            return BadRequest();
        }

        _context.Entry(bay).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BayExists(id))
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

    // POST: api/Bay
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<ActionResult<Bay>> PostBay(Bay bay)
    {
        _context.Bay.Add(bay);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBay", new { id = bay.Id }, bay);
    }

    // DELETE: api/Bay/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "Admin")]
    public async Task<IActionResult> DeleteBay(int? id)
    {
        var bay = await _context.Bay.FindAsync(id);
        if (bay == null)
        {
            return NotFound();
        }

        _context.Bay.Remove(bay);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BayExists(int? id)
    {
        return _context.Bay.Any(e => e.Id == id);
    }
}
