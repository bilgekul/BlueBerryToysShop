using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BlueBerry.ToysShop.Web.Controllers
{
	[Authorize(Roles = "Customer")]
	public class CustomersController:Controller
	{

		private readonly WebDbContext _context;
        private readonly IMapper _mapper;
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(CustomerViewModel customer, [FromForm] string First, [FromForm] string Last)
        {
            customer.FullName = First + "-" + Last;
            _context.Customers.Add(_mapper.Map<Customer>(customer));
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        {
            {
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, existingCustomer.Email),
					new Claim(ClaimTypes.Role, "Customer")
				};

				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
        }
        [HttpGet]
        {
        }
		[HttpGet]
		[Authorize(Roles = "Customer")]
		public IActionResult CustomerProfile()
		{
			// Get the currently authenticated user's email
			var email = User.Identity.Name;

			// Get the customer from the database using the email
			var customer = _context.Customers.FirstOrDefault(c => c.Email == email);

			// Map the customer to the view model
			var customerViewModel = _mapper.Map<CustomerViewModel>(customer);

        }

        [HttpPost]
        public IActionResult addToCart()
        {
            return View();
        }
        [HttpGet]
        public IActionResult viewCart() { 
        
        
            return View();
        }
        
    }
}
