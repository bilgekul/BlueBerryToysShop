namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class TwoFactorAuthenticatorViewModel
    {
        public string SharedKey { get; set; } = default!;
        public string AuthenticationUri { get; set; } = default!;
        public string VerificationCode { get; set; } = default!;
    }
}
