using Store.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Interfaces
{
    public interface IUnitWork
    {
        // h3ml fun. hya ali htkon masola 3la anha taagbli objext mn el haga ali btlobha
        IGenericRepository<TEntity,Tkey> Repository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>;
        Task<int> CompleteAsync();
    }
}
