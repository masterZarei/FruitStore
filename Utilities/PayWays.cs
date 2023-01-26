using System.Collections.Generic;

namespace Utilities
{
    public static class PayWays
    {

        public static List<Dictionary> GetWays = new List<Dictionary>()
        {
            new Dictionary{Name ="پرداخت در محل", Value="OnTheSpot_Payment"},
            new Dictionary{Name ="پرداخت اینترنتی", Value="Internet_Payment"},
            new Dictionary{Name ="پرداخت با کیف پول", Value="Wallet_Payment"}
        };
    }
}
