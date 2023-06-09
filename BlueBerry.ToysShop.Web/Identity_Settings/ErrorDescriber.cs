using Microsoft.AspNetCore.Identity;

namespace BlueBerry.ToysShop.Web.Identity_Settings
{
        public class ErrorDescriber : IdentityErrorDescriber
        {
            public override IdentityError InvalidUserName(string userName) => new() { Code = "InvalidUserName", Description = $"\"{userName}\" kullanıcı adı geçersiz." };

            public override IdentityError DuplicateEmail(string email) => new() { Code = "DuplicateEmail", Description = $"\"{email}\" adresi kullanımdadır." };

            public override IdentityError PasswordTooShort(int length) => new() { Code = "PasswordTooShort", Description = $"Şifre en az {length} karakter olmalıdır." };

            public static IdentityError PasswordContainsUsername() => new() { Code = "PasswordContainsUsername", Description = "Parola kullanıcı adı içeremez." };

            public static IdentityError UserNameLength() => new() { Code = "UserNameLength", Description = "Kullanıcı adı en az 6 karakter olmalıdır." };

            public static IdentityError UserNameContainsEmail() => new() { Code = "UserNameContainsEmail", Description = "Kullanıcı adı e-posta adı içeremez." };
        }
}
