using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Constants;
using DataService.Infrastructure;
using DataService.Models;
using DataService.Utils;
using Microsoft.EntityFrameworkCore;

namespace DataService.Repositories
{
    public interface IBookingRepository : IRepository<Bookings>
    {
        List<Bookings> GetBookingsByDates(string barberId, List<DateTimeOffset> dates,
            bool getFullDetails, Func<Bookings, bool> filter);
        List<Bookings> GetCustomerBookings(string customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate,
            bool getFullDetails, Func<Bookings, bool> filter);
        Bookings GetBooking(int bookingId, bool getFullDetails);
        Bookings AddBooking(Bookings booking);
        Bookings UpdateBooking(Bookings booking);
        BookingHistory UpdateBookingState(Bookings booking);
        void CheckStateChanged(IEnumerable<Bookings> bookings);
    }

    public class BookingRepository : Repository<Bookings>, IBookingRepository
    {   
        public BookingRepository(PBSAContext context) : base(context)
        {
        }

        public List<Bookings> GetBookingsByDates(string barberId, List<DateTimeOffset> dates,
            bool getFullDetails, Func<Bookings, bool> filter)
        {
            dates.Sort();
            var minDate = dates.First();
            var maxDate = dates.Last();
            
            var bookingQuery = GetAll()
                .Where(booking => booking.BarberId == barberId
                                  && booking.BookedTime.Date >= minDate.Date
                                  && booking.BookedTime.Date <= maxDate.Date);

            //Include details along with bookings
            List<Bookings> bookings;
            if (getFullDetails)
            {
                bookings = bookingQuery
                    .Include(b => b.Customer)
                    .Include(b => b.BookingServices)
                        .ThenInclude(bs => bs.Service)
                    .ToList();
            }
            else
            {
                bookings = bookingQuery.ToList();
            }
            
            //Check booking state changed
            CheckStateChanged(bookings);

            //Filter bookings if needed
            if (filter != null)
            {
                bookings = bookings.Where(filter).ToList();
            }

            return bookings;
        }

        public List<Bookings> GetCustomerBookings(string customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate,
            bool getFullDetails, Func<Bookings, bool> filter)
        {   
            IQueryable<Bookings> bookingQuery = GetAll()
                .Where(booking => booking.CustomerId == customerId);

            if (fromDate != null)
            {
                bookingQuery = bookingQuery.Where(booking =>
                    fromDate.GetValueOrDefault().GetDate() <= booking.BookedTime.GetDate());
            }
            
            if (toDate != null)
            {
                bookingQuery = bookingQuery.Where(booking =>
                    booking.BookedTime.GetDate() <= toDate.GetValueOrDefault().GetDate());
            }

            bookingQuery = bookingQuery
                .Include(b => b.Barber)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service);
            
            //Include details history
            if (getFullDetails)
            {
                bookingQuery = bookingQuery
                    .Include(b => b.BookingHistory);
            }

            List<Bookings> bookings = bookingQuery.ToList();
            
            //Check booking state changed
            CheckStateChanged(bookings);

            //Filter bookings if needed
            if (filter != null)
            {
                bookings = bookings.Where(filter).ToList();
            }

            return bookings;
        }

        public Bookings GetBooking(int bookingId, bool getFullDetails)
        {
            var bookingQuery = GetAll()
                .Where(b => b.Id == bookingId);

            //Include details along with bookings
            Bookings booking;
            if (getFullDetails)
            {
                booking = bookingQuery
                    .Include(b => b.Barber)
                    .Include(b => b.Customer)
                    .Include(b => b.BookingServices)
                        .ThenInclude(bs => bs.Service)
                    .First();
            }
            else
            {
                booking = bookingQuery.First();
            }
            
            //Check booking state changed
            CheckStateChanged(new[] {booking});

            return booking;
        }

        public Bookings AddBooking(Bookings booking)
        {
            var rs = Add(booking);

            UpdateBookingState(booking);

            return rs;
        }

        public Bookings UpdateBooking(Bookings booking)
        {
            var rs = Update(booking);

            UpdateBookingState(booking);

            return rs;
        }

        public BookingHistory UpdateBookingState(Bookings booking)
        {
            var history = new BookingHistory
            {
                BookingId = booking.Id,
                BookedTime = booking.BookedTime,
                TransitionTime = DateTimeUtil.GetTimeNow(),
                State = booking.State
            };
            return AddAny(history);
        }

        public void CheckStateChanged(IEnumerable<Bookings> bookings)
        {
            DateTimeOffset timeNow = DateTimeUtil.GetTimeNow();
            foreach (var booking in bookings)
            {
                if (booking.CheckStateChanged(timeNow))
                {
                    UpdateBooking(booking);
                }
            }
        }
    }
}