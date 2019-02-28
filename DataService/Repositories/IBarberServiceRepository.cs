using System.Collections.Generic;
using System.Linq;
using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IBarberServiceRepository : IRepository<BarberServices>
    {
        List<BarberServices> GetServicesByBarber(string barberId);
    }

    public class BarberServiceRepository : Repository<BarberServices>, IBarberServiceRepository
    {
        public BarberServiceRepository(PBSAContext context) : base(context)
        {
        }

        public List<BarberServices> GetServicesByBarber(string barberId)
        {
            List<BarberServices> result = GetAll()
                .Where(service => service.BarberId == barberId && service.InUsed)
                .ToList();

            return result;
        }
    }
}