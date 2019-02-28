using System.Collections.Generic;
using System.Linq;
using DataService.Constants;
using DataService.Models;
using DataService.Repositories;
using DataService.Utils;

namespace DataService.Services
{
    public interface IBarberScheduleService
    {
        int AddBarberSchedules(params BarberSchedules[] schedule);
        int RemoveBarberSchedules(params BarberSchedules[] schedule);
        List<BarberSchedules> GetSchedulesByBarber(string barberId);
    }
    
    public class BarberScheduleService : IBarberScheduleService
    {
        
        private readonly IBarberScheduleRepository _scheduleRepository;

        public BarberScheduleService(IBarberScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public int AddBarberSchedules(params BarberSchedules[] schedules)
        {
            var count = 0;
            foreach (var schedule in schedules)
            {
                if (_scheduleRepository.Exist(s => s.EqualsTo(schedule))) continue;
                _scheduleRepository.Add(schedule);
                count++;
            }
            
            return count;
        }

        public int RemoveBarberSchedules(params BarberSchedules[] schedules)
        {
            var count = 0;
            List<BarberSchedules> toRemoves = new List<BarberSchedules>();
            
            foreach (var schedule in schedules)
            {
                BarberSchedules toRemove = _scheduleRepository.Find(s => s.EqualsTo(schedule));
                if (toRemove != null)
                {
                    toRemove.InUsed = false;
                    toRemove.ExpiredTime = DateTimeUtil.GetTimeNow();
                    toRemoves.Add(toRemove);
                    count++;
                }
            }

            _scheduleRepository.UpdateBulk(toRemoves);

            return count;
        }

        public List<BarberSchedules> GetSchedulesByBarber(string barberId)
        {
            return _scheduleRepository.GetAll()
                .Where(schedule => schedule.BarberId == barberId && schedule.InUsed)
                .ToList();
        }
    }
}