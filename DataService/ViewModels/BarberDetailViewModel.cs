using System;
using System.Collections.Generic;
using System.Text;
using DataService.Models;

namespace DataService.ViewModels
{
    public class BarberDetailViewModel
    {
        public int? WalletId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public double? OverallRate { get; set; }
        public int? RatingCount { get; set; }
        public string CoverImage { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public bool IsActive { get; set; }
        public ICollection<PaymentMethods> PaymentMethods { get; set; }
    }
}
