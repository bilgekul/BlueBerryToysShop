using System.ComponentModel.DataAnnotations;

namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Password { get; set; } = default!;
        public string NewPassword { get; set; } = default!;

        [Compare(nameof(NewPassword), ErrorMessage = "Entered passwords doesn't match.")]
        public string NewPasswordConfirm { get; set; } = default!;
    }
}
