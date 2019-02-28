using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class CustomerLocations
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public int BookingId { get; set; }
        public double GpsLat { get; set; }
        public double GpsLong { get; set; }
        public DateTimeOffset Time { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual Customers Customer { get; set; }
    }
}
