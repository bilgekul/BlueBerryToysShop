using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace BlueBerry.ToysShop.Web.Controllers
{
    public class AdminsController:Controller
    {
        private readonly WebDbContext _context;
        private readonly IMapper _mapper;

        public AdminsController(WebDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
		[HttpGet]
		public IActionResult AdminRegister()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AdminRegister(AdminViewModel model, [FromForm] string First, [FromForm] string Last)
		{
			if (ModelState.IsValid)
			{
				// Kullanıcıyı veritabanına kaydetme işlemlerini gerçekleştirin
				// Örneğin:

				model.FullName = First + "-" + Last;
				// Veritabanına kullanıcıyı kaydedin
				_context.Admins.Add(_mapper.Map<Admin>(model));
				await _context.SaveChangesAsync();
				// Kayıt işlemi başarılıysa yönlendirilecek sayfa
				return RedirectToAction("Login", "Admins");
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
        public async Task<IActionResult> Login(AdminViewModel model)
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
                new Claim(ClaimTypes.Role, "Admin")
            };

                var userIdentity = new ClaimsIdentity(claims, "AdminAuthentication");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync("AdminAuthentication", new ClaimsPrincipal(userIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
            return View(model);
        }
		[HttpPost]
		public async Task<IActionResult> AdminLogout()
		{
			await HttpContext.SignOutAsync("AdminAuthentication");

			return RedirectToAction("Index", "Home");
		}
	}
}
