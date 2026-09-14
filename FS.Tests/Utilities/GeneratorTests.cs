using System.Linq;
using Utilities;
using Xunit;

namespace FS.Tests.Utilities
{
    public class GeneratorTests
    {
        [Fact]
        public void GeneratePurchaseNumber_ReturnsNumericStringWithTimestamp()
        {
            string code = Generator.GeneratePurchaseNumber();

            Assert.False(string.IsNullOrEmpty(code));
            Assert.True(code.All(char.IsDigit));
            Assert.True(code.Length >= 17);
        }

        [Fact]
        public void GeneratePurchaseNumber_IsUniqueAcrossCalls()
        {
            var a = Generator.GeneratePurchaseNumber();
            var b = Generator.GeneratePurchaseNumber();
            Assert.NotEqual(a, b);
        }

        [Fact]
        public void GenerateSecureDigits_ReturnsExpectedRange()
        {
            var code = Generator.GenerateSecureDigits(9);
            Assert.InRange(code, 100000000, 999999999);
        }

        [Fact]
        public void GenerateSecureDigits_IsNotConstant()
        {
            var a = Generator.GenerateSecureDigits(9);
            var b = Generator.GenerateSecureDigits(9);
            Assert.NotEqual(a, b);
        }
    }
}