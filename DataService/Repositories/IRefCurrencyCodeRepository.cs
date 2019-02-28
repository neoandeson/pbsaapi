using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefCurrencyCodeRepository : IRepository<RefCurrencyCodes>
    {
        
    }

    public class RefCurrencyCodeRepository : Repository<RefCurrencyCodes>, IRefCurrencyCodeRepository
    {
        public RefCurrencyCodeRepository(PBSAContext context) : base(context)
        {
        }
    }
}