using System;

namespace Utilities
{
    public static class Generator
    {
        public static string RandomNumber(int first, int second)
        {
            string Code = new Random().Next(first, second).ToString();
            return Code;
        }
    }
}
