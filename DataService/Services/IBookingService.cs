using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Components.Schedule;
using DataService.Constants;
using DataService.Models;
using DataService.Repositories;
using DataService.Utils;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IBookingService
    {
        Dictionary<DateTimeOffset, Scheduler> GetSchedulers(string barberId, List<DateTimeOffset> dates);

        Dictionary<DateTimeOffset, List<TimePoint>> GetAvailableSlots(string barberId, List<DateTimeOffset> dates, List<BarberServices> bookedServices);

        Bookings BookSlot(string customerId, string barberId, DateTimeOffset time, List<BarberServices> bookedServices, out bool result);

        bool CheckinSlot(int bookingId, double longitude, double latitude,
            out Dictionary<DateTimeOffset, List<TimePoint>> suggestTimes);
        
        bool ReBookSlot(int bookingId, DateTimeOffset modifiedTime,
            out Dictionary<DateTimeOffset, List<TimePoint>> suggestTimes);

        Bookings CheckoutSlot(int bookingId, out bool result);

        Bookings CancelBooking(int bookingId, string actorId, out bool result, out string previousState);
        
        List<Bookings> GetFullBookingsByDate(string barberId, DateTimeOffset date);

        List<Bookings> GetIncomingBookingsOfCustomer(string customerId);
    }

    public class BookingService : IBookingService
    {
        private readonly IBarberScheduleRepository _scheduleRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public BookingService(IBarberScheduleRepository scheduleRepository, IBookingRepository bookingRepository, 
            IBookingServiceRepository bookingServiceRepository, IPaymentMethodRepository paymentMethodRepository)
        {
            _scheduleRepository = scheduleRepository;
            _bookingRepository = bookingRepository;
            _bookingServiceRepository = bookingServiceRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        private int GetTotalServingTime(IEnumerable<BarberServices> services)
        {
            return services.Sum(bs => bs.DurationMinute).RoundTo5();
        }

        public Dictionary<DateTimeOffset, Scheduler> GetSchedulers(string barberId, List<DateTimeOffset> dates)
        {
            Dictionary<DateTimeOffset, Scheduler> result = new Dictionary<DateTimeOffset, Scheduler>();

            //Fetch Bookings
            List<Bookings> bookings = _bookingRepository
                .GetBookingsByDates(barberId, dates, false, 
                    b => b.State == BookingConstants.Booked || b.State == BookingConstants.CheckedIn);
            Dictionary<DateTimeOffset, List<Bookings>> bookingDictionary = 
                bookings.ClassifyToDictionary(booking => booking.BookedTime.GetDate());
                    
            //Fetch BarberSchedules
            Dictionary<DateTimeOffset, List<BarberSchedules>> barberScheduleDictionary =
                _scheduleRepository.GetSchedulesOfBarber(barberId, dates);
            
            dates.ForEach(date =>
            {
                //Convert Bookings into Slot
                List<Slot> bookedSlots = bookingDictionary
                    .GetValueOrDefault(date)
                    ?.Select(booking => booking.ToSlot())
                    .ToList();
                
                //Convert BarberSchedules into FreeTime
                List<FreeTime> freeTimes = barberScheduleDictionary
                    .GetValueOrDefault(date)
                    ?.Select(schedule => schedule.ToFreeTime(date))
                    .Where(freeTime => freeTime.YetToCome() || freeTime.OccursNow())
                    .ToList();

                if (freeTimes == null) return;
                
                //Limit start time for current freetime
                var occursNow = freeTimes.Find(freeTime => freeTime.OccursNow());
                if (occursNow != null)
                {
                    var now = DateTimeUtil.GetTimeNow();
                    now = now.SetTime(now.Hour, now.Minute, 0); //Set second tick to 0

                    occursNow.Start = now.ToTimePoint();
                }
                
                Scheduler scheduler = new Scheduler(freeTimes);
                scheduler.AddBookedSlots(bookedSlots);
                
                result.Add(date, scheduler);
            });

            return result;
        }

        public Dictionary<DateTimeOffset, List<TimePoint>> GetAvailableSlots(string barberId, List<DateTimeOffset> dates, List<BarberServices> bookedServices)
        {
            Dictionary<DateTimeOffset, Scheduler> schedulers = GetSchedulers(barberId, dates);
            int totalTime = GetTotalServingTime(bookedServices);
            
            Dictionary<DateTimeOffset, List<TimePoint>> result = 
                schedulers.ToDictionary(pair => pair.Key, pair => pair.Value.GetAvailableTimes(totalTime));

            return result;
        }

        public Bookings BookSlot(string customerId, string barberId, DateTimeOffset time,
            List<BarberServices> bookedServices, out bool result)
        {
            Dictionary<DateTimeOffset, Scheduler> schedulers = GetSchedulers(barberId, new List<DateTimeOffset> {time.GetDate()});
            int totalTimeInMinute = GetTotalServingTime(bookedServices);

            if (schedulers.Count == 0)
            {
                result = false;
                return null;
            }
            
            var slot = new Slot(new TimePoint(time), totalTimeInMinute);
            var bookResult = schedulers.Values.First().Book(slot);

            if (bookResult)
            {
                PaymentMethods customerPaymentMethod = 
                    _paymentMethodRepository.GetDefaultPaymentMethod(customerId);

                Bookings booking = new Bookings
                {
                    BarberId = barberId,
                    CustomerId = customerId,
                    BookedTime = time,
                    DurationMinute = totalTimeInMinute,
                    State = BookingConstants.Booked,
                    CustomerPaymentType = customerPaymentMethod.PaymentType
                };
                _bookingRepository.AddBooking(booking);
                
                bookedServices.ForEach(service =>
                {
                    var bookedService = new BookingServices
                    {
                        BookingId = booking.Id, 
                        ServiceId = service.Id
                    };
                    _bookingServiceRepository.Add(bookedService);
                });

                result = true;
                return booking;
            }

            result = false;
            return null;
        }

        public bool CheckinSlot(int bookingId, double longitude, double latitude,
            out Dictionary<DateTimeOffset, List<TimePoint>> suggestTimes)
        {
            suggestTimes = null;

            Bookings booking = _bookingRepository.GetBooking(bookingId, true);
            
            //Check location
            Barbers barber = booking.Barber;
            double distance = LocationUtil.CalculateDistance(longitude,latitude, 
                barber.Longitude.GetValueOrDefault(),barber.Latitude.GetValueOrDefault()); 
            if (distance > LocationUtil.AcceptableDistance)
            {
                return false;
            }
            
            //Check in-time
            if (booking.State == BookingConstants.Suspended)
            {
                var bookedServices = booking.BookingServices.Select(bs => bs.Service).ToList(); 
                suggestTimes = GetAvailableSlots(booking.BarberId, new List<DateTimeOffset> {DateTimeUtil.GetToday()}, bookedServices);
                return false;
            }

            //Update state
            if (booking.State == BookingConstants.Booked)
            {
                booking.State = BookingConstants.CheckedIn;
                _bookingRepository.UpdateBooking(booking);
                return true;
            }

            return false;
        }

        public bool ReBookSlot(int bookingId, DateTimeOffset modifiedTime,
            out Dictionary<DateTimeOffset, List<TimePoint>> suggestTimes)
        {
            suggestTimes = null;

            Bookings booking = _bookingRepository.GetBooking(bookingId, true);
            
            //Perform re-book: change time and state of suspended booking
            if (booking.State == BookingConstants.Suspended)
            {
                var bookedServices = booking.BookingServices.Select(bs => bs.Service).ToList();
                var today = DateTimeUtil.GetToday();
                suggestTimes = GetAvailableSlots(booking.BarberId, new List<DateTimeOffset> {today}, bookedServices);
                
                //re-check to make sure the booked time is still available
                if (suggestTimes[today].Any(timePoint => timePoint.Time == modifiedTime))
                {
                    booking.State = BookingConstants.Booked;
                    booking.BookedTime = modifiedTime;
                    _bookingRepository.UpdateBooking(booking);
                    return true;
                }

                //If there was a booking request that occurred and changed the schedule
                return false;
            }

            //The booking's state is not "suspended"
            return false;
        }

        public Bookings CheckoutSlot(int bookingId, out bool result)
        {   
            Bookings booking = _bookingRepository.GetBooking(bookingId, true);

            //Update state
            if (booking.State == BookingConstants.CheckedIn)
            {
                booking.State = BookingConstants.CheckedOut;
                _bookingRepository.UpdateBooking(booking);

                result = true;
                return booking;
            }

            result = false;
            return null;
        }

        public Bookings CancelBooking(int bookingId, string actorId, out bool result, out string previousState)
        {
            result = false;

            Bookings booking = _bookingRepository.GetBooking(bookingId, true);
            previousState = booking.State;

            if (!booking.IsCancellable())
            {
                return null;
            }

            //Barber cancels
            if (booking.BarberId == actorId)
            {
                booking.State = BookingConstants.BarberCancelled;
                _bookingRepository.UpdateBooking(booking);

                result = true;
                return booking;
            }

            //Customer cancels
            if (booking.CustomerId == actorId)
            {
                booking.State = BookingConstants.CustomerCancelled;
                _bookingRepository.UpdateBooking(booking);

                result = true;
                return booking;
            }

            return null;
        }

        public List<Bookings> GetFullBookingsByDate(string barberId, DateTimeOffset date)
        {
            return _bookingRepository
                .GetBookingsByDates(barberId, new List<DateTimeOffset> {date}, true, 
                    b => b.State == BookingConstants.Booked || b.State == BookingConstants.CheckedIn);
        }

        public List<Bookings> GetIncomingBookingsOfCustomer(string customerId)
        {
            List<Bookings> bookings = _bookingRepository.GetCustomerBookings(customerId,
                DateTimeUtil.GetToday(), null,
                true, null);

            return bookings;
        }
    }
}