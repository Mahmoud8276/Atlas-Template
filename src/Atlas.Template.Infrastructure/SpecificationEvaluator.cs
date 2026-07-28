using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Atlas.Template.Infrastructure
{
    public static class SpecificationEvaluator<TModel, TKey> where TModel : BaseModel<TKey>
    {
        public static IQueryable<TModel> GetQuery(IQueryable<TModel> inputQuery, ISpecification<TModel, TKey> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDesc != null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }
            if(spec.Includes != null)
            {
                query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (spec.StringIncludes != null)
            {
                query = spec.StringIncludes.Aggregate(query, (current, include) => current.Include(include));
            }
            if(spec.IsPagination)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            return query;
        }
    }
}
