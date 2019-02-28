using System;
using System.Collections.Generic;
using DataService.Components.Schedule;
using DataService.Models;
using DataService.Utils;

namespace DataService.ViewModels
{
    public class ScheduleViewModel
    {
        public string BarberId { get; set; }
        public TimePoint StartTime { get; set; }
        public TimePoint EndTime { get; set; }
        public string RepetitionType { get; set; }
        public List<string> Repetitions { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
    }

    public static class ExtendedScheduleViewModel
    {
        public static List<BarberSchedules> ToBarberSchedules(this ScheduleViewModel scheduleViewModel)
        {
            List<BarberSchedules> schedules = new List<BarberSchedules>();

            foreach (var repetition in scheduleViewModel.Repetitions)
            {
                BarberSchedules schedule = new BarberSchedules
                {
                    BarberId = scheduleViewModel.BarberId,
                    StartTimeMinute = scheduleViewModel.StartTime.GetMinuteLength(),
                    EndTimeMinute = scheduleViewModel.EndTime.GetMinuteLength(),
                    RepetitionType = scheduleViewModel.RepetitionType,
                    Repetition = repetition,
                    InUsed = true,
                    CreatedTime = DateTimeUtil.GetTimeNow()
                };
                schedules.Add(schedule);
            }

            return schedules;
        }
    }
}