using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlueBerry.ToysShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebDbContext _context;
        public HomeController(ILogger<HomeController> logger, WebDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}