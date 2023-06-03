using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueBerry.ToysShop.Web.Database_Settings
{
    public class WebDbContext: IdentityDbContext<AppUser, AppRole, string>
    {
        public WebDbContext(DbContextOptions<WebDbContext>options):base(options)
        { 

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category>? Category { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppRole> Roles { get; set; }

    }
}
