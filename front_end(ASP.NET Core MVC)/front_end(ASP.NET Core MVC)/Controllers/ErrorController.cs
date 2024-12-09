using front_end_ASP.NET_Core_MVC_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace front_end_ASP.NET_Core_MVC_.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(errorViewModel);
        }
    }
}
