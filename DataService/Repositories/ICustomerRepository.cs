using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface ICustomerRepository : IRepository<Customers>
    {
        
    }

    public class CustomerRepository : Repository<Customers>, ICustomerRepository
    {
        public CustomerRepository(PBSAContext context) : base(context)
        {
        }
    }
}