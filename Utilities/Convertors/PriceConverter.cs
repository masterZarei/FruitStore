namespace Utilities.Convertors
{
    public static class PriceConverter
    {
        public static string ToToman(this int value)
        {
            return value.ToString("#,0 تومان");
        }
        public static string ToToman(this double value)
        {
            return value.ToString("#,0 تومان");
        }

    }
}
