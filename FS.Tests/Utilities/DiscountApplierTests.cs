using Utilities.Convertors;
using Xunit;

namespace FS.Tests.Utilities
{
    public class DiscountApplierTests
    {
        [Theory]
        [InlineData(1000, 10, 900)]
        [InlineData(50000, 25, 37500)]
        [InlineData(1000, 0, 1000)]
        [InlineData(20000, 100, 0)]
        public void Apply_ReturnsDiscountedPrice(double price, double discount, double expected)
        {
            double actual = DiscountApplier.Apply(price, discount);
            Assert.Equal(expected, actual, 2);
        }
    }
}