using DataService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.ViewModels
{
    public class FeedbackViewModel
    {
        public string Customer { get; set; }
        public string Barber { get; set; }
        public double? Rate { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset Time { get; set; }
        public Bookings Booking { get; set; }
    }
}
