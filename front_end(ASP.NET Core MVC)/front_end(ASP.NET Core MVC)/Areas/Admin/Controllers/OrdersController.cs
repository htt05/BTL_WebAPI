using front_end_ASP.NET_Core_MVC_.Hubs;
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
    public class OrdersController : Controller
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly string uri = "https://localhost:44377/api/";
        HttpClient _client;
        public OrdersController(IHubContext<UpdateHub> hubContext)
        {
            _hubContext = hubContext;
            _client = new HttpClient { BaseAddress = new Uri(uri) };
        }
        // GET: Admin/Orders
        public async Task<IActionResult> Index(string? searchName, int page = 1)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var orders = JsonConvert.DeserializeObject<List<Order>>(await _client.GetStringAsync(uri + "Orders?searchName=" + searchName));
            //phan trang
            if (page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            var results = orders.ToPagedList(page, pageSize);
            ViewBag.searchName = searchName;
            return View(results);
        }

        // GET: Admin/Orders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            ViewData["options"] = new List<SelectListItem>
                            {
                            new SelectListItem { Value = "0", Text = "Not processed" },
                            new SelectListItem { Value = "1", Text = "Preparing order"},
                            new SelectListItem { Value = "2", Text = "In transit"},
                            new SelectListItem { Value = "3", Text = "Delivered"},
                            new SelectListItem { Value = "4", Text = "Canceled"},
                            };
            var order = JsonConvert.DeserializeObject<Order>(await _client.GetStringAsync(uri + "Orders/" + id));
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await _client.GetStringAsync(uri + "Orders/" + id + "/OrderDetails"));
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,Receiver,PhoneNumber,Address,ToltalPrice,Note,Status,Created_at,AccountId")] Order order)
        {
            var token = User.FindFirst("Token")?.Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            ViewData["options"] = new List<SelectListItem>
                            {
                            new SelectListItem { Value = "0", Text = "Not processed" },
                            new SelectListItem { Value = "1", Text = "Preparing order"},
                            new SelectListItem { Value = "2", Text = "In transit"},
                            new SelectListItem { Value = "3", Text = "Delivered"},
                            };
            var orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await _client.GetStringAsync(uri + "Orders/" + id + "/OrderDetails"));
            ViewBag.orderDetails = orderDetails;
            if (id != order.OrderId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var response = await _client.PutAsJsonAsync("Orders/" + order.OrderId, order);

                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {

                    TempData["success"] = responseMessage?.Message ?? "Product updated successfully!";
                    await _hubContext.Clients.All.SendAsync("AdminNotice");
                    Console.WriteLine("Order updated notification sent.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.error = responseMessage?.Message ?? "An error occurred while updating the order.";
                    return View("/Areas/Admin/Views/Orders/Details.cshtml", order);
                }
            }
            return View("/Areas/Admin/Views/Orders/Details.cshtml", order);
        }
    }
}
