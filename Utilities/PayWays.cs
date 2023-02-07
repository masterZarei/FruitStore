using System.Collections.Generic;

namespace Utilities
{
    public static class PayWays
    {

        public static List<Dictionary> GetWays = new List<Dictionary>()
        {
            new Dictionary{Name ="پرداخت در محل", Value="پرداخت در محل"},
            new Dictionary{Name ="پرداخت اینترنتی", Value="پرداخت اینترنتی"},
            new Dictionary{Name ="پرداخت با کیف پول", Value="پرداخت با کیف پول"}
        };
    }
}
