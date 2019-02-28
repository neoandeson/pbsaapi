using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class ComplaintImages
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int? ComplaintId { get; set; }

        public virtual Complaints Complaint { get; set; }
    }
}
