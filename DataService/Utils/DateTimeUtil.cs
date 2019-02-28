using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataService.Components.Schedule;
using static DataService.Constants.BarberScheduleConstants;

namespace DataService.Utils
{
    public static class DateTimeUtil
    {
        public static readonly string Today = "today";
        private static readonly string Tomorrow = "tomorrow";
        public static readonly string ShortDateFormat = "dd/MM/yyyy";
        public static readonly int VnTimezoneOffset = 7;

        public static DateTimeOffset GetTimeNow()
        {
            return DateTimeOffset.Now;
        }
        public static DateTimeOffset GetToday()
        {
            return GetTimeNow().GetDate();
        }

        private static DateTimeOffset GetTomorrow()
        {
            return GetToday().AddDays(1);
        }

        /**
         * Use to convert a strDate into literal name, e.g: "dd/MM/yyyy" => "today"
         */
        public static string GetLiteralDateName(DateTimeOffset date)
        {
            if (GetToday() == date)
            {
                return Today;
            }

            if (GetTomorrow() == date)
            {
                return Tomorrow;
            }

            return date.ToDateStringVN();
        }

        /**
         * Use to convert a literal name into DateTimeOffset object,
         * e.g: "today" => DateTimeOffset object
         * e.g: "01/01/1970" => DateTimeOffset object
         */
        public static DateTimeOffset FromLiteralDateName(string strDate)
        {
            if (strDate == Today)
            {
                return GetToday();
            }

            if (strDate == Tomorrow)
            {
                return GetTomorrow();
            }

            return Convert(strDate);
        }

        public static DateTimeOffset Convert(string strDate)
        {
            DateTime date = DateTime.ParseExact(strDate, ShortDateFormat, CultureInfo.InvariantCulture);
            return new DateTimeOffset(date, TimeSpan.FromHours(VnTimezoneOffset));
        }
        
        public static List<DateTimeOffset> GetListDate(IEnumerable<string> strDates)
        {
            return strDates.Distinct().Select(FromLiteralDateName).ToList();
        }

        public static DateTimeOffset GetTimeFrom(string strDate, string strTime)
        {
            TimePoint.ParseHour(strTime, out var hour, out var minute);
            var dateTime = FromLiteralDateName(strDate).SetTime(hour, minute, 0);

            return dateTime;
        }

        public static DayOfWeek GetDayOfWeek(string s)
        {
            switch (s)
            {
                case Repetition.Monday:
                    return DayOfWeek.Monday;
                case Repetition.Tuesday:
                    return DayOfWeek.Tuesday;
                case Repetition.Wednesday:
                    return DayOfWeek.Wednesday;
                case Repetition.Thursday:
                    return DayOfWeek.Thursday;
                case Repetition.Friday:
                    return DayOfWeek.Friday;
                case Repetition.Saturday:
                    return DayOfWeek.Saturday;
                case Repetition.Sunday:
                    return DayOfWeek.Sunday;
                default:
                    return DayOfWeek.Monday;
            }
        }
    }

    public static class ExtendedDateTime
    {
        public static string ToDateStringVN(this DateTimeOffset source)
        {
            return source.ToString(DateTimeUtil.ShortDateFormat);
        }
        
        /**
         * This function is to maintains the offset of the source time object when get the date only
         * Never get a date from DateTimeOffset directly by property .Date!!!
         */
        public static DateTimeOffset GetDate(this DateTimeOffset source)
        {
            return new DateTimeOffset(source.Date, source.Offset);
        }
        
        public static DateTimeOffset SetTime(this DateTimeOffset source, int hour, int minute, int second)
        {
            return source.GetDate() + new TimeSpan(hour, minute, second);
        }
        
        public static DateTimeOffset SetTime(this DateTimeOffset source, int minuteLength)
        {
            int hour = minuteLength / 60;
            int minute = minuteLength - hour * 60;
            return source.GetDate() + new TimeSpan(hour, minute, 0);
        }

        public static bool IsToday(this DateTimeOffset source)
        {
            return source.GetDate() == DateTimeUtil.GetToday();
        }
        
        public static TimePoint ToTimePoint(this DateTimeOffset source)
        {
            return new TimePoint(source);
        }
    }
}