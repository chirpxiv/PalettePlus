using System.Linq;

namespace PalettePlus.Extensions
{
    internal static class StringExtensions
    {
        internal static string CapitalizeEach(this string str)
        {
            return string.Join(' ', str.Split(' ').Select(word => word.Length > 0 ? char.ToUpper(word[0]) + word[1..] : word));
        }

        internal static string FullTrim(this string str)
        {
            return string.Join(' ', str.Split(' ').Where(word => word.Length > 0)).Trim();
        }
    }
}