using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IRefPhotoTypeRepository : IRepository<RefPhotoTypes>
    {
        
    }

    public class RefPhotoTypeRepository : Repository<RefPhotoTypes>, IRefPhotoTypeRepository
    {
        public RefPhotoTypeRepository(PBSAContext context) : base(context)
        {
        }
    }
}