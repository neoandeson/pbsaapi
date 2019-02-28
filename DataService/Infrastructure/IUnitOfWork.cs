using DataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Infrastructure
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> Commit();

        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly PBSAContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        public UnitOfWork(PBSAContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo = new Repository<T>(_dbContext);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public async Task<int> Commit()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }
    }
    //Scaffold-DbContext "Server=DESKTOP-UH7HU37\TIENTPSQL;Database=PBSA;User ID=sa;Password=1234;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force
}
