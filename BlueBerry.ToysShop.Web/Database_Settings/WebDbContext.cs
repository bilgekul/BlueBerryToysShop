using BlueBerry.ToysShop.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueBerry.ToysShop.Web.Database_Settings
{
    public class WebDbContext:DbContext
    {
        public WebDbContext(DbContextOptions<WebDbContext>options):base(options)
        { 

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<BlueBerry.ToysShop.Web.Models.Category>? Category { get; set; }
        public DbSet<Customer> Customers { get; set; }

    }
}
