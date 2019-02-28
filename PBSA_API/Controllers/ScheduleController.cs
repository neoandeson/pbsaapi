using System.Collections.Generic;
using System.Linq;
using DataService.Components.Schedule;
using DataService.Constants;
using DataService.Models;
using DataService.Services;
using DataService.Utils;
using DataService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IBarberScheduleService _scheduleService;

        public ScheduleController(IBarberScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost("update-all")]
        public ActionResult AddOrUpdateBarberSchedules([FromBody] List<ScheduleViewModel> scheduleViewModels)
        {
            //Get current schedules
            List<BarberSchedules> oldSchedules =
                _scheduleService.GetSchedulesByBarber(scheduleViewModels.First().BarberId);
            
            foreach (var scheduleViewModel in scheduleViewModels)
            {
                var newSchedules = scheduleViewModel.ToBarberSchedules();

                //Remove unchanged schedule (which exists in newSchedules)
                oldSchedules = oldSchedules.Where(s =>
                {
                    foreach (var newSchedule in newSchedules)
                    {
                        if (newSchedule.EqualsTo(s)) return false;
                    }
                    return true;
                }).ToList();

                _scheduleService.AddBarberSchedules(newSchedules.ToArray());
            }

            //Deactivate obsolete schedules
            _scheduleService.RemoveBarberSchedules(oldSchedules.ToArray());
            
            return new OkResult();
        }
        
        [HttpGet("get-all")]
        public List<ScheduleViewModel> GetBarberSchedules(string barberId)
        {
            var today = DateTimeUtil.GetToday();
            List<BarberSchedules> schedules = _scheduleService.GetSchedulesByBarber(barberId);

            var set = schedules
                .GroupBy(s => new { s.RepetitionType, s.StartTimeMinute, s.EndTimeMinute } )
                .Select(grouping => grouping.ToList())
                .ToList();

            List<ScheduleViewModel> result = new List<ScheduleViewModel>();
            foreach (var list in set)
            {
                var first = list.First();
                ScheduleViewModel scheduleViewModel = new ScheduleViewModel
                {
                    BarberId = first.BarberId,
                    CreatedTime = first.CreatedTime,
                    RepetitionType = first.RepetitionType,
                    StartTime = new TimePoint(today.SetTime(first.StartTimeMinute)),
                    EndTime = new TimePoint(today.SetTime(first.EndTimeMinute)),
                    Repetitions = list.Select(s => s.Repetition).ToList()
                };
                result.Add(scheduleViewModel);
            }
            
            return result;
        }
    }
}