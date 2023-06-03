using Microsoft.AspNetCore.Identity;

namespace BlueBerry.ToysShop.Web.Models.Identity
{
    public class AppRole : IdentityRole
    {
        public DateTime CreatedOn { get; set; }
    }
}
