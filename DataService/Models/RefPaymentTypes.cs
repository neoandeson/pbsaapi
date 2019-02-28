using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefPaymentTypes
    {
        public RefPaymentTypes()
        {
            Bookings = new HashSet<Bookings>();
            PaymentMethods = new HashSet<PaymentMethods>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual ICollection<PaymentMethods> PaymentMethods { get; set; }
    }
}
