using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefTransactionStates
    {
        public RefTransactionStates()
        {
            Transactions = new HashSet<Transactions>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
