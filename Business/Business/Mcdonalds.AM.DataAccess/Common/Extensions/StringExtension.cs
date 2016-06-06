using System;

namespace Mcdonalds.AM.DataAccess.Common.Extensions
{
    public static class StringExtension
    {
        public static T As<T>(this string input)
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }

        public static string AsString(this object input)
        {
            return input == null ? string.Empty : input.ToString();
        }
    }
}
