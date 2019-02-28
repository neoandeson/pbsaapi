using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class BookingServices
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ServiceId { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual BarberServices Service { get; set; }
    }
}
