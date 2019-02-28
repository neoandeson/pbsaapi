using DataService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("ref-tables")]
    [ApiController]
    public class RefTableController : ControllerBase
    {
        private readonly IRefTablesService _refTablesService;
        private readonly ILocationService _locationService;

        public RefTableController(IRefTablesService refTablesService, ILocationService locationService)
        {
            _refTablesService = refTablesService;
            _locationService = locationService;
        }

        [HttpPost("init-ref-table-data")]
        public ActionResult InitBookingStateDatabase()
        {
            _refTablesService.InitBookingStateDatabase();
            _refTablesService.InitComplaintTypeDatabase();
            _refTablesService.InitComplaintStateDatabase();
            _refTablesService.InitCurrencyCodeDatabase();
            _refTablesService.InitPaymentTypeDatabase();
            _refTablesService.InitTransactionStateDatabase();
            
            return new OkResult();
        }
        
        [HttpPost("init-city-district-db")]
        public ActionResult InitCityDistrictDatabase()
        {
            var rs = _locationService.InitCityDistrictDatabase();
            if (rs)
            {
                return new OkResult();
            }

            return new JsonResult("Data already initiated") {StatusCode = StatusCodes.Status409Conflict};
        }
    }
}