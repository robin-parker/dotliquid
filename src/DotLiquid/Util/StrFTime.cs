using System;
using System.Collections.Generic;
using System.Globalization;

namespace DotLiquid.Util
{
    public static class StrFTime
    {
        public delegate string DateTimeDelegate(DateTime dateTime);

        private static readonly List<String> FORMAT_FLAGS = new List<String>() {":"};

        private static readonly Dictionary<string, DateTimeDelegate> Formats = new Dictionary<string, DateTimeDelegate>
        {
            { "a", (dateTime) => dateTime.ToString("ddd", CultureInfo.CurrentCulture) },
            { "A", (dateTime) => dateTime.ToString("dddd", CultureInfo.CurrentCulture) },
            { "b", (dateTime) => dateTime.ToString("MMM", CultureInfo.CurrentCulture) },
            { "B", (dateTime) => dateTime.ToString("MMMM", CultureInfo.CurrentCulture) },
            { "c", (dateTime) => dateTime.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.CurrentCulture) },
            { "C", (dateTime) => ((int)Math.Floor(Convert.ToDouble(dateTime.ToString("yyyy"))/100)).ToString() },
            { "d", (dateTime) => dateTime.ToString("dd", CultureInfo.CurrentCulture) },
            { "D", (dateTime) => dateTime.ToString("MM/dd/yy", CultureInfo.CurrentCulture) },
            { "e", (dateTime) => dateTime.ToString("%d", CultureInfo.CurrentCulture).PadLeft(2, ' ') },
            // E - not specified
            // f - not specified
            { "F", (dateTime) => dateTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) },
            // g - not implemented
            // G - not implemented
            { "h", (dateTime) => dateTime.ToString("MMM", CultureInfo.CurrentCulture) },
            { "H", (dateTime) => dateTime.ToString("HH", CultureInfo.CurrentCulture) },
            // i - not specified
            { "I", (dateTime) => dateTime.ToString("hh", CultureInfo.CurrentCulture) },
            { "j", (dateTime) => dateTime.DayOfYear.ToString().PadLeft(3, '0') },
            // J - not specified
            { "k", (dateTime) => dateTime.ToString("%H", CultureInfo.CurrentCulture) },
            // K - not specified
            { "l", (dateTime) => dateTime.ToString("%h", CultureInfo.CurrentCulture) },
            { "L", (dateTime) => dateTime.ToString("FFF", CultureInfo.CurrentCulture) },
            { "m", (dateTime) => dateTime.ToString("MM", CultureInfo.CurrentCulture) },
            { "M", (dateTime) => dateTime.Minute.ToString().PadLeft(2, '0') },
            { "n", (dateTime) => "\n" },
            // N - not implemented
            // o - not specified
            // O - not specified
            { "p", (dateTime) => dateTime.ToString("tt", CultureInfo.CurrentCulture).ToUpper() },
            { "P", (dateTime) => dateTime.ToString("tt", CultureInfo.CurrentCulture).ToLower() },
            // q - not specified
            { "Q", (dateTime) => ((long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString() },
            { "r", (dateTime) => dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture).ToUpper() },
            { "R", (dateTime) => dateTime.ToString("HH:mm", CultureInfo.CurrentCulture) },
            { "s", (dateTime) => ((int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds).ToString() },
            { "S", (dateTime) => dateTime.ToString("ss", CultureInfo.CurrentCulture) },
            { "t", (dateTime) => "\t" },
            { "T", (dateTime) => dateTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture) },
            { "u", (dateTime) => ((int)(dateTime.DayOfWeek) == 0 ? ((int)(dateTime).DayOfWeek) + 7 : ((int)(dateTime).DayOfWeek)).ToString() },
            { "U", (dateTime) => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Sunday).ToString().PadLeft(2, '0') },
            { "v", (dateTime) => dateTime.ToString("%d-MMM-yyyy", CultureInfo.CurrentCulture).ToUpper().PadLeft(11, ' ') },
            // V - not implemented 
            { "w", (dateTime) => ((int) dateTime.DayOfWeek).ToString() },
            { "W", (dateTime) => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday).ToString().PadLeft(2, '0') },
            { "x", (dateTime) => dateTime.ToString("d", CultureInfo.CurrentCulture) },
            { "X", (dateTime) => dateTime.ToString("T", CultureInfo.CurrentCulture) },
            { "y", (dateTime) => dateTime.ToString("yy", CultureInfo.CurrentCulture) },
            { "Y", (dateTime) => dateTime.ToString("yyyy", CultureInfo.CurrentCulture) },
            { "z", (dateTime) => dateTime.ToString("%K").Replace(":", string.Empty) },
            { ":z", (dateTime) => dateTime.ToString("%K") },
            { "Z", (dateTime) => dateTime.ToString("zzz", CultureInfo.CurrentCulture) },
            { "%", (dateTime) => "%" } // A % sign
        };

        /// <summary>
        /// Applies formatting consistent to the rules specified by the Ruby Time.strftime function.
        /// <see href="https://help.shopify.com/themes/liquid/filters/additional-filters#date"/>
        /// <see href="https://ruby-doc.org/core-3.0.0/Time.html#method-i-strftime"/>
        /// </summary>
        /// <param name="dateTime"/>
        /// <param name="pattern"/>
        /// <returns>a string version of dateTime matching pattern.</returns>
        public static string ToStrFTime(this DateTime dateTime, string pattern)
        {
            string output = "";

            int n = 0;

            while (n < pattern.Length)
            {
                string s = pattern.Substring(n, 1);

                if (n + 1 >= pattern.Length || s != "%")
                    output += s;
                else
                {
                    string directive = pattern.Substring(++n, 1);
                    if (FORMAT_FLAGS.Contains(directive) && pattern.Length > n)
                    {
                        directive += pattern.Substring(++n, 1); //Add the directove 
                    }
                    output += Formats.ContainsKey(directive) ? Formats[directive].Invoke(dateTime) : "%" + directive;
                }
                n++;
            }

            return output;
        }
    }
}
