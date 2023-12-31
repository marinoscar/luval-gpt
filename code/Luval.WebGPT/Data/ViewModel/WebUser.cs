﻿using Luval.GPT.Data.Entities;

namespace Luval.WebGPT.Data.ViewModel
{
    public class WebUser
    {
        public string? ProviderName { get; set; }
        public string? ProviderKey { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? PictureUrl { get; set; }

        public AppUser ToAppUser()
        {
            return new AppUser { ProviderName = ProviderName, ProviderKey = ProviderKey, Email = Email, ProfilePicUrl = PictureUrl };
        }
    }
}
