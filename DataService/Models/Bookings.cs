using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Bookings
    {
        public Bookings()
        {
            BookingHistory = new HashSet<BookingHistory>();
            BookingServices = new HashSet<BookingServices>();
            Complaints = new HashSet<Complaints>();
            CustomerLocations = new HashSet<CustomerLocations>();
            Transactions = new HashSet<Transactions>();
        }

        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string BarberId { get; set; }
        public DateTimeOffset BookedTime { get; set; }
        public int DurationMinute { get; set; }
        public string State { get; set; }
        public string CustomerPaymentType { get; set; }
        public double? Rate { get; set; }
        public string Comment { get; set; }

        public virtual Barbers Barber { get; set; }
        public virtual Customers Customer { get; set; }
        public virtual RefPaymentTypes CustomerPaymentTypeNavigation { get; set; }
        public virtual RefBookingStates StateNavigation { get; set; }
        public virtual ICollection<BookingHistory> BookingHistory { get; set; }
        public virtual ICollection<BookingServices> BookingServices { get; set; }
        public virtual ICollection<Complaints> Complaints { get; set; }
        public virtual ICollection<CustomerLocations> CustomerLocations { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
