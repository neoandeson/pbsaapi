using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefComplaintStateRepository : IRepository<RefComplaintStates>
    {
        
    }

    public class RefComplaintStateRepository : Repository<RefComplaintStates>, IRefComplaintStateRepository
    {
        public RefComplaintStateRepository(PBSAContext context) : base(context)
        {
        }
    }
}