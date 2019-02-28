using System;

namespace DataService.ViewModels
{
    public class BarberServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BarberId { get; set; }
        public int DurationMinute { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
    }
}