using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace BlueBerry.ToysShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<ProductViewModel> products = _context.Products.Include(x => x.Category).Select(x => new ProductViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                CategoryName = x.Category.Name,
                Description = x.Description,
                Expire = x.Expire,
                ImagePath = x.ImagePath,
                PublishDate = x.PublishDate
            }).ToList();

            ViewBag.BrandSelect = new Dictionary<string, string>
            {
                { "Lego", "Lego" },
                { "FisherPrice", "FisherPrice" },
                { "Barbie", "Barbie" },
                { "Hot Wheels", "Hot Wheels" },
                { "Crafy", "Crafy" },
                { "Dollz'n More", "Dollz'n More" },
                { "BLX", "BLX" },
                { "Play-Doh", "Play-Doh" },
                { "Hasbro", "Hasbro" }
            };
            var categories = _context.Category?.ToList() ?? new List<Category>();
            var categorySelect = new Dictionary<int, string>();
            foreach (var category in categories)
            {
                categorySelect.Add(category.Id, category.Name);
            }

            ViewBag.categorySelect = categorySelect;
            return View(products);
        }
        [HttpGet]
        public IActionResult Result()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Result(string searchproduct)
        {
            if (!string.IsNullOrEmpty(searchproduct))
            {
                var product = _context.Products.Where(x => x.Name.Contains(searchproduct)).Select(x => new ProductViewModel(){Name = x.Name, Id = x.Id, ImagePath = x.ImagePath, Price = x.Price}).ToList();
                 
                return View(product); 
            }

            return Json("Hata");
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
        public IActionResult AccessDenied() => View();
    }
}