using front_end_ASP.NET_Core_MVC_.Hubs;
using front_end_ASP.NET_Core_MVC_.Models.BusinessModels;
using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class AccountController : Controller
    {
        private List<Cart> carts = new List<Cart>();

        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/";
        HttpClient _client;

        public AccountController(SessionWork sessionWork, IHubContext<UpdateHub> hubContext)
        {
            _sessionWork = sessionWork;
            _hubContext = hubContext;
            _client = new HttpClient { BaseAddress = new Uri(uri) };
        }
        public async Task<IActionResult> Index(int accountId)
        {
            var state = _sessionWork.GetStateFromSession();
            var user = _sessionWork.GetUserFromSession();
            if (user.AccountName == null)
            {
                State redirect = new State() { StateName = "RedirectLogin", StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            if (accountId == null)
            {
                return NotFound();
            }
            ViewBag.state = (state.StateName == "success") ? state.StateMessage : "";
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var account = JsonConvert.DeserializeObject<Account>(await _client.GetStringAsync(uri + "Accounts/" + accountId));
            account.Token = token;
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("AccountId,AccountName,Email,Password,ConfirmPassWord,PhoneNumber,Role")] Account account, string? oldPassword)
        {
            var user = _sessionWork.GetUserFromSession();
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            account.Token = token;
            if (user.AccountName == null)
            {
                State redirect = new State() { StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            if (oldPassword != null)
            {
                if (user.Password != Cipher.GenerateMD5(oldPassword))
                {
                    ViewBag.errorOldPassword = "Current Password incorrect";
                    account.Password = null;
                    account.ConfirmPassWord = null;
                    account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                    return View("Index", account);
                }
                else if (account.Password != account.ConfirmPassWord)
                {
                    ViewBag.errorConfirmPassword = "Confirm Password does not match Password";
                    account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                    return View("Index", account);
                }
                else
                {
                    account.Password = Cipher.GenerateMD5(account.Password);
                    account.ConfirmPassWord = Cipher.GenerateMD5(account.Password);
                    if (ModelState.IsValid)
                    {
                        var response = await _client.PutAsJsonAsync(uri + "Accounts/" + account.AccountId, account);
                        if (response.IsSuccessStatusCode)
                        {
                            await _hubContext.Clients.All.SendAsync("ClientNotice");
                            State state = new State() { StateName = "success", StateMessage = "<div class='alert alert-success'>You have successfully updated your account.</div>" };
                            _sessionWork.SaveStateToSession(state);
                            _sessionWork.SaveUserToSession(account);
                        }
                        return RedirectToAction("Index", "Account", new { accountId = account.AccountId });
                    }
                    else
                    {
                        account.Password = null;
                        account.ConfirmPassWord = null;
                        account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                        return View("Index", account);
                    }
                }
            }
            else
            {
                if (account.Password != null)
                {
                    ViewBag.errorOldPassword = "You need to enter your old password";
                    account.Password = null;
                    account.ConfirmPassWord = null;
                    account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                    return View("Index", account);
                }
                else if (account.ConfirmPassWord != null)
                {
                    ViewBag.errorOldPassword = "You need to enter your old password";
                    account.Password = null;
                    account.ConfirmPassWord = null;
                    account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                    return View("Index", account);
                }
                else
                {
                    if (account.Email == null || account.AccountName == null || account.PhoneNumber == null)
                    {
                        ViewBag.error = "<div class='alert alert-danger'>You entered missing account information.</div>";
                        account.Password = null;
                        account.ConfirmPassWord = null;
                        account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                        return View("Index", account);
                    }
                    else if (!Regex.IsMatch(account.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    {
                        ViewBag.errorEmail = "Invalid email address";
                        account.Password = null;
                        account.ConfirmPassWord = null;
                        account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                        return View("Index", account);
                    }
                    else if (!Regex.IsMatch(account.PhoneNumber, @"^0\d{9}$"))
                    {
                        ViewBag.phoneNumber = "Invalid phone number";
                        account.Password = null;
                        account.ConfirmPassWord = null;
                        account.Orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?accountId=" + account.AccountId));
                        return View("Index", account);
                    }
                    else
                    {
                        account.Password = user.Password;
                        var response = await _client.PutAsJsonAsync(uri + "Accounts/" + account.AccountId, account);
                        if (response.IsSuccessStatusCode)
                        {
                            await _hubContext.Clients.All.SendAsync("ClientNotice");
                            State state = new State() { StateName = "success", StateMessage = "<div class='alert alert-success'>You have successfully updated your account.</div>" };
                            _sessionWork.SaveStateToSession(state);
                            _sessionWork.SaveUserToSession(account);
                        }
                        return RedirectToAction("Index", "Account", new { accountId = account.AccountId });
                    }
                }
            }
        }
    }
}
