using BlueBerry.ToysShop.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace BlueBerry.ToysShop.Web.Identity_Settings.Validators
{
        public class PasswordValidator : IPasswordValidator<AppUser>
        {
            public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
            {
                var errors = new List<IdentityError>();

                if (user.UserName == password)
                {
                    errors.Add(ErrorDescriber.PasswordContainsUsername());
                }

                if (errors.Any())
                {
                    return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
                }
                return Task.FromResult(IdentityResult.Success);
            }
        }
    
}
