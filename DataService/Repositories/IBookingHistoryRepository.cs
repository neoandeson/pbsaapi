using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IBookingHistoryRepository : IRepository<BookingHistory>
    {
    }

    public class BookingHistoryRepository : Repository<BookingHistory>, IBookingHistoryRepository
    {
        public BookingHistoryRepository(PBSAContext context) : base(context)
        {
        }
    }
}