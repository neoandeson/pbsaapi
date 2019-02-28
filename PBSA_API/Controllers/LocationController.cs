using DataService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost("add")]
        public ActionResult AddCustomerLocation(
            [FromQuery] string customerId,
            [FromQuery] int bookingId, 
            [FromQuery] double longitude,
            [FromQuery] double latitude)
        {
            _locationService.AddCustomerLocation(customerId, bookingId, longitude, latitude);
            return new OkResult();
        }
    }
}