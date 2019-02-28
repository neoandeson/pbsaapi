using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.ViewModels
{
    public class TransactionViewModel
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
    }
}
