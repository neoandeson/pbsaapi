using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefCurrencyCodes
    {
        public RefCurrencyCodes()
        {
            Transactions = new HashSet<Transactions>();
            Wallets = new HashSet<Wallets>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Transactions> Transactions { get; set; }
        public virtual ICollection<Wallets> Wallets { get; set; }
    }
}
