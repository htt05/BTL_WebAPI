using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using X.PagedList.Extensions;

namespace front_end_ASP.NET_Core_MVC_.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly HttpClient _client;
        string uri = "https://localhost:44377/api/Categories";
        public CategoriesController(HttpClient client)
        {
            _client = client;
        }
        // GET: Admin/Categories
        public async Task<IActionResult> Index(string? searchName, int page = 1)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "?searchName=" + searchName);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await _client.GetStringAsync(""));
            //phan trang
            if (page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            var results = categories.ToPagedList(page, pageSize);
            ViewBag.searchName = searchName;
            return View(results);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["options"] = new List<SelectListItem>
                             {
                             new SelectListItem { Value = "true", Text = "Enable" },
                             new SelectListItem { Value = "false", Text = "Disable"}
                             };
            return View();

        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Status")] Category category)
        {
            ViewData["options"] = new List<SelectListItem>
                            {
                            new SelectListItem { Value = "true", Text = "Enable" },
                            new SelectListItem { Value = "false", Text = "Disable"}
                            };
            if (ModelState.IsValid)
            {
                var token = User.FindFirst("Token")?.Value;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _client.BaseAddress = new Uri(uri);
                var response = await _client.PostAsJsonAsync("", new { CategoryName = category.CategoryName, Status = category.Status });
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
                    return View(category);
                }
            }
            return View(category);

        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["options"] = new List<SelectListItem>
                              {
                               new SelectListItem { Value = "true", Text = "Enable" },
                              new SelectListItem { Value = "false", Text = "Disable"}
                              };
            if (id == null)
            {
                return NotFound();
            }
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "/" + id);
            var category = JsonConvert.DeserializeObject<Category>(await _client.GetStringAsync(""));
            if (category == null)
            {
                return (NotFound());
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Status")] Category category)
        {
            ViewData["options"] = new List<SelectListItem>
                            {
                            new SelectListItem { Value = "true", Text = "Enable" },
                            new SelectListItem { Value = "false", Text = "Disable"}
                            };
            if (id != category.CategoryId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var token = User.FindFirst("Token")?.Value;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _client.BaseAddress = new Uri(uri + "/" + id);
                var response = await _client.PutAsJsonAsync("", category);
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());
                if (response.IsSuccessStatusCode)
                {

                    TempData["success"] = responseMessage.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.error = responseMessage.Message;
                    return View(category);
                }
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri(uri + "/" + id);
            var category = JsonConvert.DeserializeObject<Category>(await _client.GetStringAsync(""));
            if (category == null)
            {
                return (NotFound());
            }
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
                var category = JsonConvert.DeserializeObject<Category>(await _client.GetStringAsync(""));
                ViewBag.error = responseMessage.Message;
                return View(category);
            }
        }

    }
}
