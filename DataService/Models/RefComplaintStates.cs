using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefComplaintStates
    {
        public RefComplaintStates()
        {
            Complaints = new HashSet<Complaints>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Complaints> Complaints { get; set; }
    }
}
