using FS.Models.Models;
using System.Collections.Generic;

namespace Utilities
{
    public static class IconStore
    {
        public static List<Icons> GetIcons = new List<Icons>()
        {
            new Icons{Name ="تلگرام", Value="fab fa-telegram text-primary"},
            new Icons{Name ="اینستاگرام", Value="fab fa-instagram text-danger"},
            new Icons{Name ="فیسبوک", Value="fab fa-facebook text-info"},
            new Icons{Name ="واتس اپ", Value="fab fa-whatsapp text-success"},
            new Icons{Name ="یوتوب", Value="fab fa-youtube text-danger"},
            new Icons{Name ="توئیتر", Value="fab fa-twitter text-info"},
            new Icons{Name ="تیک تاک", Value="fab fa-tiktok text-dark"},
            new Icons{Name ="لینکدین", Value="fab fa-linkedin text-primary"},
            new Icons{Name ="آدرس فیزیکی", Value="fa fa-map-marker text-white"},
            new Icons{Name ="شماره تلفن ثابت", Value="fa fa-phone text-primary"},
            new Icons{Name ="شماره تلفن همراه", Value="fa fa-mobile text-primary"}
        };
    }
}
