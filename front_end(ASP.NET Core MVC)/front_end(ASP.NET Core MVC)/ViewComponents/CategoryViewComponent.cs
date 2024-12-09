using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace front_end_ASP.NET_Core_MVC_.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        string uri = "https://localhost:44377/api/Categories";
        HttpClient client = new HttpClient();
        public async Task<IViewComponentResult> InvokeAsync(string viewName)
        {
            client.BaseAddress = new Uri(uri);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync(""));
            if (string.IsNullOrEmpty(viewName))
                viewName = "Default";
            return View(viewName, categories);

        }
    }
}
