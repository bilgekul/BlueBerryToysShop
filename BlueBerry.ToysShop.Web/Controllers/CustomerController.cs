using AutoMapper;
using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Helpers;
using BlueBerry.ToysShop.Web.Identity_Settings;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.Models.Identity;
using BlueBerry.ToysShop.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlueBerry.ToysShop.Web.Controllers
{
    public class CustomerController:Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailHelper _emailHelper;
        private readonly TwoFactorAuthenticationService _twoFactorAuthService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public CustomerController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TwoFactorAuthenticationService twoFactorAuthService, EmailHelper emailHelper, IMapper mapper, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailHelper = emailHelper;
            _twoFactorAuthService = twoFactorAuthService;
            _mapper = mapper;
            _context = context;
        }
        [HttpPost]
        public IActionResult UpdateCartItem(int cartItemId, int quantity)
        {
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                    if (cartItem != null)
                    {
                        if (quantity > 0)
                        {
                            cartItem.Quantity = quantity;
                            _context.SaveChanges();
                            TempData["status"] = "Ürün miktarı güncellendi.";
                        }
                        else
                        {
                            _context.CartItems.Remove(cartItem);
                            _context.SaveChanges();
                            TempData["status"] = "Ürün sepetinizden kaldırıldı.";
                        }
                        return RedirectToAction("ViewCart");
                    }
                }
            }

            return Json(new { IsSuccess = false, Message = "Ürün miktarını güncellemek için giriş yapmanız gerekmektedir." });
        }

        public IActionResult ViewCart()
        {
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserName == username);

                    if (cart != null)
                    {
                        var cartItems = _context.CartItems.Include(ci => ci.Product).Where(ci => ci.CartId == cart.Id).ToList();
                        return View(cartItems);
                    }
                }
            }

            return View(new List<CartItem>());
        }

        [HttpGet]
        public IActionResult Cart()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserName == username);

                    if (cart != null)
                    {
                        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

                        if (existingCartItem != null)
                        {
                            existingCartItem.Quantity += quantity;
                        }
                        else
                        {
                            var newCartItem = new CartItem
                            {
                                CartId = cart.Id,
                                ProductId = productId,
                                Quantity = quantity
                            };

                            cart.CartItems.Add(newCartItem);
                        }
                    }
                    else
                    {
                        var newCart = new Cart
                        {
                            UserName = username
                        };

                        _context.Carts.Add(newCart);

                        var newCartItem = new CartItem
                        {
                            CartId = newCart.Id,
                            ProductId = productId,
                            Quantity = quantity
                        };

                        newCart.CartItems.Add(newCartItem);
                    }

                    _context.SaveChanges();

                    var cartItems = _context.CartItems
                        .Include(ci => ci.Product)
                        .Where(ci => ci.Cart.UserName == username)
                        .ToList();

                    return View("Cart", cartItems);
                }
            }

            return Json(new { IsSuccess = false, Message = "Ürünü sepete eklemek için giriş yapmanız gerekmektedir." });
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId && ci.Cart.UserName == username);

                    if (cartItem != null)
                    {
                        if (cartItem.Quantity > 1)
                        {
                            cartItem.Quantity--;
                        }
                        else
                        {
                            _context.CartItems.Remove(cartItem);
                        }

                        _context.SaveChanges();
                        TempData["status"] = "Ürün sepetinizden kaldırıldı.";

                        var cartItems = _context.CartItems
                            .Include(ci => ci.Product)
                            .Where(ci => ci.Cart.UserName == username)
                            .ToList();

                        return View("Cart", cartItems);
                    }
                }
            }

            return Json(new { IsSuccess = false, Message = "Ürünü sepetinizden kaldırmak için giriş yapmanız gerekmektedir." });
        }

        [HttpGet]
        public IActionResult CustomerList()
        {
            var username = User.Identity.Name; 
            var productList = _context.ProductLists
                .Include(pl => pl.ListItems)
                .ThenInclude(li => li.Product)
                .FirstOrDefault(pl => pl.UserName == username);
            return View(productList);
        }
		public IActionResult RemoveFromList(int productId)
		{
			if (User.IsInRole("Customer"))
			{
				var username = User.Identity.Name;
				var user = _context.Users.FirstOrDefault(u => u.UserName == username);
				if (user != null)
				{
					var productList = _context.ProductLists.FirstOrDefault(pl => pl.UserName == username);
					if (productList != null)
					{
						var listItem = _context.ListItems.FirstOrDefault(li => li.ProductId == productId && li.ListId == productList.Id);
						if (listItem != null)
						{
							_context.ListItems.Remove(listItem);
							_context.SaveChanges();
							TempData["status"] = "Ürün listenizden kaldırıldı.";
							return RedirectToAction("Ürün", "Detaylar", new { id = productId });
						}
					}
				}
			}
			return Json(new { IsSuccess = false, Message = "Ürünü listenizden kaldırmak için giriş yapmanız gerekmektedir." });
		}


		[HttpPost]
        public IActionResult AddToList(int productId)
        {
            if (User.IsInRole("Customer"))
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    var productList = _context.ProductLists.FirstOrDefault(pl => pl.UserName == username);
                    if (productList != null)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            var listItem = new ListItem
                            {
                                ProductId = product.Id,
                                ListId = productList.Id
                            };

                            _context.ListItems.Add(listItem);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        var newList = new ProductList
                        {
                            UserName = username
                        };

                        _context.ProductLists.Add(newList);
                        _context.SaveChanges();

                        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            var listItem = new ListItem
                            {
                                ProductId = product.Id,
                                ListId = newList.Id
                            };

                            _context.ListItems.Add(listItem);
                            _context.SaveChanges();
                        }
                    }
                }
                TempData["status"] = "Ürün listeye eklendi.";
                return RedirectToAction("Ürün", "Detaylar", new { id = productId });
            }
            return Json(new { IsSuccess = false, Message = "Ürünü listenize eklemek için giriş yapmanız gerekmektedir." });
        }




        [Authorize(Policy = "CustomerOnly")]
        [HttpPost]
        public async Task<IActionResult> SaveVisitorComment(string name, string comment, int productId, double productRating)
        {
            if (User.IsInRole("Customer"))
            {
                var user = await _userManager.GetUserAsync(User);
                productRating = Math.Max(0, Math.Min(5, productRating));
                var visitor = new Visitor
                {
                    Name = name,
                    Comment = comment,
                    Created = DateTime.Now,
                    UserId = user.Id,
                    ProductId = productId,
                    ProductRating = productRating
                };

                _context.Visitors.Add(visitor);
                _context.SaveChanges();
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    var visitorRatings = _context.Visitors.Where(v => v.ProductId == productId).Select(v => v.ProductRating).ToList();
                    double averageRating = visitorRatings.Any() ?(int)visitorRatings.Average() : 0;


                    product.Rating = averageRating;
                    _context.Products.Update(product);
                    _context.SaveChanges();
                }

                return Json(new { IsSuccess = true });
            }
            return Json(new { IsSuccess = false, Message = "Yorum eklemek için giriş yapmanız gerekmektedir." });
        }
        [HttpGet]
        public IActionResult VisitorCommentList(int productId)
        {
            var visitors = _context.Visitors
                .Where(v => v.ProductId == productId)
                .OrderByDescending(x => x.Created)
                .ToList();

            var visitorViewModels = _mapper.Map<List<VisitorViewModel>>(visitors);

            return Json(visitorViewModels);
        }
        [HttpGet]
        public IActionResult GetProductRating(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                return Json(new { averageRating = product.Rating });
            }

            return Json(new { averageRating = 0 });
        }
        public IActionResult CustomerIndex()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public IActionResult CustomerRegister() => View();

        [HttpPost]
        public async Task<IActionResult> CustomerRegister(SignUpViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    Gender = viewModel.Gender,
                    BirthDay = viewModel.BirthDay,
                    TwoFactorType = Models.TwoFactorType.None,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("CustomerConfirmEmail", "Customer", new
                    {
                        userId = user.Id,
                        token = confirmationToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Confirm e-mail",
                        Body = $"Please <a href='{confirmationLink}'>click</a> to confirm your e-mail address.",
                        To = user.Email
                    });
                    return RedirectToAction("CustomerLogin");
                }
                result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
            }
            return View(viewModel);
        }
        public IActionResult CustomerLogin(string? returnUrl)
        {
            if (returnUrl != null)
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerLogin(SignInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        var returnUrl = TempData["ReturnUrl"];
                        if (returnUrl != null)
                        {
                            return Redirect(returnUrl.ToString() ?? "/");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction("TwoFactorLogin", new { ReturnUrl = TempData["ReturnUrl"] });
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutEndUtc = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutEndUtc.Value - DateTime.UtcNow;
                        ModelState.AddModelError(string.Empty, $"Bu hesap kilitlendi, {timeLeft.Minutes} dakika sonra tekrar deneyin..");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "E-posta adresinizi onaylamanız gerekiyor.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> CustomerTwoFactorLogin(string? returnUrl)
        {
            if (returnUrl != null)
            {
                TempData["ReturnUrl"] = returnUrl;
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            return View(new TwoFactorLoginViewModel
            {
                TwoFactorType = user.TwoFactorType,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CustomerTwoFactorLogin(TwoFactorLoginViewModel vieWModel)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user.TwoFactorType == Models.TwoFactorType.Authenticator)
            {
                var result = vieWModel.IsRecoveryCode ? await _signInManager.TwoFactorRecoveryCodeSignInAsync(vieWModel.VerificationCode) : await _signInManager.TwoFactorAuthenticatorSignInAsync(vieWModel.VerificationCode, true, false);
                if (result.Succeeded)
                {
                    return Redirect(TempData["ReturnUrl"]?.ToString() ?? "/");
                }
                ModelState.AddModelError(string.Empty, "Doğrulama kodu geçersiz.");
            }
            else if (user.TwoFactorType == Models.TwoFactorType.Email || user.TwoFactorType == Models.TwoFactorType.Sms)
            {
                // Handle verificationCode control flow

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, true);

                return Redirect(TempData["ReturnUrl"]?.ToString() ?? "/");
            }

            return View(vieWModel);
        }

        public async Task CustomerLogout() => await _signInManager.SignOutAsync();

        public async Task<IActionResult> CustomerProfile()
        {
            var me = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (me == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(me.Adapt<UpdateProfileViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> CustomerProfile(UpdateProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var me = await _userManager.FindByNameAsync(User.Identity?.Name);
                if (me != null)
                {
                    if (me.PhoneNumber != viewModel.PhoneNumber && _userManager.Users.Any(a => a.PhoneNumber == viewModel.PhoneNumber))
                    {
                        ModelState.AddModelError(string.Empty, "Telefon numarası zaten kullanılıyor.");
                    }
                    else
                    {
                        me.UserName = viewModel.UserName;
                        me.Email = viewModel.Email;
                        me.PhoneNumber = viewModel.PhoneNumber;
                        me.Gender = viewModel.Gender;
                        me.BirthDay = viewModel.BirthDay;

                        var result = await _userManager.UpdateAsync(me);
                        if (result.Succeeded)
                        {
                            await _userManager.UpdateSecurityStampAsync(me);
                            await _signInManager.SignOutAsync();
                            await _signInManager.SignInAsync(me, true);

                            return RedirectToAction("CustomerIndex", "Customer");
                        }
                        result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                    }
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(viewModel);
        }

        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> CustomerChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var me = await _userManager.FindByNameAsync(User.Identity?.Name);

                var passwordValid = await _userManager.CheckPasswordAsync(me, viewModel.Password);
                if (passwordValid)
                {
                    var result = await _userManager.ChangePasswordAsync(me, viewModel.Password, viewModel.NewPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(me);

                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(me, true);

                        return RedirectToAction("CustomerIndex", "Customer");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Şifre geçersiz.");
                }
            }

            return View();
        }

        public IActionResult CustomerForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordLink = Url.Action("CustomerResetPassword", "Customer", new
                    {
                        userId = user.Id,
                        token = passwordResetToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Reset password",
                        Body = $"Please <a href='{passwordLink}'>click</a> to reset your password.",
                        To = user.Email
                    });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                }
            }
            return View(viewModel);
        }

        public IActionResult CustomerResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }

            return View(new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> CustomerResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.UserId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        return RedirectToAction("CustomerLogin", "Customer");
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> CustomerConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction("CustomerLogin");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CustomerTwoFactorType()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            return View(new TwoFactorTypeViewModel
            {
                TwoFactorType = user.TwoFactorType
            });
        }

        [HttpPost]
        public async Task<IActionResult> CustomerTwoFactorType(TwoFactorTypeViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            user.TwoFactorType = viewModel.TwoFactorType;
            await _userManager.UpdateAsync(user);
            await _userManager.SetTwoFactorEnabledAsync(user, user.TwoFactorType != Models.TwoFactorType.None);

            if (viewModel.TwoFactorType == Models.TwoFactorType.Authenticator)
            {
                return RedirectToAction("CustomerTwoFactorAuthenticator", "Customer");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CustomerTwoFactorAuthenticator()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (user.TwoFactorEnabled && user.TwoFactorType == Models.TwoFactorType.Authenticator)
            {
                var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (authenticatorKey == null)
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                return View(new TwoFactorAuthenticatorViewModel
                {
                    SharedKey = authenticatorKey,
                    AuthenticationUri = _twoFactorAuthService.GenerateQrCodeUri(user.Email, authenticatorKey)
                });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CustomerTwoFactorAuthenticator(TwoFactorAuthenticatorViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            var verificationCode = viewModel.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (isTokenValid)
            {
                TempData["RecoveryCodes"] = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CustomerFacebookLogin(string returnUrl)
        {
            var redirectUrl = Url.Action("CustomerExternalResponse", "Customer", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }

        public IActionResult CustomerGoogleLogin(string returnUrl)
        {
            var redirectUrl = Url.Action("CustomerExternalResponse", "Customer", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public IActionResult CustomerMicrosoftLogin(string returnUrl)
        {
            var redirectUrl = Url.Action("CustomerExternalResponse", "Customer", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
            return new ChallengeResult("Microsoft", properties);
        }

        public async Task<IActionResult> CustomerExternalResponse(string ReturnUrl = "/")
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("CustomerLogin");
            }

            var externalLoginResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
            if (externalLoginResult.Succeeded)
            {
                return Redirect(ReturnUrl);
            }

            var externalUserId = loginInfo.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var externalEmail = loginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value;

            var existUser = await _userManager.FindByEmailAsync(externalEmail);
            if (existUser == null)
            {
                var user = new AppUser { Email = externalEmail };

                if (loginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    var userName = loginInfo.Principal.FindFirst(ClaimTypes.Name)?.Value;
                    if (userName != null)
                    {
                        userName = userName.Replace(' ', '-').ToLower() + externalUserId?.Substring(0, 5);
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = user.Email;
                    }
                }
                else
                {
                    user.UserName = user.Email;
                }

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
                    if (loginResult.Succeeded)
                    {
                        // await SignInManager.SignInAsync(user, true);
                        await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        loginResult.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                    }
                }
                else
                {
                    createResult.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
            }
            else
            {
                var loginResult = await _userManager.AddLoginAsync(existUser, loginInfo);
                if (loginResult.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, true);
                    await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                    return Redirect(ReturnUrl);
                }
                else
                {
                    loginResult.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
            }

            var errors = ModelState.Values.SelectMany(s => s.Errors).Select(s => s.ErrorMessage).ToList();

            return View("Error", errors);
        }


    }
}
