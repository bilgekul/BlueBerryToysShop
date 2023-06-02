using BlueBerry.ToysShop.Web.Database_Settings;
using BlueBerry.ToysShop.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MyAspNetCore.Web.Helpers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<WebDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
});
// Mapleme olaylarını kontrol etme
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Dosya ve Resim işlemleri için Singleton nesnesi oluşturma
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
// Yardımcı Sınıf işlemleri için Singleton nesnesi oluşturma
builder.Services.AddSingleton<IHelper, Helper>();
// Authentication işlemleri
builder.Services.AddAuthentication("AdminAuthentication")
     .AddCookie("AdminAuthentication", options =>
     {
         options.LoginPath = "/Admins/Login"; // Admin girişi için kullanılacak sayfa yolu
		 options.AccessDeniedPath = "/Admins/AccessDenied"; // Yetkisiz erişim durumunda yönlendirilecek sayfa yolu
	 });
builder.Services.AddAuthentication("CustomerAuthentication")
	.AddCookie("CustomerAuthentication", options =>
	{
		options.LoginPath = "/Customers/Login"; // Customer girişi için kullanılacak sayfa yolu
		options.AccessDeniedPath = "/Customers/AccessDenied"; // Yetkisiz erişim durumunda yönlendirilecek sayfa yolu
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
