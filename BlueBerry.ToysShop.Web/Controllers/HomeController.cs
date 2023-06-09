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

			var categories = _context.Category.ToList();
			ViewBag.Categories = categories;
			return View(products);
        }
        [HttpGet]
        public IActionResult Result()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Result([FromForm]string searchproduct)
        {
            if (!string.IsNullOrEmpty(searchproduct))
            {
                var product = _context.Products.Where(x => x.Name.Contains(searchproduct)).Select(x => new ProductViewModel(){Name = x.Name, Id = x.Id, ImagePath = x.ImagePath, Price = x.Price, Rating = x.Rating}).ToList();
                 
                return View(product); 
            }

            return View();
        }
		[HttpGet]
		public IActionResult GetBrand()
		{
			return View();
		}

		[HttpPost]
		public IActionResult GetBrand(string brand)
		{
			if (!string.IsNullOrEmpty(brand))
			{
				var product = _context.Products.Where(x => x.Brand.Contains(brand)).Select(x => new ProductViewModel()
				{
					Name = x.Name,
					Id = x.Id,
					ImagePath = x.ImagePath,
					Price = x.Price,
					Rating = x.Rating
				}).ToList();

				if (product.Count > 0)
				{
					return View(product);
				}
			}

			return View();
		}

		[HttpGet]
		public IActionResult GetCategory(int categoryId)
		{
				var products = _context.Products.Where(p => p.CategoryId == categoryId)
					.Select(p => new ProductViewModel()
					{
						Name = p.Name,
						Id = p.Id,
						ImagePath = p.ImagePath,
						Price = p.Price,
						Rating = p.Rating,
					}).ToList();

				return View(products);
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