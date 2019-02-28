using System;
using System.Collections.Generic;
using System.Text;
using DataService.Models;

namespace DataService.ViewModels
{
    public class CustomerViewModel
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public ICollection<PaymentMethods> PaymentMethods { get; set; }
    }
}
