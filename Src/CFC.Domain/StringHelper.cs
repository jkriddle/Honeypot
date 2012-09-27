using System;
using System.Text;

namespace CFC.Domain
{
    public class RandomHelper
    {
        public static string GenerateRandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static int GenerateRandomInt(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);

            return random.Next(10000, 99999);
        }

    }
}
