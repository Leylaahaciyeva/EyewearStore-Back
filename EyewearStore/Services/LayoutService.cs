using EyewearStore.Contexts;
using EyewearStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EyewearStore.Services;

public class LayoutService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<AppUser> _userManager;
    public LayoutService(AppDbContext context, IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
    {
        _context = context;
        _httpContext = httpContext;
        _userManager = userManager;
    }

    public async Task<Dictionary<string, string>> GetSettingsAsync()
    {
        var settings = await _context.Settings.ToDictionaryAsync(x=>x.Key, x=>x.Value);

        return settings;
    }

    public async Task<List<BasketItem>> GetBasketAsync()
    {
        if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new();
            }
            var BasketItems = await _context.BasketItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == userId).ToListAsync();
            return BasketItems;

        }

        var basktItms = _getBasket();
        if (basktItms is null || basktItms.Count == 0)
            basktItms = new();

        foreach (var item in basktItms)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
            item.Product = product;

        }
        return basktItms;


    }


    private List<BasketItem> _getBasket()
    {
        List<BasketItem> basketItems = new();
        if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(_httpContext.HttpContext.Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}
