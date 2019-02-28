using DataService.Infrastructure;
using DataService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Repositories
{
    public interface IBarberRepository : IRepository<Barbers>
    {
    }

    public class BarberRepository : Repository<Barbers>, IBarberRepository
    {
        public BarberRepository(PBSAContext context) : base(context)
        {

        }
    }
}
