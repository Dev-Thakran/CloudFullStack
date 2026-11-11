using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;
    public VehicleController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }

    // GET: api/Vehicle
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicle()
    {
        return await _context.Vehicle.ToListAsync();
    }

    // GET: api/Vehicle/5
    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicle.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    // PUT: api/Vehicle/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IActionResult> PutVehicle(int? id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest();
        }

        _context.Entry(vehicle).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
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

    // POST: api/Vehicle
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<Vehicle>> PostVehicle(Vehicle vehicle)
    {
        _context.Vehicle.Add(vehicle);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, vehicle);
    }

    // DELETE: api/Vehicle/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IActionResult> DeleteVehicle(int? id)
    {
        var vehicle = await _context.Vehicle.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicle.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool VehicleExists(int? id)
    {
        return _context.Vehicle.Any(e => e.Id == id);
    }
}
