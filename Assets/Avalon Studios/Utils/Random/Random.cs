using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AvalonStudios.Additions.Utils
{
    public sealed class Random
    {
        /// <summary>
        /// Return a random string with a specific size.
        /// </summary>
        /// <param name="size">Size of the random string</param>
        /// <returns>Returns a random string</returns>
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder(size);

            char offset = 'A';
            int lettersOffset = 26;

            for (int i = 0; i < size; i++)
            {
                char c = (char)UnityEngine.Random.Range(offset, offset + lettersOffset);
                builder.Append(c);
            }

            return builder.ToString();
        }

        public static T CumulativeProbability<T>(List<T> elements, List<int> weight)
        {
            int random = UnityEngine.Random.Range(0, 100);
            int cumulative = 0;

            int i = 0;
            foreach (T element in elements)
            {
                cumulative += weight[i];
                if (random <= cumulative) return element;
                i++;
            }

            return default;
        }
    }
}
