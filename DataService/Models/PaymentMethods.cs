using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class PaymentMethods
    {
        public PaymentMethods()
        {
            TransactionsReceiverPaymentMethod = new HashSet<Transactions>();
            TransactionsSenderPaymentMethod = new HashSet<Transactions>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string PaymentType { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public bool InUsed { get; set; }
        public bool IsDefault { get; set; }

        public virtual RefPaymentTypes PaymentTypeNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual Wallets Wallets { get; set; }
        public virtual ICollection<Transactions> TransactionsReceiverPaymentMethod { get; set; }
        public virtual ICollection<Transactions> TransactionsSenderPaymentMethod { get; set; }
    }
}
