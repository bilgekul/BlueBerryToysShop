using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.Models.SelectLists;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Linq;


namespace BlueBerry.ToysShop.Web.Controllers
{
    public class ProductsController:Controller
    {
        private readonly WebDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;

        public ProductsController(WebDbContext context, IMapper mapper, IFileProvider provider)
        {
            _context = context;
            _mapper = mapper;
            _fileProvider = provider;
        }
        [Authorize(Roles ="Admin, Customer")]
        [HttpGet]
        public IActionResult DisplayProduct()
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
        [Authorize(Roles = "Admin, Customer")]
        [HttpPost]
		public IActionResult DisplayProduct([FromForm] string productName, [FromForm] decimal? minPrice, [FromForm] decimal? maxPrice, [FromForm] List<string> brands, [FromForm] List<int> categories)
		{
			var query = _context.Products.Include(x => x.Category).AsQueryable();

			if (!string.IsNullOrEmpty(productName))
			{
				query = query.Where(x => x.Name.Contains(productName));
			}

			if (minPrice.HasValue)
			{
				query = query.Where(x => x.Price >= minPrice.Value);
			}

			if (maxPrice.HasValue)
			{
				query = query.Where(x => x.Price <= maxPrice.Value);
			}

			if (brands != null && brands.Count > 0)
			{
				query = query.Where(x => brands.Contains(x.Brand));
			}

			if (categories != null && categories.Count > 0)
			{
				query = query.Where(x => categories.Contains(x.CategoryId));
			}

			var products = query.Select(x => new ProductViewModel()
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

			return PartialView("_ProductListPartial", products);
		}
        [Authorize(Roles = "Admin, Customer")]
        [HttpPost]
		public IActionResult DisplayProductMultiFilter(ProductFilterViewModel filters)
		{
			var query = _context.Products.Include(x => x.Category).AsQueryable();

			if (!string.IsNullOrEmpty(filters.ProductName))
			{
				query = query.Where(x => x.Name.Contains(filters.ProductName));
			}

			if (filters.MinPrice.HasValue)
			{
				query = query.Where(x => x.Price >= filters.MinPrice.Value);
			}

			if (filters.MaxPrice.HasValue)
			{
				query = query.Where(x => x.Price <= filters.MaxPrice.Value);
			}
			if (filters.Brands != null && filters.Brands.Count > 0)
			{
				query = query.Where(x => filters.Brands.Contains(x.Brand));
			}
			if (filters.CategoryIds != null && filters.CategoryIds.Count > 0)
			{
				query = query.Where(x => filters.CategoryIds.Contains(x.CategoryId));
			}

			var products = query.Select(x => new ProductViewModel()
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

			return PartialView("_ProductListPartial", products);
		}
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddProduct()
        {

            var expireSelect = new SelectList(new List<ExpireSelectList>()
            {
                new(){Data="1 Ay",Value=1},
                 new(){Data="3 Ay",Value=3},
                  new(){Data="6 Ay",Value=6},
                   new(){Data="12 Ay",Value=12},
                    new(){Data="24 Ay",Value=24}

            }, "Value", "Data");
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var item in expireSelect)
            {
                int value;
                if (int.TryParse(item.Value, out value))
                {
                    dictionary.Add(item.Text, value);
                }
                else
                {
                    dictionary.Add(item.Text, 0);
                    ModelState.AddModelError("", "Bir hata oluştu. Geçerli bir değer sağlanamadı.");
                }
            }
            ViewBag.ExpireSelect = dictionary;
            ViewBag.BrandSelect = new SelectList(new List<BrandSelectList>()
            {
                new(){Data="Lego",Value="Lego"},
                 new(){Data="FisherPrice",Value="FisherPrice"},
                  new(){Data="Barbie",Value="Barbie"},
                   new(){Data="Hot Wheels",Value="Hot Wheels"},
                    new(){Data="Crafy",Value="Crafy"},
                     new(){Data="Dollz'n More",Value="Dollz'n More"},
                      new(){Data="BLX",Value="BLX"},
                       new(){Data="Play-Doh",Value="Play-Doh"},
                        new(){Data="Hasbro",Value="Hasbro"},
            },"Value","Data");

            var categories = _context.Category?.ToList() ?? new List<Category>();

            ViewBag.categorySelect = new SelectList(categories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(ProductViewModel newProduct)
        {
            IActionResult result = null;
       
            if (ModelState.IsValid)
            {
                try
                {
                    var product = _mapper.Map<Product>(newProduct);
                    if (newProduct.Image != null && newProduct.Image.Length > 0)
                    {
                        var root = _fileProvider.GetDirectoryContents("wwwroot");
                        var images = root.First(x => x.Name == "images");
                        var randomImageName = Guid.NewGuid() + Path.GetExtension(newProduct.Image.FileName);
                        var path = Path.Combine(images.PhysicalPath, randomImageName);

                        using var stream = new FileStream(path, FileMode.Create);

                        newProduct.Image.CopyTo(stream);

                        product.ImagePath = randomImageName;
                        
                    }
                    if (newProduct.CategoryId != 0)
                    {
                        var category = _context.Category.Find(newProduct.CategoryId);
                        if (category != null)
                        {
                            product.Category = category;
                        }
                    }
                    _context.Products.Add(product);
                    _context.SaveChanges();

                        return RedirectToAction("DisplayProduct");
                }catch(Exception)
                {
                    result = View();
                }
            }
            else { result = View(); }
            
      
            var categories = _context.Category.ToList();

            ViewBag.categorySelect = new SelectList(categories, "Id", "Name");

            var expireSelect = new SelectList(new List<ExpireSelectList>()
            {
                new(){Data="1 Ay",Value=1},
                 new(){Data="3 Ay",Value=3},
                  new(){Data="6 Ay",Value=6},
                   new(){Data="12 Ay",Value=12},
                    new(){Data="24 Ay",Value=24}

            }, "Value", "Data");

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var item in expireSelect)
            {
                int value;
                if (int.TryParse(item.Value, out value))
                {
                    dictionary.Add(item.Text, value);
                }
                else
                {
                    dictionary.Add(item.Text, 0);
                    ModelState.AddModelError("", "Bir hata oluştu. Geçerli bir değer sağlanamadı.");
                }
            }

            ViewBag.ExpireSelect = dictionary;

            ViewBag.BrandSelect = new SelectList(new List<BrandSelectList>()
            {
                new(){Data="Lego",Value="Lego"},
                 new(){Data="FisherPrice",Value="FisherPrice"},
                  new(){Data="Barbie",Value="Barbie"},
                   new(){Data="Hot Wheels",Value="Hot Wheels"},
                    new(){Data="Crafy",Value="Crafy"},
                     new(){Data="Dollz'n More",Value="Dollz'n More"},
                      new(){Data="BLX",Value="BLX"},
                       new(){Data="Play-Doh",Value="Play-Doh"},
                        new(){Data="Hasbro",Value="Hasbro"},
            },"Value","Data");

            return result;
        }
        public IActionResult RemoveProduct(int productid)
        {
            var product = _context.Products.Find(productid);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("DisplayProduct");
        }
        [HttpGet]
        public IActionResult UpdateProduct(int productid)
        {
            var product = _context.Products.Find(productid);
            ViewBag.ExpireValue = product.Expire;
            var expireSelect = new SelectList(new List<ExpireSelectList>()
            {
                new(){Data="1 Ay",Value=1},
                 new(){Data="3 Ay",Value=3},
                  new(){Data="6 Ay",Value=6},
                   new(){Data="12 Ay",Value=12},
                    new(){Data="24 Ay",Value=24}

            }, "Value", "Data");
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var item in expireSelect)
            {
                int value;
                if (int.TryParse(item.Value, out value))
                {
                    dictionary.Add(item.Text, value);
                }
                else
                {
                    dictionary.Add(item.Text, 0);
                    ModelState.AddModelError("", "Bir hata oluştu. Geçerli bir değer sağlanamadı.");
                }
            }
            ViewBag.ExpireSelect = dictionary;
            ViewBag.BrandSelect = new SelectList(new List<BrandSelectList>()
            {
                new(){Data="Lego",Value="Lego"},
                 new(){Data="FisherPrice",Value="FisherPrice"},
                  new(){Data="Barbie",Value="Barbie"},
                   new(){Data="Hot Wheels",Value="Hot Wheels"},
                    new(){Data="Crafy",Value="Crafy"},
                     new(){Data="Dollz'n More",Value="Dollz'n More"},
                      new(){Data="BLX",Value="BLX"},
                       new(){Data="Play-Doh",Value="Play-Doh"},
                        new(){Data="Hasbro",Value="Hasbro"},
            }, "Value", "Data");

            var categories = _context.Category?.ToList() ?? new List<Category>();

            ViewBag.categorySelect = new SelectList(categories, "Id", "Name");

            return View(_mapper.Map<ProductUpdateViewModel>(product));
        }
        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            return View();
        }
        [Authorize(Roles = "Admin, Customer")]
        [HttpGet]
        [Route("Detaylar/Ürün/{productid}")]
        public IActionResult _DetailsProductPartial(int productid) {
            var product = _context.Products.Find(productid);
            return View(_mapper.Map<ProductViewModel>(product)); 
        }
    }
}
