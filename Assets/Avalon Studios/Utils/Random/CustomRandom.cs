using System.Text;
using UnityEngine;

namespace AvalonStudios.Additions.Utils.CustomRandom
{
    public sealed class CustomRandom
    {
        /// <summary>
        /// Return a random string with a specific size.
        /// </summary>
        /// <param name="size">Size of the random string</param>
        /// <returns><seealso cref="string"/></returns>
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder(size);

            char offset = 'A';
            int lettersOffset = 26;

            for (int i = 0; i < size; i++)
            {
                char c = (char)Random.Range(offset, offset + lettersOffset);
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
