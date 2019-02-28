using DataService.Infrastructure;
using DataService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Repositories
{
    public interface ITransactionRepository : IRepository<Transactions>
    {

    }

    public class TransactionRepository : Repository<Transactions>, ITransactionRepository
    {
        public TransactionRepository(PBSAContext context) : base(context)
        {

        }
    }
}
