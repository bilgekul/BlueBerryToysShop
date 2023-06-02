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
        
         public CustomersController(WebDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public IActionResult SignUp(CustomerViewModel customer, [FromForm] string First, [FromForm] string Last)
        {
            customer.FullName = First + "-" + Last;
            _context.Customers.Add(_mapper.Map<Customer>(customer));
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
		[Authorize(Roles = "Customer")]
		[HttpGet]
        public IActionResult Login()
        {
            return View();
        }
		[Authorize(Roles = "Customer")]
		[HttpPost]
        public async Task<IActionResult> Login(CustomerViewModel customer)
        {
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (existingCustomer != null && existingCustomer.Password == customer.Password)
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
                return View(customer);
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> CustomerLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Customer");
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

            return View(customerViewModel);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public IActionResult addToCart()
        {
            return View();
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult viewCart() { 
        
        
            return View();
        }
        
    }
}
