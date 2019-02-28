using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Constants;
using DataService.Models;
using DataService.Services;
using DataService.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;

        public CustomerController(ICustomerService customerService, IBookingService bookingService)
        {
            _customerService = customerService;
            _bookingService = bookingService;
        }

        [HttpPost("register-test")]
        public ActionResult RegisterCustomerForTesting(
            [FromQuery] string username, 
            [FromQuery] string firstName, 
            [FromQuery] string middleName, 
            [FromQuery] string lastName, 
            [FromQuery] string phone)
        {
            var rs = _customerService.RegisterCustomer(username, firstName, middleName, lastName, phone);
            if (rs != null)
            {
                return new JsonResult(new {Id = rs.UserId});
            }
            return new JsonResult("Cannot create new customer") {StatusCode = StatusCodes.Status409Conflict};
        }

        [HttpGet("bookings/incoming")]
        public ActionResult GetIncomingBookingsOfCustomer(string customerId)
        {
            List<Bookings> bookings = _bookingService
                .GetIncomingBookingsOfCustomer(customerId)
                .OrderBy(booking => booking.BookedTime)
                .ToList();

            List<Bookings> passed = new List<Bookings>();
            List<Bookings> next = new List<Bookings>();
            List<Bookings> current = new List<Bookings>();

            DateTimeOffset now = DateTimeUtil.GetTimeNow();
            foreach (var booking in bookings)
            {
                if (booking.GetFinishTime() < now)
                {
                    passed.Add(booking);
                }
                else if (booking.BookedTime <= now && now <= booking.GetFinishTime())
                {
                    current.Add(booking);
                }
                else if (now < booking.BookedTime)
                {
                    next.Add(booking);
                }

                booking.RestrictDataField();
            }
            
            return new JsonResult(
                new
                {
                    notFound= bookings.Count == 0,
                    passed,
                    current,
                    next
                }) 
                {StatusCode = StatusCodes.Status200OK};
        }
    }
}