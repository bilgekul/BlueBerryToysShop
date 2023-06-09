﻿using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Helpers;
using BlueBerry.ToysShop.Web.Identity_Settings;
using BlueBerry.ToysShop.Web.Identity_Settings.Requirements;
using BlueBerry.ToysShop.Web.Identity_Settings.Validators;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.Models.Emails;
using BlueBerry.ToysShop.Web.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn"));
});
// Mapleme olaylarını kontrol etme
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Dosya ve Resim işlemleri için Singleton nesnesi oluşturma
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<TwoFactorAuthenticationService>();
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.SignIn.RequireConfirmedEmail = true;

}).AddUserValidator<UserValidator>().AddPasswordValidator<PasswordValidator>()
.AddErrorDescriber<ErrorDescriber>().AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


var serviceProvider = builder.Services.BuildServiceProvider();
var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
// Customer rolünü oluşturma işlemleri
if (!await roleManager.RoleExistsAsync("Customer"))
{
    var role = new AppRole
    {
        Name = "Customer"
    };
    await roleManager.CreateAsync(role);
}
// Admin rolünü oluşturma işlemleri
if (!await roleManager.RoleExistsAsync("Admin"))
{
    var role = new AppRole
    {
        Name = "Admin"
    };
    await roleManager.CreateAsync(role);
}
builder.Services.ConfigureApplicationCookie(options =>
{
    // User cookie yapılandırması
    options.LoginPath = new PathString("/User/Login");
    options.LogoutPath = new PathString("/User/Logout");
    options.AccessDeniedPath = new PathString("/Home/AccessDenied");

    options.Cookie = new()
    {
        Name = "IdentityCookie",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.Always
    };
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);

    // Customer cookie yapılandırması
    options.LoginPath = new PathString("/Customer/CustomerLogin");
    options.LogoutPath = new PathString("/Customer/CustomerLogout");
    options.AccessDeniedPath = new PathString("/Home/AccessDenied");

    options.Cookie = new()
    {
        Name = "IdentityCookie",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.Always
    };
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services.AddAuthentication()
.AddFacebook(options =>
{
    options.AppId = builder.Configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppId");
    options.AppSecret = builder.Configuration.GetValue<string>("ExternalLoginProviders:Facebook:AppSecret");
    //options.CallbackPath = new PathString("/User/FacebookCallback");
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("ExternalLoginProviders:Google:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("ExternalLoginProviders:Google:ClientSecret");
})
.AddMicrosoftAccount(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("ExternalLoginProviders:Microsoft:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("ExternalLoginProviders:Microsoft:ClientSecret");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HrDepartmentPolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetName(Department.HR)!);
    });

    options.AddPolicy("SalesDepartmentPolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetName(Department.Sales)!);
    });

    options.AddPolicy("EmployeePolicy", policy =>
    {
        policy.RequireClaim("Department", Enum.GetNames<Department>());
    });

    options.AddPolicy("AtLeast18Policy", policy =>
    {
        policy.AddRequirements(new MinimumAgeRequirement(18));
    });
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("CustomerOnly", policy =>
    {
        policy.RequireRole("Customer");
    });
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default Conventional Router
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
