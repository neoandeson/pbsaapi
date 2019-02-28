using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class BarberServices
    {
        public BarberServices()
        {
            BookingServices = new HashSet<BookingServices>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string BarberId { get; set; }
        public int DurationMinute { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public bool InUsed { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public DateTimeOffset? ExpiredTime { get; set; }

        public virtual Barbers Barber { get; set; }
        public virtual ICollection<BookingServices> BookingServices { get; set; }
    }
}
