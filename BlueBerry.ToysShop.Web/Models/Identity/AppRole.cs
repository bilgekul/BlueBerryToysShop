using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBerry.ToysShop.Web.Models.Identity
{
    public class AppRole : IdentityRole
    {
        public DateTime CreatedOn { get; set; }
    }
}
