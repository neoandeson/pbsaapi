using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Components.Schedule;
using DataService.Constants;
using DataService.Models;
using DataService.Services;
using DataService.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBarberService _barberService;
        private readonly IPaymentMethodService _paymentService;

        public BookingController(IBookingService bookingService, IBarberService barberService, IPaymentMethodService paymentService)
        {
            _bookingService = bookingService;
            _barberService = barberService;
            _paymentService = paymentService;
        }
        
        [HttpGet("get-all")]
        public ActionResult GetAllBookingsByDate(string barberId, [FromQuery(Name = "date")] string dateStr)
        {
            DateTimeOffset date = DateTimeUtil.FromLiteralDateName(dateStr);
            List<Bookings> bookings = _bookingService.GetFullBookingsByDate(barberId, date)
                .OrderBy(b => b.BookedTime).ToList();

            Scheduler scheduler = _bookingService.GetSchedulers(barberId, new List<DateTimeOffset> {date}).First().Value;
            
            List<Bookings> freeTimes = new List<Bookings>();
            scheduler.AvailableList.ForEach(freeTime =>
            {
                freeTimes.Add(new Bookings
                {
                    BookedTime = freeTime.Start.SetDate(date).Time, 
                    DurationMinute = freeTime.GetDurationMinute(), 
                    CustomerId = "#FREETIME"
                });
            });
            
            bookings.AddRange(freeTimes);
            bookings = bookings.OrderBy(b => b.BookedTime).ToList();
            
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
                else
                {
                    next.Add(booking);
                }

                booking.RestrictDataField();
            }
            
            return new JsonResult(new {passed, current, next}) {StatusCode = StatusCodes.Status200OK};
        }

        [HttpGet("available-slot")]
        public Dictionary<string, List<TimePoint>> GetAvailableSlot(
            string barberId,
            [FromQuery(Name = "dates")] List<string> strDates, 
            [FromQuery] List<int> serviceIds)
        {
            //Get dates
            List<DateTimeOffset> dates = DateTimeUtil.GetListDate(strDates)
                .Where(date => date >= DateTimeUtil.GetToday()).ToList();

            //Get services of barber
            List<BarberServices> services = _barberService.GetServicesByBarber(barberId)
                .Where(s => serviceIds.Contains(s.Id)).ToList();
            
            var result = _bookingService.GetAvailableSlots(barberId, dates, services);

            return ConvertToReadableDate(result);
        }

        [HttpPost("book")]
        public ActionResult BookHaircut(
            [FromQuery] string customerId, 
            [FromQuery] string barberId,
            [FromQuery] string date,
            [FromQuery] string time,
            [FromQuery] List<int> serviceIds)
        {
            DateTimeOffset dateTime = DateTimeUtil.GetTimeFrom(date, time);
            
            //Get services of barber
            List<BarberServices> services = _barberService.GetServicesByBarber(barberId)
                .Where(s => serviceIds.Contains(s.Id)).ToList();

            var booking = _bookingService.BookSlot(customerId, barberId, dateTime, services, out var rs);
            if (rs)
            {
                //Make deposit transaction
                _paymentService.MakeDeposit(
                    customerId,
                    barberId,
                    PaymentConstants.PaymentAmount.BookingDepositAmount,
                    booking.Id);
                
                return new JsonResult( "Booked successfully") { StatusCode = StatusCodes.Status200OK };
            }

            return new JsonResult("Fail to book") { StatusCode = StatusCodes.Status404NotFound } ;
        }

        [HttpPost("check-in")]
        public ActionResult CheckIn(
            [FromQuery] int bookingId,
            [FromQuery] double longitude,
            [FromQuery] double latitude)
        {
            bool rs = _bookingService.CheckinSlot(bookingId, longitude, latitude, out var suggestTimes);

            if (rs)
            {
                return new JsonResult("Checked in successfully") {StatusCode = StatusCodes.Status200OK};
            }

            if (suggestTimes == null)
            {
                return new JsonResult("Fail to checkin") {StatusCode = StatusCodes.Status404NotFound};
            }
            
            return new JsonResult(ConvertToReadableDate(suggestTimes)) {StatusCode = StatusCodes.Status301MovedPermanently};
        }
        
        [HttpPost("re-book")]
        public ActionResult ReBookHaircut(
            [FromQuery] int bookingId,
            [FromQuery] string date,
            [FromQuery] string time)
        {
            DateTimeOffset dateTime = DateTimeUtil.GetTimeFrom(date, time);
            
            bool rs = _bookingService.ReBookSlot(bookingId, dateTime, out var suggestTimes);

            if (rs)
            {
                return new JsonResult("Re-book new slot successfully") {StatusCode = StatusCodes.Status200OK};
            }

            if (suggestTimes == null)
            {
                return new JsonResult("Fail to re-book") {StatusCode = StatusCodes.Status404NotFound};
            }
            
            return new JsonResult(ConvertToReadableDate(suggestTimes)) {StatusCode = StatusCodes.Status301MovedPermanently};
        }
        
        [HttpPost("check-out")]
        public ActionResult CheckOut([FromQuery] int bookingId)
        {
            Bookings booking = _bookingService.CheckoutSlot(bookingId, out var rs);

            if (rs)
            {
                //Return deposit back to customer
                List<Transactions> depositTransactions = _paymentService.GetDepositTransactionsByBooking(bookingId);
                depositTransactions.ForEach(transaction => _paymentService.ReturnDeposit(transaction.Id));

                //Get total money
                decimal totalPrice = booking.BookingServices
                    .Select(service => service.Service)
                    .Select(barberService => barberService.Price)
                    .Sum()
                    .GetValueOrDefault();

                //Create payment transaction
                _paymentService.ForcePay(
                    booking.CustomerId,
                    booking.BarberId,
                    booking.CustomerPaymentType,
                    totalPrice,
                    bookingId,
                    TransactionConstants.TransactionDetail.PaidTransaction);

                return new JsonResult("Checked out successfully") {StatusCode = StatusCodes.Status200OK};
            }

            return new JsonResult("Fail to checkout") {StatusCode = StatusCodes.Status404NotFound};
        }
        
        [HttpPost("cancel")]
        public ActionResult Cancel(
            [FromQuery] int bookingId,
            [FromQuery] string actorId)
        {
            Bookings booking = _bookingService.CancelBooking(bookingId, actorId, out var rs, out var previousState);

            if (rs)
            {
                if (booking.State == BookingConstants.BarberCancelled)
                {
                    //Return deposit back to customer
                    List<Transactions> depositTransactions = _paymentService.GetDepositTransactionsByBooking(bookingId);
                    depositTransactions.ForEach(transaction => _paymentService.ReturnDeposit(transaction.Id));
                    
                    if (previousState == BookingConstants.Booked || previousState == BookingConstants.CheckedIn)
                    {
                        //Charge a fine on barber
                        _paymentService.ForcePay(
                            booking.BarberId, 
                            booking.CustomerId,
                            PaymentConstants.PaymentType.Wallet,
                            PaymentConstants.PaymentAmount.BookingDepositAmount,
                            bookingId,
                            TransactionConstants.TransactionDetail.BarberCompensationTransaction);
                    }
                    return new JsonResult("Barber cancelled successfully") {StatusCode = StatusCodes.Status200OK};
                }
                
                if (booking.State == BookingConstants.CustomerCancelled)
                {
                    //Take away deposit and send to barber
                    List<Transactions> depositTransactions = _paymentService.GetDepositTransactionsByBooking(bookingId);
                    depositTransactions.ForEach(transaction => _paymentService.TakeAwayDeposit(transaction.Id));
                    
                    return new JsonResult("Customer cancelled successfully") {StatusCode = StatusCodes.Status200OK};
                }
            }

            return new JsonResult("Fail to cancel booking") {StatusCode = StatusCodes.Status404NotFound};
        }

        private Dictionary<string, List<TimePoint>> ConvertToReadableDate(Dictionary<DateTimeOffset, List<TimePoint>> input)
        {
            //Convert from "dd/MM/yyyy" into literal name, such as "today" or "tomorrow"
            var result = new Dictionary<string, List<TimePoint>>();
            foreach (var (dateTime, val) in input)
            {
                var key = DateTimeUtil.GetLiteralDateName(dateTime);
                result.Add(key, val);
            }

            return result;
        }
    }
}