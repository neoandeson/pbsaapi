using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class BookingHistory
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTimeOffset BookedTime { get; set; }
        public DateTimeOffset TransitionTime { get; set; }
        public string State { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual RefBookingStates StateNavigation { get; set; }
    }
}
