using front_end_ASP.NET_Core_MVC_.Models.DataModels;
using Newtonsoft.Json;

public class SessionWork
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionWork(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string UserSessionKey { get; } = "UserSession";
    public string CartSessionKey { get; } = "CartSession";
    public string BuyNowProductSessionKey { get; } = "BuyNowProductSession";
    public string StateSessionKey { get; } = "StateSession";
    public Account GetUserFromSession()
    {
        var userInSession = _httpContextAccessor.HttpContext.Session.GetString(UserSessionKey);
        if (!string.IsNullOrEmpty(userInSession))
        {
            return JsonConvert.DeserializeObject<Account>(userInSession);
        }
        return new Account();
    }
    public void SaveUserToSession(Account account)
    {
        var accountJson = JsonConvert.SerializeObject(account);
        _httpContextAccessor.HttpContext.Session.SetString(UserSessionKey, accountJson);
    }
    public List<Cart> GetCartFromSession()
    {
        var cartInSession = _httpContextAccessor.HttpContext.Session.GetString(CartSessionKey);
        if (!string.IsNullOrEmpty(cartInSession))
        {
            return JsonConvert.DeserializeObject<List<Cart>>(cartInSession);
        }
        return new List<Cart>();
    }
    public void SaveCartToSession(List<Cart> carts)
    {
        var cartJson = JsonConvert.SerializeObject(carts);
        _httpContextAccessor.HttpContext.Session.SetString(CartSessionKey, cartJson);
    }
    public Product GetBuyNowProductFromSession()
    {
        var productInSession = _httpContextAccessor.HttpContext.Session.GetString(BuyNowProductSessionKey);
        if (!string.IsNullOrEmpty(productInSession))
        {
            return JsonConvert.DeserializeObject<Product>(productInSession);
        }
        return new Product();
    }
    public void SaveBuyNowProductToSession(Product product)
    {
        var productJson = JsonConvert.SerializeObject(product);
        _httpContextAccessor.HttpContext.Session.SetString(BuyNowProductSessionKey, productJson);
    }
    public State GetStateFromSession()
    {
        var stateInSession = _httpContextAccessor.HttpContext.Session.GetString(BuyNowProductSessionKey);
        if (!string.IsNullOrEmpty(stateInSession))
        {
            return JsonConvert.DeserializeObject<State>(stateInSession);
        }
        return new State();
    }
    public void SaveStateToSession(State state)
    {
        var stateJson = JsonConvert.SerializeObject(state);
        _httpContextAccessor.HttpContext.Session.SetString(BuyNowProductSessionKey, stateJson);
    }

}
