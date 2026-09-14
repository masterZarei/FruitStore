using System;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
    public static class Generator
    {
        public static string RandomNumber(int first, int second)
        {
            string Code = new Random().Next(first, second).ToString();
            return Code;
        }

        /// <summary>
        /// Generates a cryptographically secure numeric code with the given length.
        /// The first digit is always 1-9 so the integer never has leading zeros.
        /// </summary>
        public static int GenerateSecureDigits(int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var sb = new StringBuilder(length);
            sb.Append((char)('1' + bytes[0] % 9));
            for (int i = 1; i < length; i++)
            {
                sb.Append((char)('0' + bytes[i] % 10));
            }
            return int.Parse(sb.ToString());
        }

        /// <summary>
        /// Generates a unique, human-readable numeric purchase number string
        /// (timestamp + random suffix). No schema change required.
        /// </summary>
        public static string GeneratePurchaseNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmssfff") + GenerateSecureDigits(3).ToString("D3");
        }
    }
}
