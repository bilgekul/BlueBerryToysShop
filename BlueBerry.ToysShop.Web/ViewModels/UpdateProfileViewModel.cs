﻿using BlueBerry.ToysShop.Web.Models;

namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class UpdateProfileViewModel
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public Gender Gender { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
