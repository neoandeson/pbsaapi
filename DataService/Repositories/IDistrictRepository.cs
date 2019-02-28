using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IDistrictRepository : IRepository<Districts>
    {
        
    }

    public class DistrictRepository : Repository<Districts>, IDistrictRepository
    {
        public DistrictRepository(PBSAContext context) : base(context)
        {
        }
    }
}