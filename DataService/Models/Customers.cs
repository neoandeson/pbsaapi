using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Customers
    {
        public Customers()
        {
            Bookings = new HashSet<Bookings>();
            CustomerLocations = new HashSet<CustomerLocations>();
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual ICollection<CustomerLocations> CustomerLocations { get; set; }
    }
}
