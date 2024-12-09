using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace front_end_ASP.NET_Core_MVC_.ViewComponents
{
    public class MiniCartViewComponent : ViewComponent
    {
        private readonly SessionWork _sessionWork;
        public MiniCartViewComponent(SessionWork sessionWork)
        {
            _sessionWork = sessionWork;
        }
        private List<Cart> carts = new List<Cart>();
        private const string CartSessionKey = "CartSession";
        public IViewComponentResult Invoke(string viewName)
        {
            carts = _sessionWork.GetCartFromSession();
            ViewBag.CoutCartItem = carts.Count;
            double? totalCart = 0;
            foreach (Cart c in carts)
            {
                totalCart += (c.Quantity * c.Price);
            }
            ViewBag.TotalCart = totalCart;
            return View(viewName, carts);
        }
    }
}
