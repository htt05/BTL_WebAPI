using back_end.Models.BusinessModels;
using back_end.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly back_endContext _context;

        public ProductsController(back_endContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult> GetProducts([FromQuery] string? searchName, [FromQuery] int? categoryId)
        {
            var query = from product in _context.Products
                        join category in _context.Categories on product.CategoryId equals category.CategoryId
                        select new
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            SalePrice = product.SalePrice,
                            Discount = product.Discount,
                            ProductCount = product.ProductCount,
                            Picture = product.Picture,
                            Description = product.Description,
                            Status = product.Status,
                            CategoryId = product.CategoryId,
                            Category = category,
                        };
            if (searchName != null)
                query = query.Where(p => p.ProductName.ToLower().Contains(searchName.ToLower()));
            if (categoryId != null)
                query = query.Where(p => p.CategoryId == categoryId);
            var products = await query.ToListAsync();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            var product = await _context.Products.Include(p => p.Category).Include(p => p.ProductImgs).FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutProduct(string id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }
            if (await _context.Products.AnyAsync(p => p.ProductName == product.ProductName && p.ProductId != id))
            {
                return Conflict(new { message = "Product already exists." });
            }
            else
            {
                product.SalePrice = product.Price - ((product.Price * product.Discount) / 100);
                _context.Entry(product).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(new { message = "Update successfully." });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                if (await _context.Products.AnyAsync(p => (p.ProductName == product.ProductName && p.ProductId != product.ProductId) || (p.ProductId == product.ProductId)))
                {
                    return Conflict(new { message = "Product already exists." });
                }
                else
                {
                    product.SalePrice = product.Price - ((product.Price * product.Discount) / 100);
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Create successfully" });
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.Include(p => p.ProductImgs).Include(p => p.OrderDetails).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            if (product.OrderDetails.Count != 0)
            {
                return Conflict(new { message = "This product is in order, cannot be deleted." });
            }
            foreach (var img in product.ProductImgs)
            {
                _context.ProductImgs.Remove(img);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete successfully" });
        }

        // GET: api/ProductImgs/5
        [HttpGet("{id}/ProductImgs")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ProductImg>>> GetProductImg(int id)
        {
            var productImg = await _context.ProductImgs.Where(p => p.ProductId.Equals(id)).ToListAsync();

            if (productImg.Count == 0)
            {
                return new List<ProductImg>();
            }
            else
            {
                return productImg;
            }
        }
        [HttpDelete("DeleteProductImg/{id}")]
        [Authorize(Roles = "admin")]

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
        [HttpDelete("{id}/DeleteAllProductImgs")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAllProductImgs(int id)
        {
            var productImgs = await _context.ProductImgs.Where(p => p.ProductId.Equals(id)).ToListAsync();
            if (productImgs == null)
            {
                return NotFound();
            }
            foreach (var img in productImgs)
            {
                _context.ProductImgs.Remove(img);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
