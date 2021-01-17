using System;
using System.Text;

namespace AppleCopy
{
    public static class ExtensionMethods
    {
        public static bool IsNullOrEmpty(this string someString)
        {
            return string.IsNullOrEmpty(someString);
        }

        public static string GetCondensedTimeSpanString(this TimeSpan timeSpan)
        {
            StringBuilder response = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                response.Append($"{timeSpan.Days}d ");
            }

            if (timeSpan.Hours > 0)
            {
                response.Append($"{timeSpan.Hours}h ");
            }

            if (timeSpan.Minutes > 0)
            {
                response.Append($"{timeSpan.Minutes}m ");
            }

            if (timeSpan.Seconds > 0)
            {
                response.Append($"{timeSpan.Seconds}s ");
            }

            return response.ToString();

        }
    }
}
