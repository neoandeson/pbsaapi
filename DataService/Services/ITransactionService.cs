using DataService.Models;
using DataService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataService.Services
{
    public interface ITransactionService
    {
        List<Transactions> GetTransactions();
        Transactions GetTransactionsInfo(int id);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            this._transactionRepository = transactionRepository;
        }

        public List<Transactions> GetTransactions()
        {
            List<Transactions> transactions = _transactionRepository.GetAll().ToList();
            return transactions;
        }

        public Transactions GetTransactionsInfo(int id)
        {
            Transactions transactions = _transactionRepository.GetById(id);
            return transactions;
        }
    }
}
