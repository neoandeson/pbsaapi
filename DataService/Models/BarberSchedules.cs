using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class BarberSchedules
    {
        public int Id { get; set; }
        public string BarberId { get; set; }
        public int StartTimeMinute { get; set; }
        public int EndTimeMinute { get; set; }
        public string RepetitionType { get; set; }
        public string Repetition { get; set; }
        public bool InUsed { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public DateTimeOffset? ExpiredTime { get; set; }

        public virtual Barbers Barber { get; set; }
    }
}
