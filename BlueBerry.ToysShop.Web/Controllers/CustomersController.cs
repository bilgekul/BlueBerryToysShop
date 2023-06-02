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
using Microsoft.AspNetCore.Identity;

namespace BlueBerry.ToysShop.Web.Controllers
{
	[Authorize(Roles = "Customer")]
	public class CustomersController:Controller
	{

		private readonly WebDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CustomersController(WebDbContext context, IMapper mapper, UserManager<Customer> userManager, SignInManager<Customer> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> SignUp(CustomerViewModel customer, [FromForm] string First, [FromForm] string Last)
        {
            customer.FullName = First + "-" + Last;
            if (ModelState.IsValid)
            {   
                var _customer = _mapper.Map<Customer>(customer);
                _context.Customers.Add(_customer);
                _context.SaveChanges();
                var existingCustomer = await _userManager.FindByEmailAsync(customer.Email);

                if (existingCustomer == null)
                {
                    var newCustomer = _mapper.Map<Customer>(customer);
                    newCustomer.Email = customer.Email;

                    var result = await _userManager.CreateAsync(newCustomer, customer.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newCustomer, "Customer");
                        
                        await _signInManager.SignInAsync(newCustomer, isPersistent: false);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "A customer with the provided email already exists.");
                }
            }

            return View(customer);
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
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(customer.Email, customer.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            return View(customer);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> CustomerLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
 
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public IActionResult CustomerProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);

            if (customer != null)
            {
                var customerViewModel = _mapper.Map<CustomerViewModel>(customer);
                return View(customerViewModel);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult UpdateProfile(CustomerViewModel model, [FromForm] string First, [FromForm] string Last)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);

                if (customer != null)
                {
                    customer.FullName = First+"-"+Last;
                    _context.SaveChanges();
                }

                return RedirectToAction("CustomerProfile");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);

                if (customer != null)
                {
                    var result = _userManager.ChangePasswordAsync(customer, model.Password, model.Password).Result;

                    if (result.Succeeded)
                    {
                        return RedirectToAction("CustomerProfile");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(model);
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
