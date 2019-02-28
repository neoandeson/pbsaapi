using System;
using DataService.Components.Schedule;
using DataService.Models;
using DataService.Utils;

namespace DataService.Constants
{
    public static class BarberScheduleConstants
    {
        public static class RepetitionType
        {
            public const string Weekly = "weekly";
            public const string Once = "once";
        }
        
        public static class Repetition
        {
            public const string Monday = "mon";
            public const string Tuesday = "tue";
            public const string Wednesday = "wed";
            public const string Thursday = "thu";
            public const string Friday = "fri";
            public const string Saturday = "sat";
            public const string Sunday = "sun";
        }

        public static FreeTime ToFreeTime(this BarberSchedules schedule, DateTimeOffset date)
        {
            var startTime = date.SetTime(schedule.StartTimeMinute);
            var endTime = date.SetTime(schedule.EndTimeMinute);
            
            var freeTime = new FreeTime
            {
                Start = new TimePoint(startTime),
                End = new TimePoint(endTime)
            };

            return freeTime;
        }
    }
}