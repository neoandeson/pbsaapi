using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IBookingServiceRepository : IRepository<BookingServices>
    {
        
    }

    public class BookingServiceRepository : Repository<BookingServices>, IBookingServiceRepository
    {
        public BookingServiceRepository(PBSAContext context) : base(context)
        {
        }
    }
}