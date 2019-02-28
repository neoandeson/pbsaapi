using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface ICityRepository : IRepository<Cities>
    {
        
    }

    public class CityRepository : Repository<Cities>, ICityRepository
    {
        public CityRepository(PBSAContext context) : base(context)
        {
        }
    }
}