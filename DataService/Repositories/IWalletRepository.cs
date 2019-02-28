using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IWalletRepository : IRepository<Wallets>
    {
        
    }

    public class WalletRepository : Repository<Wallets>, IWalletRepository
    {
        public WalletRepository(PBSAContext context) : base(context)
        {
        }
    }
}