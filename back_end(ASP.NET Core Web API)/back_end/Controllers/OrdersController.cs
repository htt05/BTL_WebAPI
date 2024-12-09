using back_end.Models.BusinessModels;
using back_end.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly back_endContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(back_endContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] string? searchName, [FromQuery] int? accountId)
        {
            var query = _context.Orders.Include(o => o.Account).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(o => o.Account.AccountName.ToLower().Contains(searchName.ToLower()));
            }
            if (accountId != null)
            {
                query = query.Where(o => o.AccountId == accountId);
            }
            var orders = await query.ToListAsync();

            return orders;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders.Include(o => o.Account).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        //lấy các orderdetail có orderid == id
        [HttpGet("{id}/OrderDetails")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails(string id)
        {
            var orderDetails = await _context.OrderDetails.Include(o => o.Product).Where(o => o.OrderId == id).ToListAsync();
            if (orderDetails == null)
            {
                return NotFound();
            }

            return orderDetails;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(string id, Order order)
        {
            if (id != order.OrderId)
            {
                _logger.LogWarning("Order ID mismatch: {OrderId}", id);
                return BadRequest(new { message = "vao day" });
            }
            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!OrderExists(id))
                {
                    _logger.LogWarning("Order not found: {OrderId}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency exception while updating order: {OrderId}", id);
                    throw;
                }
            }

            return Ok(new { message = "Update successfully." });
        }

        [HttpGet("{id}/Cancel")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = 4;
            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!OrderExists(id))
                {
                    _logger.LogWarning("Order not found: {OrderId}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency exception while updating order: {OrderId}", id);
                    throw;
                }
            }

            return Ok(new { result = true, accountId = order.AccountId });

        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        /* // DELETE: api/Orders/5
         [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteOrder(int id)
         {
             var order = await _context.Orders.FindAsync(id);
             if (order == null)
             {
                 return NotFound();
             }

             _context.Orders.Remove(order);
             await _context.SaveChangesAsync();

             return NoContent();
         }*/

        private bool OrderExists(string id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
