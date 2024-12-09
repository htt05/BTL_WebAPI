using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using X.PagedList.Extensions;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class PageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/";
        HttpClient client;

        public PageController(ILogger<HomeController> logger, SessionWork sessionWork)
        {
            _logger = logger;
            _sessionWork = sessionWork;
            client = new HttpClient { BaseAddress = new Uri(uri) };
        }
        public IActionResult Contact()
        {
            return View("/Views/Page/Contact.cshtml");
        }
        public IActionResult About_Us()
        {
            return View("/Views/Page/About_Us.cshtml");
        }
        public IActionResult FAQ()
        {
            return View("/Views/Page/FAQ.cshtml");
        }
        public async Task<IActionResult> Shop(string? sort, string? searchName, int? categoryId, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            int pageSize = 12;
            var Data = new List<SelectListItem>
    {
        new SelectListItem { Value = "No", Text = "Relevance" },
        new SelectListItem { Value = "NameInt", Text = "Name, A to Z" },
        new SelectListItem { Value = "NameDesc", Text = "Name, Z to A" },
        new SelectListItem { Value = "PriceInt", Text = "Price, Low to High" },
        new SelectListItem { Value = "PriceDesc", Text = "Price, High to Low" }
    };
            ViewBag.Options = new SelectList(Data, "Value", "Text", sort);

            var response = await client.GetStringAsync(uri + $"Products?searchName={searchName}&categoryId={categoryId}");
            var products = JsonConvert.DeserializeObject<List<Product>>(response);

            if (!string.IsNullOrEmpty(searchName))
            {
                ViewBag.searchName = searchName;
            }

            if (categoryId != null)
            {
                ViewBag.id = categoryId;
            }

            // Sắp xếp danh sách sản phẩm
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sort = sort;
                switch (sort)
                {
                    case "NameDesc":
                        products = products.OrderByDescending(p => p.ProductName).ToList();
                        break;
                    case "NameInt":
                        products = products.OrderBy(p => p.ProductName).ToList();
                        break;
                    case "PriceInt":
                        products = products.OrderBy(p => p.Price).ToList();
                        break;
                    case "PriceDesc":
                        products = products.OrderByDescending(p => p.Price).ToList();
                        break;
                    default:
                        products = products.OrderBy(p => p.ProductName).ToList();
                        break;
                }
            }

            // Phân trang danh sách sản phẩm
            var pagedProducts = products.ToPagedList(page, pageSize);

            return View("/Views/Page/Shop.cshtml", pagedProducts);
        }

        public IActionResult Blog()
        {
            return View("/Views/Page/Blog.cshtml");
        }
        public async Task<IActionResult> Single_Product(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var response = await client.GetStringAsync(uri + "Products/" + id);
                var product = JsonConvert.DeserializeObject<Product>(response);
                if (product == null)
                {
                    return NotFound(); // Xử lý khi sản phẩm không tồn tại
                }
                var response1 = await client.GetStringAsync(uri + $"Products");
                var products = JsonConvert.DeserializeObject<List<Product>>(response1);
                ViewBag.TopProducts = products.Take(12);
                ViewBag.Cart = new Cart();
                return View("/Views/Page/Single_Product.cshtml", product);
            }
            return BadRequest("Product ID is required"); // Xử lý khi productId là null hoặc rỗng
        }
    }
}
