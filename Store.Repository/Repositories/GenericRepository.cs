using Microsoft.EntityFrameworkCore;
using Store.Data.Contexts;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using Store.Repository.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _context;

        public GenericRepository( StoreDbContext Context)
        {
            _context = Context;
        }
        public async Task AddAsync(TEntity entity)
        => await _context.Set<TEntity>().AddAsync(entity);

        public void DeleteAsync(TEntity entity)
        =>  _context.Set<TEntity>().Remove(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsNoTrackingAsync()
        => await _context.Set<TEntity>().AsNoTracking().ToListAsync();

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        => await _context.Set<TEntity>().ToListAsync();

        //public async Task<TEntity> GetByIdAsNoTrackingAsync(TKey? id)
        //{
        //    if (id == null)
        //    {
        //        return null; 
        //    }
        //    return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(product => product.Id == id.Value);
        //}

        public async Task<TEntity> GetByIdAsync(TKey? id)
        => await _context.Set<TEntity>().FindAsync(id);
 
        public void UpdateAsync(TEntity entity)
        => _context.Set<TEntity>().Update(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecificationAsync(ISpecification<TEntity> specification)
        => await ApplySpecification(specification).ToListAsync();
        public async Task<TEntity> GetWithSpecificationByIdAsync(ISpecification<TEntity> specification)
        => await ApplySpecification(specification).FirstOrDefaultAsync();

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        => SpecificationEvaluater<TEntity, TKey>.GetQuery(_context.Set<TEntity>(), specification);

        public async Task<int> GetCountSpecificationAsync(ISpecification<TEntity> specification)
        => await ApplySpecification(specification).CountAsync();
    }
  
}

