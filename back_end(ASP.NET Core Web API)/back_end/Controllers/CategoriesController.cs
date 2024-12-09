using back_end.Models.BusinessModels;
using back_end.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly back_endContext _context;

        public CategoriesController(back_endContext context)
        {
            _context = context;
        }

        // GET: api/Categories?searchName=&.....
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories([FromQuery] string? searchName)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(c => c.CategoryName.ToLower().Contains(searchName.ToLower()));
            }
            var categories = await query.ToListAsync();
            return categories;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }
            if (await _context.Categories.AnyAsync(a => a.CategoryName == category.CategoryName && a.CategoryId != id))
            {
                return Conflict(new { message = "Category already exists." });
            }
            else
            {
                _context.Entry(category).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(new { message = "Update successfully" });
            }
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category category)
        {
            if (await _context.Categories.AnyAsync(a => a.CategoryName == category.CategoryName))
            {
                return Conflict(new { message = "Category already exists." });
            }
            else
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Create successfully" });
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            if (category.Products.Count() > 0)
            {
                return Conflict(new { message = "This category has products, cannot be deleted." });
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Delete successfully" });
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
