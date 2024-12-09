using Microsoft.AspNetCore.Mvc;

public class HeaderTopViewComponent : ViewComponent
{
    private readonly SessionWork _sessionWork;

    public HeaderTopViewComponent(SessionWork sessionWork)
    {
        _sessionWork = sessionWork;
    }

    public IViewComponentResult Invoke(string viewName)
    {
        ViewBag.User = _sessionWork.GetUserFromSession();
        return View(viewName);
    }
}
