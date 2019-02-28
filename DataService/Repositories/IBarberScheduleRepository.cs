using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Infrastructure;
using DataService.Models;
using DataService.Utils;
using static DataService.Constants.BarberScheduleConstants;

namespace DataService.Repositories
{
    public interface IBarberScheduleRepository : IRepository<BarberSchedules>
    {
        Dictionary<DateTimeOffset, List<BarberSchedules>> GetSchedulesOfBarber(string barberId, List<DateTimeOffset> dates);
    }

    public class BarberScheduleRepository : Repository<BarberSchedules>, IBarberScheduleRepository
    {
        public BarberScheduleRepository(PBSAContext context) : base(context)
        {
        }

        private List<BarberSchedules> ClassifyScheduleByDates(List<BarberSchedules> schedules, DateTimeOffset date)
        {
            List<BarberSchedules> result = new List<BarberSchedules>();

            bool once = false;
            schedules.ForEach(schedule =>
            {
                switch (schedule.RepetitionType)
                {
                    case RepetitionType.Once:
                        if (DateTimeUtil.Convert(schedule.Repetition).GetDate() == date.GetDate())
                        {
                            once = true;
                            result.Add(schedule);
                        }
                        break;
                        
                    case RepetitionType.Weekly:
                        if (!once && DateTimeUtil.GetDayOfWeek(schedule.Repetition) == date.DayOfWeek)
                        {
                            result.Add(schedule);
                        }
                        break;
                }
            });

            return result;
        }

        public Dictionary<DateTimeOffset, List<BarberSchedules>> GetSchedulesOfBarber(string barberId, List<DateTimeOffset> dates)
        {   
            List<BarberSchedules> schedules = GetAll()
                .Where(barberSchedule => barberSchedule.BarberId == barberId && barberSchedule.InUsed)
                .ToList();
         
            schedules.Sort((s1, s2) =>
            {
                var i1 = s1.RepetitionType == RepetitionType.Once ? 0 : 1;
                var i2 = s2.RepetitionType == RepetitionType.Once ? 0 : 1;
                return i1 - i2;
            });
            
            //Classify by dates
            Dictionary<DateTimeOffset, List<BarberSchedules>> result = new Dictionary<DateTimeOffset, List<BarberSchedules>>();
            dates.ForEach(date =>
            {
                var key = date;
                var val = ClassifyScheduleByDates(schedules, date);
                
                result.Add(key, val);
            });
            return result;
        }
    }
}