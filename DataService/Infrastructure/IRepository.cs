using DataService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataService.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        T GetById(int id);

        T Find(Expression<Func<T, bool>> match);

        T Add(T entity);
        
        TE AddAny<TE>(TE entity) where TE : class;

        int AddBulk(List<T> entities);

        T AddOrUpdate(Expression<Func<T, bool>> match, T entity);

        T Update(T updated);
        
        TE UpdateAny<TE>(TE entity) where TE : class;
        
        int UpdateBulk(List<T> entities);

        void Delete(T t);
        
        void DeleteAny<TE>(TE entity) where TE : class;
        
        int DeleteBulk(List<T> entities);

        int Count();

        bool Exist(Expression<Func<T, bool>> predicate);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PBSAContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly int BulkSize = 1000; //https://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework

        public Repository(PBSAContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }


        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public TE AddAny<TE>(TE entity) where TE : class
        {
            _context.Set<TE>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public int AddBulk(List<T> entities)
        {
            if (entities == null)
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _context.Set<T>().Add(entity);

                if ((i + 1) % BulkSize == 0)
                {
                    count += _context.SaveChanges();
                }
            }
            count += _context.SaveChanges();

            return count;
        }

        public T AddOrUpdate(Expression<Func<T, bool>> match, T entity)
        {
            return Exist(match) ? Update(entity) : Add(entity);
        }


        public T Update(T updated)
        {
            if (updated == null)
            {
                return null;
            }

            _context.Set<T>().Attach(updated);
            _context.Entry(updated).State = EntityState.Modified;
            _context.SaveChanges();

            return updated;
        }

        public TE UpdateAny<TE>(TE entity) where TE : class
        {
            if (entity == null)
            {
                return null;
            }

            _context.Set<TE>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

            return entity;
        }

        public int UpdateBulk(List<T> entities)
        {
            if (entities == null)
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                if ((i + 1) % BulkSize == 0)
                {
                    count += _context.SaveChanges();
                }
            }
            count += _context.SaveChanges();

            return count;
        }

        public void Delete(T t)
        {
            _context.Set<T>().Remove(t);
            _context.SaveChanges();
        }

        public void DeleteAny<TE>(TE entity) where TE : class
        {
            _context.Set<TE>().Remove(entity);
            _context.SaveChanges();
        }

        public int DeleteBulk(List<T> entities)
        {
            if (entities == null)
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _context.Set<T>().Remove(entity);

                if ((i + 1) % BulkSize == 0)
                {
                    count += _context.SaveChanges();
                }
            }

            count += _context.SaveChanges();

            return count;
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public bool Exist(Expression<Func<T, bool>> predicate)
        {
            var exist = _context.Set<T>().Where(predicate);
            return exist.Any() ? true : false;
        }
    }
}
