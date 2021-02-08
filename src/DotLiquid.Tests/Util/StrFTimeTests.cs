using System;
using System.Globalization;
using System.Threading;
using DotLiquid.Util;
using NUnit.Framework;

namespace DotLiquid.Tests.Util
{
    [Description("See https://help.shopify.com/themes/liquid/filters/additional-filters#date")]
    [TestFixture]
    public class StrFTimeTests
    {
        // Descriptions below are taken from the Ruby Yime.strftime documentation.
        [TestCase("%a", ExpectedResult = "Sun")] // Abbreviated weekday name (Mon)
        [TestCase("%A", ExpectedResult = "Sunday")] // Full weekday name (Monday)
        [TestCase("%b", ExpectedResult = "Jan")] // Abbreviated month name (Jan)
        [TestCase("%B", ExpectedResult = "January")] // Full month name (January)
        [TestCase("%^B", ExpectedResult = "JANUARY")] // Upcased month name
		[TestCase("%^_10B", ExpectedResult = "   JANUARY")] // Upcase, then Space-pad
		[TestCase("%_^10B", ExpectedResult = "   JANUARY")] // Space-pad, then Upcase
        [TestCase("%c", ExpectedResult = "Sun Jan 08 14:32:14 2012")] // Date and time representation (Thu Aug 23 14:55:02 2001)
        [TestCase("%C", ExpectedResult = "20")] //Century (20 in 2009)
        [TestCase("%d", ExpectedResult = "08")] // Day of Month, zero-padded (01..31)
        [TestCase("%-d", ExpectedResult = "8")] // Remove zero-padding
        [TestCase("%D", ExpectedResult = "01/08/12")] // Short date (%m/%d/%y)
        [TestCase("%e", ExpectedResult = " 8")] // Day of month, space-padded ( 1..31)
        // E - not specified
        // f - not specified
        [TestCase("%F", ExpectedResult = "2012-01-08")] // ISO 8601 date (2001-08-23)
        // g // Week-based year, last two digits (00..99)
        // G // Week based year (2001)
        [TestCase("%h", ExpectedResult = "Jan")] // Abbreviated month name (Jan)
        [TestCase("%H", ExpectedResult = "14")] // Hour in 24h format, zero-padded (00..23)
        // i - not specified
        [TestCase("%I", ExpectedResult = "02")] // Hour in 12h format, zero-padded (01..12)
        [TestCase("%j", ExpectedResult = "008")] // Day of year (001..366)
        // J - not specified
        [TestCase("%k", ExpectedResult = "14")] //Hour in 24-hour format, blank-padded ( 0..23)
        // K - not specified
        [TestCase("%l", ExpectedResult = "2")] //Hour in 12-hour format, blank-padded ( 0..12)
        [TestCase("%L", ExpectedResult = "123")] // Millisecond of the second (000..999). The digits under millisecond are truncated to not produce 1000.
        [TestCase("%m", ExpectedResult = "01")] // Month of the year, zero-padded (01..12)
        [TestCase("%-m", ExpectedResult = "1")] // Remove zero-padding
        [TestCase("%_m", ExpectedResult = " 1")] // Replace zero-pad with space-padding
        [TestCase("%_3m", ExpectedResult = "  1")] // Replace single zero-pad with multi-space-pad
        [TestCase("%010m", ExpectedResult = "0000000001")] // Replace single zero-pad with multi-zero-pad
        [TestCase("%M", ExpectedResult = "32")] // Minute of the hour (00..59)
        [TestCase("%n", ExpectedResult = "\n")] // new-line character (\n)
        // N // Fractional seconds digits, default is 9 digits (nanosecond)
        // o - not specified
        // O - not specified
        [TestCase("%p", ExpectedResult = "PM")] // Meridian indicator, uppercase (AM/PM)
        [TestCase("%P", ExpectedResult = "pm")] // Meridian indicator, lowercase (am/pm)
        // q - not specified
        [TestCase("%Q", ExpectedResult = "1326033134123")] // Milliseconds since 1970
        [TestCase("%r", ExpectedResult = "02:32:14 PM")] // 12-hour clock time (02:55:02 PM)
        [TestCase("%R", ExpectedResult = "14:32")] // 24-hour HH:MM time, equivalent to %H:%M (14:55)
        [TestCase("%s", ExpectedResult = "1326033134")] // Seconds since 1970
        [TestCase("%S", ExpectedResult = "14")] //Second (00..60)
        [TestCase("%t", ExpectedResult = "\t")] // Tab character (\t)
        [TestCase("%T", ExpectedResult = "14:32:14")] // ISO 8601 time format (14:55:02)
        [TestCase("%u", ExpectedResult = "7")] // Day of the week, Monday is 1 (1..7)
        [TestCase("%U", ExpectedResult = "02")] // Week of the year, Sunday as the first day of week one (01..53)
        [TestCase("%v", ExpectedResult = " 8-JAN-2012")] // VMS date (%e-%^b-%4Y)
        // V // Week number of the week-based year (01..53)
        [TestCase("%w", ExpectedResult = "0")] // Day of the week, Sunday is 0 (0..6)
        [TestCase("%W", ExpectedResult = "01")] // Week number with the first Monday as the first day of week one (00..53)
        [TestCase("%x", ExpectedResult = "08/01/2012")] // Locale Date representation (08/23/01)
        [TestCase("%X", ExpectedResult = "14:32:14")] // Locale Time representation (14:55:02)
        [TestCase("%y", ExpectedResult = "12")] // Year, last two digits (00-99)
        [TestCase("%Y", ExpectedResult = "2012")] // Year with century (2001)
        [TestCase("%_2Y", ExpectedResult = "2012")] // Ensure space-pad does not truncate.
        [TestCase("%", ExpectedResult = "%")]
        public string TestFormat(string format)
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Assert.That(CultureInfo.CurrentCulture, Is.EqualTo(new CultureInfo("en-GB")));
                return new DateTime(2012, 1, 8, 14, 32, 14, 123).ToStrFTime(format);
            }
        }

        [Test]
        // NOTE: "%Z" does not match spec: Ruby DateTime.strftime specification states that '%Z' should return a timezone description, such as 'GMT' or 'CST'
        public void TestTimeZone()
        {
            var now = DateTimeOffset.Now;
            string timeZoneOffset = now.ToString("zzz");
            Assert.That(now.DateTime.ToStrFTime("%Z"), Is.EqualTo(timeZoneOffset));
        }

        [TestCase("%z", ExpectedResult = "Z")] // ISO 8601 offset from UTC in timezone (1 minute=1, 1 hour=100) If timezone cannot be determined, no characters (+100)
        [TestCase("%:z", ExpectedResult = "Z")] // hour and minute offset from UTC with a colon (e.g. +09:00)
        //[TestCase("%Z", ExpectedResult = "GMT")] //Abbreviated time zone name (.e.g CST) or similar information. (OS dependent)
        public string TestTimeZoneUTC(string format)
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Assert.That(CultureInfo.CurrentCulture, Is.EqualTo(new CultureInfo("en-GB")));
                return DateTime.UtcNow.ToStrFTime(format);
            }
        }

        [TestCase("%z", ExpectedResult = "+0100")] // ISO 8601 offset from UTC in timezone (1 minute=1, 1 hour=100) If timezone cannot be determined, no characters (+100)
        [TestCase("%:z", ExpectedResult = "+01:00")] // hour and minute offset from UTC with a colon (e.g. +09:00)
        //[TestCase("%Z", ExpectedResult = "BST")] //Abbreviated time zone name (.e.g CST) or similar information. (OS dependent)
        public string TestTimeZoneLocal(string format)
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Assert.That(CultureInfo.CurrentCulture, Is.EqualTo(new CultureInfo("en-GB")));
                // Jun-10 is during UK British Summer Time (BST / +01:00)
                return DateTime.Parse("2012-06-10T14:32:14+01:00").ToStrFTime(format);
            }
        }
    }
}
