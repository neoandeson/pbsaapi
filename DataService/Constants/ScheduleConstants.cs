using DataService.Models;

namespace DataService.Constants
{
    public static class ScheduleConstants
    {
        public static bool EqualsTo(this BarberSchedules schedule, BarberSchedules schedule2)
        {
            return schedule.Repetition == schedule2.Repetition
                   && schedule.RepetitionType == schedule2.RepetitionType
                   && schedule.StartTimeMinute == schedule2.StartTimeMinute
                   && schedule.EndTimeMinute == schedule2.EndTimeMinute 
                   && schedule.InUsed == schedule2.InUsed;
        }
        
        public static bool HaveSameTimes(this BarberSchedules schedule, BarberSchedules schedule2)
        {
            return schedule.StartTimeMinute == schedule2.StartTimeMinute
                   && schedule.EndTimeMinute == schedule2.EndTimeMinute;
        }
    }
}