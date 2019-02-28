using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Cities
    {
        public Cities()
        {
            Barbers = new HashSet<Barbers>();
            Districts = new HashSet<Districts>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Barbers> Barbers { get; set; }
        public virtual ICollection<Districts> Districts { get; set; }
    }
}
