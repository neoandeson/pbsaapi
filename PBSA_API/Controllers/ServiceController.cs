using System.Collections.Generic;
using AutoMapper;
using DataService.Models;
using DataService.Services;
using DataService.Utils;
using DataService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBarberService _barberService;

        public ServiceController(IMapper mapper, IBarberService barberService)
        {
            _mapper = mapper;
            _barberService = barberService;
        }

        [HttpPost("add-test")]
        public ActionResult AddBarberServiceForTest(
            [FromQuery] string barberId,
            [FromQuery] string name,
            [FromQuery] int durationMinute)
        {
            BarberServices service = new BarberServices
            {
                BarberId = barberId,
                Name = name,
                DurationMinute = durationMinute,
                InUsed = true,
                CreatedTime = DateTimeUtil.GetTimeNow()
            };
            var rs = _barberService.AddOrUpdateBarberService(service);
            if (rs != null)
            {
                return new JsonResult(new {service = rs}) {StatusCode = StatusCodes.Status200OK};
            }
            return new JsonResult("Cannot create service") {StatusCode = StatusCodes.Status409Conflict};
        }
        
        [HttpPost("add-or-update")]
        public ActionResult AddBarberService([FromBody] BarberServiceViewModel serviceViewModel)
        {
            BarberServices service = new BarberServices
            {
                Id = serviceViewModel.Id,
                BarberId = serviceViewModel.BarberId,
                Name = serviceViewModel.Name,
                DurationMinute = serviceViewModel.DurationMinute,
                Price = serviceViewModel.Price,
                Currency = serviceViewModel.Currency,
                InUsed = true,
                CreatedTime = DateTimeUtil.GetTimeNow()
            };
            var rs = _barberService.AddOrUpdateBarberService(service);
            if (rs != null)
            {
                return new JsonResult(new {service = rs}) {StatusCode = StatusCodes.Status200OK};
            }
            return new JsonResult("Cannot create or update service") {StatusCode = StatusCodes.Status500InternalServerError};
        }
        
        [HttpPost("remove")]
        public ActionResult RemoveBarberService(int serviceId)
        {
            
            var rs = _barberService.RemoveBarberService(serviceId);
            if (rs)
            {
                return new OkResult();
            }
            return new JsonResult("Cannot remove service") {StatusCode = StatusCodes.Status500InternalServerError};
        }
        
        [HttpGet("get-all")]
        public List<BarberServiceViewModel> GetBarberServices(string barberId)
        {
            List<BarberServices> services = _barberService.GetServicesByBarber(barberId);

            return _mapper.Map<List<BarberServiceViewModel>>(services);
        }
    }
}