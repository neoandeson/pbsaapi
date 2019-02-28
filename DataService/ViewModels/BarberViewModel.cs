using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.ViewModels
{
    public class BarberViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public double? OverallRate { get; set; }
        //public int? RatingCount { get; set; }
        //public string CoverImage { get; set; }
        public string Address { get; set; }
        //public string City { get; set; }
        //public string District { get; set; }
        //public double? Longitude { get; set; }
        //public double? Latitude { get; set; }
    }
}
