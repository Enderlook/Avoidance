using UnityEngine;

namespace AvalonStudios.Additions.Extensions
{
    public static class GeneralExtensions
    {
        public static bool IsNull<T>(this T v) =>
            v == null ? true : false;
    }
}
