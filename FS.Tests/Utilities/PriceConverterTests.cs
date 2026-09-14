using System.Globalization;
using Utilities.Convertors;
using Xunit;

namespace FS.Tests.Utilities
{
    public class PriceConverterTests
    {
        private static void UseInvariantCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        [Fact]
        public void ToToman_Double_UsesThousandSeparator()
        {
            UseInvariantCulture();
            double price = 12345;
            Assert.Equal("12,345 تومان", price.ToToman());
        }

        [Fact]
        public void ToToman_Int_UsesThousandSeparator()
        {
            UseInvariantCulture();
            int price = 1000000;
            Assert.Equal("1,000,000 تومان", price.ToToman());
        }

        [Fact]
        public void ToToman_Decimal_UsesThousandSeparator()
        {
            UseInvariantCulture();
            decimal price = 5000m;
            Assert.Equal("5,000 تومان", price.ToToman());
        }
    }
}