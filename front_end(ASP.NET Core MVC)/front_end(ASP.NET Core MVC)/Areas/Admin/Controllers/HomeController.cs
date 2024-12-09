using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace front_end_ASP.NET_Core_MVC_.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        // code bắt api
        private readonly HttpClient _client;
        string uri = "https://localhost:44377/api/Accounts";
        public HomeController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                ViewBag.error = "Email or password is blank";
                return View(loginModel);
            }

            _client.BaseAddress = new Uri(uri + "/Login");
            var response = await _client.PostAsJsonAsync("", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var account = JsonConvert.DeserializeObject<Account>(await response.Content.ReadAsStringAsync());
                if (account.Role != 0)
                {
                    ViewBag.error = "You are not Admin";
                    ViewBag.Email = loginModel.Email;
                    return View(loginModel);
                }
                else
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim("Email", account.Email),
                        new Claim("AccountName", account.AccountName),
                        new Claim("PhoneNumber", account.PhoneNumber),
                        new Claim("Token", account.Token)
                    }, "CookieAdmin");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("CookieAdmin", principal);

                    return RedirectToAction("Index");
                }
            }
            else
            {
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                ViewBag.error = responseMessage.Message;
                return View(loginModel);
            }
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("CookieAdmin");
            return RedirectToAction("Index");
        }
    }
}
