using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefPaymentTypeRepository : IRepository<RefPaymentTypes>
    {
        
    }

    public class RefPaymentTypeRepository : Repository<RefPaymentTypes>, IRefPaymentTypeRepository
    {
        public RefPaymentTypeRepository(PBSAContext context) : base(context)
        {
        }
    }
}