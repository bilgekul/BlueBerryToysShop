using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MyAspNetCore.Web.Helpers;

namespace BlueBerry.ToysShop.Web.Controllers
{
	public class CustomersController:Controller
	{

		private readonly WebDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        public CustomersController(WebDbContext context, IMapper mapper, IHelper helper)
        {
            _context = context;
            _mapper = mapper;
            _helper = helper;
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
        public IActionResult Login(CustomerViewModel customer) 
        {
            var _customer = _mapper.Map<Customer>(customer);
            if (_helper.ValidCredentials(_customer.Email, _customer.Password,_context))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }
        }
        [HttpGet]
        public IActionResult viewProductDetails()
        {
            return View();
        }

        [HttpPost]
        public IActionResult searchProduct()
        {
            return View();
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
