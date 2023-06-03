using BlueBerry.ToysShop.Web.Models;

namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class TwoFactorLoginViewModel
    {
        public string VerificationCode { get; set; } = default!;
        public bool IsRecoveryCode { get; set; }
        public TwoFactorType TwoFactorType { get; set; }
    }
}
