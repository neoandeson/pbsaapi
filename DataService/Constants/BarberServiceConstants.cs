using DataService.Models;

namespace DataService.Constants
{
    public static class BarberServiceConstants
    {
        public static bool EqualsTo(this BarberServices service1, BarberServices service2)
        {
            return service1.InUsed
                   && service2.InUsed
                   && service1.Name == service2.Name
                   && service1.DurationMinute == service2.DurationMinute
                   && service1.Price == service2.Price
                   && service1.Currency == service2.Currency;
        }
    }
}