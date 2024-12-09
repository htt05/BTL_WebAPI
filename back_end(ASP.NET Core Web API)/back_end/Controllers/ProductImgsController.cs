using back_end.Models.BusinessModels;
using back_end.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImgsController : ControllerBase
    {
        private readonly back_endContext _context;

        public ProductImgsController(back_endContext context)
        {
            _context = context;
        }

        // GET: api/ProductImgs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductImg>>> GetProductImgs()
        {
            return await _context.ProductImgs.ToListAsync();
        }

        // GET: api/ProductImgs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductImg>> GetProductImg(int id)
        {
            var productImg = await _context.ProductImgs.FindAsync(id);

            if (productImg == null)
            {
                return NotFound();
            }

            return productImg;
        }

        // PUT: api/ProductImgs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductImg(int id, ProductImg productImg)
        {
            if (id != productImg.ProductImgId)
            {
                return BadRequest();
            }

            _context.Entry(productImg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImgExists(id))
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

        // POST: api/ProductImgs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductImg>> PostProductImg(ProductImg productImg)
        {
            _context.ProductImgs.Add(productImg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductImg", new { id = productImg.ProductImgId }, productImg);
        }

        // DELETE: api/ProductImgs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductImg(int id)
        {
            var productImg = await _context.ProductImgs.FindAsync(id);
            if (productImg == null)
            {
                return NotFound();
            }

            _context.ProductImgs.Remove(productImg);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductImgExists(int id)
        {
            return _context.ProductImgs.Any(e => e.ProductImgId == id);
        }
    }
}
