using System.Collections.Generic;
using AutoMapper;
using DataService.Models;
using DataService.Services;
using DataService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BarberController : ControllerBase
    {
        private readonly IBarberService _barberService;
        private readonly IMapper _mapper;

        public BarberController(IBarberService barberService, IMapper mapper)
        {
            this._barberService = barberService;
            this._mapper = mapper;
        }

        [HttpGet]
        public List<BarberViewModel> SearchByName(string name)
        {
            List<Barbers> barbers = _barberService.GetBarbersByName(name);
            List<BarberViewModel> mapListBarberVM = _mapper.Map<List<BarberViewModel>>(barbers);
            return mapListBarberVM;
        }

        [HttpGet]
        public List<BarberViewModel> SearchByCity(string city)
        {
            List<Barbers> barbers = _barberService.GetBarbersByCity(city);
            List<BarberViewModel> mapListBarberVM = _mapper.Map<List<BarberViewModel>>(barbers);
            return mapListBarberVM;
        }

        [HttpGet]
        public Barbers GetBarberInfo(string username)
        {
            Barbers barber = _barberService.GetBarberInfo(username);
            return barber;
        }

        [HttpPost]
        public ActionResult RegisterBarberForTesting(
            [FromQuery] string username, 
            [FromQuery] string fullName, 
            [FromQuery] string phone)
        {
            var rs = _barberService.RegisterBarber(username, fullName, phone);
            if (rs != null)
            {
                return new JsonResult(new {Id = rs.UserId});
            }
            return new JsonResult("Cannot create new barber") {StatusCode = StatusCodes.Status409Conflict};
        }
    }
}