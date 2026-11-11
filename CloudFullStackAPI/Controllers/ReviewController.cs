using CloudFullStackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly CloudFullStackAPIDBContext _context;
    public ReviewController(CloudFullStackAPIDBContext context)
    {
        _context = context;
    }

    // GET: api/Review
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReview()
    {
        return await _context.Review.ToListAsync();
    }

    // GET: api/Review/5
    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
        var review = await _context.Review.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        return review;
    }

    // PUT: api/Review/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IActionResult> PutReview(int? id, Review review)
    {
        if (id != review.Id)
        {
            return BadRequest();
        }

        _context.Entry(review).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReviewExists(id))
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

    // POST: api/Review
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
        _context.Review.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetReview", new { id = review.Id }, review);
    }

    // DELETE: api/Review/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IActionResult> DeleteReview(int? id)
    {
        var review = await _context.Review.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.Review.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReviewExists(int? id)
    {
        return _context.Review.Any(e => e.Id == id);
    }
}
