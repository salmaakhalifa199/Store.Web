using Store.Data.Contexts;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using System;
using System.Collections;


namespace Store.Repository.Repositories
{
    public class UnitWork : IUnitWork
    {
        private readonly StoreDbContext _context;
        private Hashtable _repositories;

        public UnitWork(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        => await _context.SaveChangesAsync();

        public IGenericRepository<TEntity, Tkey> Repository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>
        {
            #region Notes
            //hashtable => by3ml mapping li key,value
            // hastghdmo 34an yakhzam instance mn el repos. bt3t 34an ymn3 ani a-create new repos. instances kol mara ana btlob feha el data  bt3te
            #endregion

            if(_repositories is null) _repositories = new Hashtable();  
            var entityKey = typeof(TEntity).Name; //Product
            if (!_repositories.ContainsKey(entityKey))
            {
                var repositoryType = typeof(GenericRepository<,>); //GenericRepository<Product,int>
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity),typeof(Tkey)), _context);
                _repositories.Add(entityKey, repositoryInstance);
            }
            return (IGenericRepository<TEntity, Tkey>) _repositories[entityKey]; //_repositories[entityKey di btrag3 obj fa 34an kda 3mlt cast 
        }
    }
}
