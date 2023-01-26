using FS.Models.Models;
using System.Collections.Generic;

namespace Utilities
{
    public static class IconStore
    {
        public static List<Dictionary> GetIcons = new List<Dictionary>()
        {
            new Dictionary{Name ="تلگرام", Value="fab fa-telegram text-primary"},
            new Dictionary{Name ="اینستاگرام", Value="fab fa-instagram text-danger"},
            new Dictionary{Name ="فیسبوک", Value="fab fa-facebook text-info"},
            new Dictionary{Name ="واتس اپ", Value="fab fa-whatsapp text-success"},
            new Dictionary{Name ="یوتوب", Value="fab fa-youtube text-danger"},
            new Dictionary{Name ="توئیتر", Value="fab fa-twitter text-info"},
            new Dictionary{Name ="تیک تاک", Value="fab fa-tiktok text-dark"},
            new Dictionary{Name ="لینکدین", Value="fab fa-linkedin text-primary"},
            new Dictionary{Name ="آدرس فیزیکی", Value="fa fa-map-marker text-white"},
            new Dictionary{Name ="شماره تلفن ثابت", Value="fa fa-phone text-primary"},
            new Dictionary{Name ="شماره تلفن همراه", Value="fa fa-mobile text-primary"}
        };
    }
}
