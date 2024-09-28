using Microsoft.EntityFrameworkCore;
using Store.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specification
{
    public class SpecificationEvaluater<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        #region Notes
        //IEnumable VS IQueryable (both are collection of data)
        // IQueryable the query byhslha execution 3nd el data source(db) + ay remote data source(excel , etc..)
        // aghlb astghdmtha filtration , Sorting ,pagination (server side)
        // IEnumable ali hwa 3aml list,dic ad eh el hagat di (memory b3d ma y-load el data  + client side)
        // ka performance fi enum msh ahsn haga lw bt3ml ma large dataset
        // iquery hyb'a helw ma large dataset    
        // l2e link to entities or sql
        #endregion
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecification<TEntity> specification )
        {
            var query = inputQuery;
            if (specification.Criteria is not null)
                query = query.Where(specification.Criteria);

            if (specification.OrderBy is not null)
                query = query.OrderBy(specification.OrderBy);

            if (specification.OrderByDescending is not null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if(specification.IsPaginated)
                query = query.Skip(specification.Skip).Take(specification.Take);

            query = specification.Includes.Aggregate(query, (current, inculdeExpression) => current.Include(inculdeExpression));
            return query;
        }
    }
}
