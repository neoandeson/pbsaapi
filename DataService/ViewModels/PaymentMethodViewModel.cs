using DataService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.ViewModels
{
    public class PaymentMethodViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PaymentType { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public bool InUsed { get; set; }
        public bool IsDefault { get; set; }
        public virtual Wallets Wallets { get; set; }
    }
}
