using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace BucketList
{
   public static class DateTimeExtensions
    {

        public static long GetDateTimeInMillis(this DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1);
            return (long)timeSpan.TotalMilliseconds;
        }

        public static DateTime GetDateTimeFromMillis(this long millis)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(millis);
        }

        public static string ToNiceString(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month;
            var dayOfMonth = dateTime.Day;
            var selectedDateInString
                = $"{string.Format("{0:00}", dayOfMonth)}.{string.Format("{0:00}", month)}.{year}";
            return selectedDateInString;
        }
    }
}
