using System;
using Utilities.Convertors;
using Xunit;

namespace FS.Tests.Utilities
{
    public class DateConvertorTests
    {
        [Fact]
        public void ToShamsi_ConvertsNowruz2024()
        {
            var date = new DateTime(2024, 3, 20);
            Assert.Equal("1403/01/01", date.ToShamsi());
        }

        [Fact]
        public void ToShamsi_ConvertsKnownWinterDate()
        {
            var date = new DateTime(2024, 1, 1);
            Assert.Equal("1402/10/11", date.ToShamsi());
        }
    }
}