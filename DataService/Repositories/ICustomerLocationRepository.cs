using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface ICustomerLocationRepository : IRepository<CustomerLocations>
    {
        
    }

    public class CustomerLocationRepository : Repository<CustomerLocations>, ICustomerLocationRepository
    {
        public CustomerLocationRepository(PBSAContext context) : base(context)
        {
        }
    }
}