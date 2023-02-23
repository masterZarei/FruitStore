namespace Utilities.Convertors
{
    public static class DiscountApplier
    {
        public static double Apply(double price, double discount)
        {
            return price - price * discount / 100;
        }
    }
}
