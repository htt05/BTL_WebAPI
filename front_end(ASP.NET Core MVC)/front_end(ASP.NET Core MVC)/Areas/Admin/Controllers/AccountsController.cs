using front_end_ASP.NET_Core_MVC_.Hubs;
using front_end_ASP.NET_Core_MVC_.Models.BusinessModels;
using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using X.PagedList.Extensions;

namespace front_end_ASP.NET_Core_MVC_.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly HttpClient _client;
        string uri = "https://localhost:44377/api/Accounts";
        public AccountsController(IHubContext<UpdateHub> hubContext, HttpClient client)
        {
            _hubContext = hubContext;
            _client = client;
        }

        public async Task<IActionResult> Index(string? searchName, int page = 1)
        {
            // Thêm token vào header của Http_client
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "?searchName=" + searchName);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await _client.GetStringAsync(""));
            //phan trang
            if (page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            var results = accounts.ToPagedList(page, pageSize);
            ViewBag.searchName = searchName;
            return View(results);
        }

        public IActionResult Create()
        {
            ViewData["options"] = new List<SelectListItem>
                                    {
                                    new SelectListItem { Value = "0", Text = "Admin" },
                                    new SelectListItem { Value = "1", Text = "User"}
                                    };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,AccountName,Email,Password,ConfirmPassWord,PhoneNumber,Role")] Account account)
        {
            ViewData["options"] = new List<SelectListItem>
                                    {
                                    new SelectListItem { Value = "0", Text = "Admin" },
                                    new SelectListItem { Value = "1", Text = "User"}
                                    };
            if (ModelState.IsValid)
            {
                _client.BaseAddress = new Uri(uri);
                var response = await _client.PostAsJsonAsync("", new { AccountName = account.AccountName, Email = account.Email, Password = account.Password, PhoneNumber = account.PhoneNumber, Role = account.Role });
                if (response.IsSuccessStatusCode)
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    TempData["success"] = responseMessage.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    ViewBag.error = responseMessage.Message;
                    return View(account);
                }
            }
            return View(account);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Thêm token vào header của Http_client
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "/" + id);
            var account = JsonConvert.DeserializeObject<Account>(await _client.GetStringAsync(""));
            if (account == null)
            {
                return (NotFound());
            }
            account.ConfirmPassWord = account.Password;
            byte allowEdit = 0;
            if (account.Email == User.Claims.Skip(0).FirstOrDefault().Value)
            {
                allowEdit = 1;
            }
            ViewData["options"] = new List<SelectListItem>
                            {
                            new SelectListItem { Value = "0", Text = "Admin" },
                            new SelectListItem { Value = "1", Text = "User"}
                            };
            ViewBag.allowEdit = allowEdit;
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,AccountName,Email,Password,ConfirmPassWord,PhoneNumber,Role")] Account account, byte allowEdit)
        {
            ViewData["options"] = new List<SelectListItem>
    {
        new SelectListItem { Value = "0", Text = "Admin" },
        new SelectListItem { Value = "1", Text = "User"}
    };

            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Thêm token vào header của Http_client
                var token = User.FindFirst("Token")?.Value;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _client.BaseAddress = new Uri(uri + "/" + id);
                if (allowEdit == 1)
                {
                    account.Password = Cipher.GenerateMD5(account.Password);
                }
                var response = await _client.PutAsJsonAsync("", account);
                if (response.IsSuccessStatusCode)
                {
                    await _hubContext.Clients.All.SendAsync("AdminNotice");
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    TempData["success"] = responseMessage.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                    ViewBag.error = responseMessage.Message;
                    ViewBag.allowEdit = allowEdit;
                    return View(account);
                }
            }
            else
            {
                ViewBag.allowEdit = allowEdit;
                return View(account);
            }
        }


        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Thêm token vào header của Http_client
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "/" + id);
            var account = JsonConvert.DeserializeObject<Account>(await _client.GetStringAsync(""));
            if (account == null)
            {
                return (NotFound());
            }
            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Thêm token vào header của Http_client
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "/" + id);
            var response = await _client.DeleteAsync("");
            if (response.IsSuccessStatusCode)
            {
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                TempData["success"] = responseMessage.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                var account = JsonConvert.DeserializeObject<Account>(await _client.GetStringAsync(""));
                ViewBag.error = responseMessage.Message;
                return View(account);
            }
        }
    }
}
