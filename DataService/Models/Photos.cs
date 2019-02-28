using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Photos
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public string Type { get; set; }
        public bool InUsed { get; set; }

        public virtual RefPhotoTypes TypeNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
