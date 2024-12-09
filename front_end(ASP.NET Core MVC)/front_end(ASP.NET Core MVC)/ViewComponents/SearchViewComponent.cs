using Microsoft.AspNetCore.Mvc;

namespace front_end_ASP.NET_Core_MVC_.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string viewName)
        {
            return View(viewName);
        }
    }
}
