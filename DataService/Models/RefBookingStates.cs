using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefBookingStates
    {
        public RefBookingStates()
        {
            BookingHistory = new HashSet<BookingHistory>();
            Bookings = new HashSet<Bookings>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BookingHistory> BookingHistory { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
    }
}
