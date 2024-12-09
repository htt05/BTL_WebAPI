using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class LoginController : Controller
    {
        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/Accounts";
        HttpClient client;
        public LoginController(SessionWork sessionWork)
        {
            _sessionWork = sessionWork;
            client = new HttpClient { BaseAddress = new Uri(uri) };
        }
        public IActionResult Index()
        {
            var state = _sessionWork.GetStateFromSession();
            ViewBag.state = (state.StateName == "RedirectLogin") ? state.StateMessage : "";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string? email_login, string? password)
        {
            if (string.IsNullOrEmpty(email_login) || string.IsNullOrEmpty(password))
            {
                ViewBag.errorLogin = "<div class='alert alert-danger'>Email or password is blank</div>";
                return View();
            }
            else
            {
                client.BaseAddress = new Uri(uri + "/Login");
                var response = await client.PostAsJsonAsync("", new
                {
                    Email = email_login,
                    Password = password
                });
                if (response.IsSuccessStatusCode)
                {
                    var account = JsonConvert.DeserializeObject<Account>(await response.Content.ReadAsStringAsync());
                    _sessionWork.SaveUserToSession(account);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    ViewBag.errorLogin = "<div class='alert alert-danger'>" + responseMessage.Message + "</div>";
                    return View();
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(Account account)
        {
            if (ModelState.IsValid)
            {
                client.BaseAddress = new Uri(uri);
                var response = await client.PostAsJsonAsync("", new
                { AccountName = account.AccountName, Email = account.Email, Password = account.Password, PhoneNumber = account.PhoneNumber, Role = account.Role });
                if (response.IsSuccessStatusCode)
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    TempData["success"] = "<div class='alert alert-success'>" + responseMessage.Message + "</div>";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    ViewBag.errorRegister = "<div class='alert alert-danger'>" + responseMessage.Message + "</div>";
                    return View("Index", account);
                }
            }
            return View("Index", account);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(_sessionWork.UserSessionKey);
            return RedirectToAction("Index");
        }
    }
}
