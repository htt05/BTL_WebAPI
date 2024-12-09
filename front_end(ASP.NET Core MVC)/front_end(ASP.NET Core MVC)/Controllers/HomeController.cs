using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionWork _sessionWork;
        private readonly string uri = "https://localhost:44377/api/";
        HttpClient client;
        public HomeController(ILogger<HomeController> logger, SessionWork sessionWork)
        {
            _logger = logger;
            _sessionWork = sessionWork;
            client = new HttpClient { BaseAddress = new Uri(uri) };
        }

        public async Task<IActionResult> Index()
        {
            var response = await client.GetStringAsync(uri + $"Products");
            var products = JsonConvert.DeserializeObject<List<Product>>(response);
            ViewBag.TopProducts = products.Take(12);
            return View();
        }/*

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(errorViewModel);
        }*/
    }
}
