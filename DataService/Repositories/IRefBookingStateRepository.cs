using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefBookingStateRepository : IRepository<RefBookingStates>
    {
        
    }

    public class RefBookingStateRepository : Repository<RefBookingStates>, IRefBookingStateRepository
    {
        public RefBookingStateRepository(PBSAContext context) : base(context)
        {
        }
    }
}