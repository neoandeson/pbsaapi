using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefTransactionStateRepository : IRepository<RefTransactionStates>
    {
        
    }

    public class RefTransactionStateRepository : Repository<RefTransactionStates>, IRefTransactionStateRepository
    {
        public RefTransactionStateRepository(PBSAContext context) : base(context)
        {
        }
    }
}