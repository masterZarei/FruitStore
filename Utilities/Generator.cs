using System;

namespace Utilities
{
    public static class Generator
    {
        public static string RandomNumber()
        {
            string Code = new Random().Next(0,255).ToString("D5");
            return Code;
        }


    }
}
