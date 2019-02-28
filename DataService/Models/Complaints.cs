using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Complaints
    {
        public Complaints()
        {
            ComplaintImages = new HashSet<ComplaintImages>();
        }

        public int Id { get; set; }
        public int? BookingId { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }
        public string StaffId { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public DateTimeOffset? ResolvedTime { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual AspNetUsers Staff { get; set; }
        public virtual RefComplaintStates StateNavigation { get; set; }
        public virtual RefComplaintTypes TypeNavigation { get; set; }
        public virtual ICollection<ComplaintImages> ComplaintImages { get; set; }
    }
}
