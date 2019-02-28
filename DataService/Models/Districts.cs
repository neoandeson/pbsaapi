using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Districts
    {
        public Districts()
        {
            Barbers = new HashSet<Barbers>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string CitiCode { get; set; }

        public virtual Cities CitiCodeNavigation { get; set; }
        public virtual ICollection<Barbers> Barbers { get; set; }
    }
}
