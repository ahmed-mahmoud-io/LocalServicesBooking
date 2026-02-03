using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProviderService _providerService;

    public HomeController(ILogger<HomeController> logger, IProviderService providerService)
    {
        _logger = logger;
        _providerService = providerService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _providerService.GetCategoriesAsync();
        ViewBag.Categories = categories.Take(6).ToList();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult CommunityChat()
    {
        return View();
    }

    public IActionResult DesignShowcase()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
