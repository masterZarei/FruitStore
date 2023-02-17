using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public static class ImageFormats
    {
        public static List<Dictionary> Formats = new List<Dictionary>()
        {
            new Dictionary{Name =".jpg", Value=".jpg"},
            new Dictionary{Name =".jpeg", Value=".jpeg"},
            new Dictionary{Name =".png", Value=".png"},
            new Dictionary{Name =".webp", Value=".webp"},
            new Dictionary{Name =".gif", Value=".gif"},
        };
        public static Dictionary CheckFormats(string input)
        {
            var check = Formats.FirstOrDefault(a => a.Value == input);
            return check;
        }
    }
}
