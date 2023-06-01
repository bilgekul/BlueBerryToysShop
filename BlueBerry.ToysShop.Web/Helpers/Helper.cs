using BlueBerry.ToysShop.Web.Database_Settings;
using Microsoft.EntityFrameworkCore;

namespace MyAspNetCore.Web.Helpers
{
    public class Helper : IHelper
    {
        public bool ValidCredentials(string email, string password, WebDbContext context)
        {
            var user = context.Customers.FirstOrDefault(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.Password))
            {
                return true;
            }

            return false;
        }

        public bool VerifyPassword(string password, string savedPassword)
        {
            bool passwordsMatch = (savedPassword == password);

            return passwordsMatch;
        }
    }
}
