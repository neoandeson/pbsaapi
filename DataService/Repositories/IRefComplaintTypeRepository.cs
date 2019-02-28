using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefComplaintTypeRepository : IRepository<RefComplaintTypes>
    {
        
    }

    public class RefComplaintTypeRepository : Repository<RefComplaintTypes>, IRefComplaintTypeRepository
    {
        public RefComplaintTypeRepository(PBSAContext context) : base(context)
        {
        }
    }
}