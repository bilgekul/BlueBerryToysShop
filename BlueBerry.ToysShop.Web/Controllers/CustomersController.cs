using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlueBerry.ToysShop.Web.Controllers
{
	public class CustomersController : Controller
	{
		private readonly WebDbContext _context;
		private readonly IMapper _mapper;

		public CustomersController(WebDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult CustomerRegister()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CustomerRegister(CustomerViewModel model, [FromForm] string First, [FromForm] string Last)
		{    	
			if (ModelState.IsValid)
			{   
				model.FullName = First+"-"+Last;
				var customer = _mapper.Map<Customer>(model);
				// Kullanıcıyı veritabanına kaydetme işlemlerini gerçekleştirin
				// Örneğin:
				

				// Veritabanına kullanıcıyı kaydedin
				_context.Customers.Add(customer);
			    await _context.SaveChangesAsync();

				// Kayıt işlemi başarılıysa yönlendirilecek sayfa
				return RedirectToAction("Login", "Customers");
			}
            // Hata durumunda, kayıt sayfasını tekrar gösterin
            return View(model);
		}


		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(CustomerViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _context.Admins.SingleOrDefaultAsync(u => u.Email == model.Email);
			if (user != null && user.Password == model.Password)
			{
				var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Email),
				new Claim(ClaimTypes.Role, "Customer")
			};

				var userIdentity = new ClaimsIdentity(claims, "CustomerAuthentication");

				var authProperties = new AuthenticationProperties
				{
					IsPersistent = model.RememberMe
				};

				await HttpContext.SignInAsync("CustomerAuthentication", new ClaimsPrincipal(userIdentity), authProperties);

				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> CustomerLogout()
		{
			await HttpContext.SignOutAsync("CustomerAuthentication");

			return RedirectToAction("Index", "Home");
		}


	}
}
