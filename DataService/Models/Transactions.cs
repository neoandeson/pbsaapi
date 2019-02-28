using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Transactions
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public string State { get; set; }
        public DateTimeOffset Time { get; set; }
        public string SenderName { get; set; }
        public int SenderPaymentMethodId { get; set; }
        public string ReceiverName { get; set; }
        public int ReceiverPaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Detail { get; set; }

        public virtual Bookings Booking { get; set; }
        public virtual RefCurrencyCodes CurrencyCodeNavigation { get; set; }
        public virtual PaymentMethods ReceiverPaymentMethod { get; set; }
        public virtual PaymentMethods SenderPaymentMethod { get; set; }
        public virtual RefTransactionStates StateNavigation { get; set; }
    }
}
