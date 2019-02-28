using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class RefPhotoTypes
    {
        public RefPhotoTypes()
        {
            Photos = new HashSet<Photos>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Photos> Photos { get; set; }
    }
}
