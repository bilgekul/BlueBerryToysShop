using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;

namespace BlueBerry.ToysShop.Web.Controllers
{
	public class AdminsController : Controller
	{
		private readonly UserManager<Admin> _userManager;
		private readonly SignInManager<Admin> _signInManager;
		private readonly IMapper _mapper;
		private readonly WebDbContext _context;

		public AdminsController(UserManager<Admin> userManager, SignInManager<Admin> signInManager,IMapper mapper, WebDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_mapper = mapper;
			_context = context;
		}
		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult Register()
		{
         
            return View();
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Register(AdminViewModel model, [FromForm]string First, [FromForm]string Last)
		{
			model.FullName = First + "-" + Last;
			if (ModelState.IsValid)
			{
				var admin = _mapper.Map<Admin>(model);
				
				_context.Admins.Add(admin);
				await _context.SaveChangesAsync();

                return RedirectToAction("DisplayProduct", "Products");
			}
            return View(model);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Login(AdminViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
					if (result.Succeeded)
					{
						return RedirectToAction("Index", "Home");
					}
				}

				ModelState.AddModelError(string.Empty, "Invalid email or password.");
			}

			return View(model);
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}

