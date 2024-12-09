using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class CartController : Controller
    {
        private List<Cart> carts = new List<Cart>();

        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/Products";
        HttpClient client;

        public CartController(SessionWork sessionWork)
        {
            _sessionWork = sessionWork;
            client = new HttpClient { BaseAddress = new Uri(uri) };
        }
        public async Task<IActionResult> Add(string id)
        {
            carts = _sessionWork.GetCartFromSession();
            if (carts.Any(c => c.ProductId.Equals(id)))
            {
                carts.Find(c => c.ProductId.Equals(id)).Quantity += 1;
            }
            else
            {
                var response = await client.GetStringAsync(uri + "/" + id);
                var product = JsonConvert.DeserializeObject<Product>(response);
                if (product != null)
                {
                    var itemCart = new Cart() { ProductId = product.ProductId, Name = product.ProductName, Picture = product.Picture, Price = product.SalePrice, Quantity = 1 };
                    carts.Add(itemCart);
                }
            }
            _sessionWork.SaveCartToSession(carts);
            return RedirectToAction("Index", "Cart");
        }
        public async Task<IActionResult> AddMultiple(string id, int quantity)
        {
            carts = _sessionWork.GetCartFromSession();
            if (carts.Any(c => c.ProductId.Equals(id)))
            {
                carts.Find(c => c.ProductId.Equals(id)).Quantity += quantity;
            }
            else
            {
                var response = await client.GetStringAsync(uri + "/" + id);
                var product = JsonConvert.DeserializeObject<Product>(response);
                if (product != null)
                {
                    var itemCart = new Cart() { ProductId = product.ProductId, Name = product.ProductName, Picture = product.Picture, Price = product.SalePrice, Quantity = quantity };
                    carts.Add(itemCart);
                }
            }
            _sessionWork.SaveCartToSession(carts);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult Remove(string id)
        {
            carts = _sessionWork.GetCartFromSession();
            if (carts.Any(c => c.ProductId == id)) // nếu có sản phẩm này tron Session giỏ hàng
            {
                var item = carts.Where(c => c.ProductId == id).First(); // tìm sản phẩm đó trong session giỏ hàng
                carts.Remove(item);
                _sessionWork.SaveCartToSession(carts);
            }
            return RedirectToAction("Index", "Cart");
        } // Xóa sản phẩm ra khỏi sesion
        public async Task<IActionResult> Update(string id, int quantity)
        {
            double? updatedPrice = 0;
            carts = _sessionWork.GetCartFromSession();
            if (carts.Any(c => c.ProductId == id))
            {
                var cartItem = carts.Where(c => c.ProductId == id).First();
                updatedPrice = cartItem.Price * quantity;
                carts.Where(c => c.ProductId == id).First().Quantity = quantity;
                _sessionWork.SaveCartToSession(carts);
            }
            /*else
            {
                var response = await client.GetStringAsync(uri + "/" + id);
                var product = JsonConvert.DeserializeObject<Product>(response);
                if (product != null)
                {
                    var itemCart = new Cart() { ProductId = product.ProductId, Name = product.ProductName, Picture = product.Picture, Price = product.SalePrice, Quantity = quantity };
                    updatedPrice = itemCart.Price * quantity;
                    carts.Add(itemCart);
                }
                _sessionWork.SaveCartToSession(carts);
            }*/
            return Json(new { updatedPrice = updatedPrice });
        } // cập nhật sô lượng
        public IActionResult Clear()
        {
            // lưu lại Session, chỉ cần List<Cart> => null
            HttpContext.Session.Remove(_sessionWork.CartSessionKey);
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Index()
        {
            var carts = _sessionWork.GetCartFromSession().OrderByDescending(c => c.ProductId);
            return View(carts);
        }
    }
}
