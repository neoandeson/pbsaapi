using System;
using DataService.Utils;

namespace DataService.Components.Schedule
{
    public class TimePoint
    {
        public double TimeMillis { get; private set; }
        private DateTimeOffset _time;
        public DateTimeOffset Time
        {
            get => _time;
            private set { 
                _time = value;
                TimeMillis = value.ToUnixTimeMilliseconds();
            }
        }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public TimePoint(DateTimeOffset time)
        {
            Time = time;
            Hour = time.Hour;
            Minute = time.Minute;
        }

        public static void ParseHour(string hourStr, out int hour, out int minute)
        {
            string[] times = hourStr.Split(":");
            hour = Convert.ToInt32(times[0]);
            minute = Convert.ToInt32(times[1]);
        }

        public int GetMinuteLength()
        {
            return Hour * 60 + Minute;
        }

        //Clone DateTimeOffset before modifying
        //https://stackoverflow.com/questions/4265399/how-can-i-clone-a-datetime-object-in-c
        public TimePoint AddMinute(int minute)
        {
            var newTime = Time;
            return new TimePoint(newTime.AddMinutes(minute));
        }
        
        public override string ToString()
        {
            return Hour + ":" + Minute;
        }

        public bool Equals(TimePoint obj)
        {
            return Hour == obj.Hour && Minute == obj.Minute;
        }

        public TimePoint Clone()
        {
            return new TimePoint(Time);
        }

        public TimePoint SetDate(DateTimeOffset date)
        {
            Time = date.SetTime(Hour, Minute, 0);
            return this;
        }
    }
}