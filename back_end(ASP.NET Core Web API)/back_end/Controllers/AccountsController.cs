using back_end.Models.BusinessModels;
using back_end.Models.DataModels;
using back_end.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly back_endContext _context;
        private readonly IConfiguration _configuration;

        public AccountsController(back_endContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var passMd5 = Cipher.GenerateMD5(loginModel.Password);
            var account = _context.Accounts.FirstOrDefault(a => a.Email == loginModel.Email && a.Password == passMd5);

            if (account == null)
            {
                return Unauthorized(new { message = "Login failed" });
            }

            var token = GenerateJwtToken(account);

            account.Token = token;

            return Ok(account);
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts(string? searchName)
        {
            IQueryable<Account> query = _context.Accounts.Include(a => a.Orders);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(c => c.AccountName.ToLower().Contains(searchName.ToLower()));
            }

            var accounts = await query.ToListAsync();

            return Ok(accounts);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var query = from acc in _context.Accounts
                        where acc.AccountId == id
                        select new
                        {
                            AccountId = acc.AccountId,
                            AccountName = acc.AccountName,
                            Email = acc.Email,
                            PhoneNumber = acc.PhoneNumber,
                            Password = acc.Password,
                            Orders = _context.Orders.Where(order => order.AccountId == acc.AccountId)
                            .Select(order => new
                            {
                                OrderId = order.OrderId,
                                Created_at = order.Created_at,
                                Status = order.Status,
                                ToltalPrice = order.ToltalPrice,
                                AccountId = order.AccountId
                            }).ToList()
                        };
            var account = await query.FirstOrDefaultAsync();
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }
        [HttpPost]
        public async Task<IActionResult> PostAccount(Account account)
        {
            if (await _context.Accounts.AnyAsync(a => a.Email == account.Email))
            {
                return Conflict(new { message = "Email already exists.", account = account });
            }
            else
            {
                account.Password = Cipher.GenerateMD5(account.Password);

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Create successfully" });
            }
        }


        /*[Authorize]*/
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAccount(int id, [FromBody] Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == account.Email && a.AccountId != id);
            if (existingAccount != null)
            {
                return Conflict(new { message = "Email already exists", account = account });
            }

            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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
        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.Include(a => a.Orders).FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            int notSuccess = 0;
            foreach (var order in account.Orders)
            {
                if (order.Status != 3)
                    notSuccess++;
            }
            if (notSuccess != 0)
            {
                return Conflict(new { message = "This account has orders not success, cannot be deleted!" });
            }
            if (account.Role == 0)
            {
                return Conflict(new { message = "This account is admin, cannot be deleted!" });
            }
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete success" });
        }


        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
        private string GenerateJwtToken(Account account)
        {
            var key = _configuration["Jwt:Key"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, account.Role == 1 ? "user" : "admin"),
                new Claim(ClaimTypes.Name, account.AccountName),
                new Claim(ClaimTypes.MobilePhone, account.PhoneNumber),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
