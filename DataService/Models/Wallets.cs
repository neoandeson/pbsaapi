using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Wallets
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public bool InUsed { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public DateTimeOffset? ExpiredTime { get; set; }

        public virtual RefCurrencyCodes CurrencyCodeNavigation { get; set; }
        public virtual PaymentMethods IdNavigation { get; set; }
    }
}
