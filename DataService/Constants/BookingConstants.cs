using System;
using DataService.Components.Schedule;
using DataService.Models;

namespace DataService.Constants
{
    public static class BookingConstants
    {
        public static readonly string Booked = "booked";
        public static readonly string CheckedIn = "checkedIn";
        public static readonly string Suspended = "suspended";
        public static readonly string BarberCancelled = "barberCancelled";
        public static readonly string CustomerCancelled = "customerCancelled";
        public static readonly string CheckedOut = "checkedOut";

        private static readonly double CustomerLatencyPercentage = 0.15;
        public static readonly double BarberLatencyPercentage = 0.3;
        
        public static readonly string[] States = {Booked, CheckedIn, Suspended, BarberCancelled, CustomerCancelled, CheckedOut};
        
        public static bool CheckStateChanged(this Bookings booking, DateTimeOffset time)
        {
            DateTimeOffset suspendedPoint = booking.BookedTime.AddMinutes(booking.DurationMinute * CustomerLatencyPercentage);
            if (booking.State == Booked && time > suspendedPoint)
            {
                booking.State = Suspended;
                return true;
            }

            return false;
        }

        public static DateTimeOffset GetFinishTime(this Bookings booking)
        {
            return booking.BookedTime.AddMinutes(booking.DurationMinute);
        }

        public static bool IsCancellable(this Bookings booking)
        {
            return booking.State == Booked || booking.State == Suspended || booking.State == CheckedIn;
        }

        public static void RestrictDataField(this Bookings booking)
        {
            if (booking.Customer != null)
            {
                booking.Customer.Bookings = null;
                booking.Customer.CustomerLocations = null;
            }

            if (booking.Barber != null)
            {
                booking.Barber.BarberServices = null;
                booking.Barber.Bookings = null;
            }

            if (booking.BookingServices != null)
            {
                foreach (var bookingService in booking.BookingServices)
                {
                    if (bookingService?.Service != null)
                    {
                        bookingService.Service.BookingServices = null;
                        bookingService.Service.Barber = null;
                    }
                }
            }
        }

        public static Slot ToSlot(this Bookings booking)
        {
            return new Slot(new TimePoint(booking.BookedTime), booking.DurationMinute);
        }
    }
}