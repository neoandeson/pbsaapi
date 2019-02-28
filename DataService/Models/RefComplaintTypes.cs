using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefComplaintTypes
    {
        public RefComplaintTypes()
        {
            Complaints = new HashSet<Complaints>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }

        public virtual ICollection<Complaints> Complaints { get; set; }
    }
}
