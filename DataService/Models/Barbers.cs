using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Barbers
    {
        public Barbers()
        {
            BarberSchedules = new HashSet<BarberSchedules>();
            BarberServices = new HashSet<BarberServices>();
            Bookings = new HashSet<Bookings>();
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public double? OverallRate { get; set; }
        public int? RatingCount { get; set; }
        public string CoverImage { get; set; }
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string DistrictCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public bool IsActive { get; set; }

        public virtual Cities CityCodeNavigation { get; set; }
        public virtual Districts DistrictCodeNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<BarberSchedules> BarberSchedules { get; set; }
        public virtual ICollection<BarberServices> BarberServices { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
    }
}
