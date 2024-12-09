using front_end_ASP.NET_Core_MVC_.Hubs;
using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using front_end_ASP.NET_Core_MVC_.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/";
        HttpClient _client;

        public OrdersController(SessionWork sessionWork, IHubContext<UpdateHub> hubContext)
        {
            _sessionWork = sessionWork;
            _hubContext = hubContext;
            _client = new HttpClient { BaseAddress = new Uri(uri) };
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(string? productId)
        {
            var user = _sessionWork.GetUserFromSession();
            if (user.AccountName == null)
            {
                State redirect = new State() { StateName = "RedirectLogin", StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var token = user.Token;
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (productId != null)
                {
                    var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync(uri + "Products/" + productId));
                    List<Cart> result = new List<Cart>();
                    if (product != null)
                    {
                        var itemCart = new Cart() { ProductId = product.ProductId, Name = product.ProductName, Picture = product.Picture, Price = product.SalePrice, Quantity = 1 };
                        result.Add(itemCart);
                        _sessionWork.SaveBuyNowProductToSession(product);
                    }
                    ViewBag.clearCart = 0;
                    ViewBag.carts = result;

                }
                else
                {
                    ViewBag.clearCart = 1;
                    ViewBag.carts = _sessionWork.GetCartFromSession();
                }
                ViewBag.orderId = GenerateRandomString();
                ViewBag.user = user;
                return View("Create");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,Receiver,PhoneNumber,Address,ToltalPrice,Note,Status,Created_at,AccountId")] Order order, byte clearCart)
        {
            var user = _sessionWork.GetUserFromSession();
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (ModelState.IsValid)
            {
                order.Created_at = DateTime.Now;
                var response = await _client.PostAsJsonAsync(uri + "Orders", order);
                if (clearCart == 0)
                {
                    var product = _sessionWork.GetBuyNowProductFromSession();
                    var respone = await _client.PostAsJsonAsync(uri + "OrderDetails", new
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = 1,
                    });
                }
                else
                {
                    var carts = _sessionWork.GetCartFromSession();
                    foreach (var c in carts)
                    {
                        var res = await _client.PostAsJsonAsync(uri + "OrderDetails", new
                        {
                            OrderId = order.OrderId,
                            ProductId = c.ProductId,
                            Quantity = c.Quantity,
                        });
                    }
                    HttpContext.Session.Remove(_sessionWork.CartSessionKey);
                }
                await _hubContext.Clients.All.SendAsync("ClientNotice");
                Console.WriteLine("Order Created notification sent.");
                State state = new State() { StateName = "success", StateMessage = "<div class='alert alert-success'>You have successfully placed your order.</div>" };
                _sessionWork.SaveStateToSession(state);
                return RedirectToAction("Index", "Account", new { accountId = user.AccountId });
            }
            return RedirectToAction("Create", "Orders");
        }

        public async Task<IActionResult> Edit(string? orderId)
        {
            var user = _sessionWork.GetUserFromSession();
            if (user.AccountName == null)
            {
                State redirect = new State() { StateName = "RedirectLogin", StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            if (orderId == null)
            {
                return NotFound();
            }
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var order = JsonConvert.DeserializeObject<Order>(await _client.GetStringAsync(uri + "Orders/" + orderId));
            ViewBag.orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await _client.GetStringAsync(uri + "Orders/" + orderId + "/OrderDetails"));
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,Receiver,PhoneNumber,Address,ToltalPrice,Note,Status,Created_at,AccountId")] Order order)
        {
            var user = _sessionWork.GetUserFromSession();
            if (user.AccountName == null)
            {
                State redirect = new State() { StateName = "RedirectLogin", StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                    await _hubContext.Clients.All.SendAsync("ClientNotice");
                    Console.WriteLine("Order Created notification sent.");
                    State state = new State() { StateName = "success", StateMessage = "<div class='alert alert-success'>You have successfully updated your order.</div>" };
                    _sessionWork.SaveStateToSession(state);
                    return RedirectToAction("Index", "Account", new { accountId = order.AccountId });
                }
            }
            ViewBag.orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await _client.GetStringAsync(uri + "Orders/" + id + "/OrderDetails"));
            return View(order);
        }
        public async Task<IActionResult> Cancel(string orderId)
        {
            var user = _sessionWork.GetUserFromSession();
            if (user.AccountName == null)
            {
                State redirect = new State() { StateName = "RedirectLogin", StateMessage = "<div class='alert alert-success'>You need to log in first</div>" };
                _sessionWork.SaveStateToSession(redirect);
                return RedirectToAction("Index", "Login");
            }
            var token = user.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetStringAsync("Orders/" + orderId + "/Cancel");
            var result = JsonConvert.DeserializeObject<dynamic>(response);
            if (result.result == "true")
            {
                int id = (int)result.accountId;
                await _hubContext.Clients.All.SendAsync("ClientNotice");
                Console.WriteLine("Order Created notification sent.");
                State state = new State() { StateName = "success", StateMessage = "<div class='alert alert-success'>You have successfully canceled your order.</div>" };
                _sessionWork.SaveStateToSession(state);
                return RedirectToAction("Index", "Account", new { accountId = id });
            }
            else
            {
                return View("Error");
            }
        }
        public static string GenerateRandomString()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                char randomChar = chars[random.Next(chars.Length)];
                stringBuilder.Append(randomChar);
            }
            return stringBuilder.ToString();
        }
    }
}
