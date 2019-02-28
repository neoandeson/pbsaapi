using System;
using System.Collections.Generic;
using DataService.Utils;

namespace DataService.Components.Schedule
{
    public class FreeTime
    {
        public TimePoint Start { get; set; }
        public TimePoint End { get; set; }

        public int GetDurationMinute()
        {
            return End.GetMinuteLength() - Start.GetMinuteLength();
        }

        public List<TimePoint> CalculateAvailableSlots(int durationMinute)
        {
            List<TimePoint> result = new List<TimePoint>();

            int freeTimeDuration = GetDurationMinute();
            int numberOfSlot = freeTimeDuration / durationMinute;
            int remainderMinutes = freeTimeDuration % durationMinute; //Split from the bottom

            for (int i = 0; i < numberOfSlot; i++)
            {
                result.Add(Start.AddMinute(i * durationMinute + remainderMinutes));
            }

            return result;
        }

        public List<FreeTime> SplitFreeTimes(Slot slot)
        {
            List<FreeTime> result = new List<FreeTime>();

            FreeTime first = new FreeTime {Start = this.Start.Clone(), End = slot.StartPoint.Clone()};
            FreeTime second = new FreeTime {Start = slot.EndPoint.Clone(), End = this.End.Clone()};

            if (first.GetDurationMinute() > 0)
            {
                result.Add(first);
            }

            if (second.GetDurationMinute() > 0)
            {
                result.Add(second);
            }

            return result;
        }

        public override string ToString()
        {
            return $"{Start} - {End} (total {GetDurationMinute()} minutes)";
        }

        public bool Passed()
        {
            var now = DateTimeUtil.GetTimeNow().ToTimePoint();
            return End.TimeMillis < now.TimeMillis;
        }

        public bool OccursNow()
        {
            var now = DateTimeUtil.GetTimeNow().ToTimePoint();
            return Start.TimeMillis <= now.TimeMillis
                && now.TimeMillis < End.TimeMillis;
        }
        
        public bool YetToCome()
        {
            var now = DateTimeUtil.GetTimeNow().ToTimePoint();
            return now.TimeMillis < Start.TimeMillis;
        }
    }
}