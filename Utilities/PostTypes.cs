using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class PostTypes
    {
        public static List<Dictionary> GetTypes = new List<Dictionary>()
        {
            new Dictionary{Name ="پست معمولی (12000)", Value="Normal_Transfer"},
            new Dictionary{Name ="پست پیشتاز (18000)", Value="Fast_Transfer"}
        };
    }
}
