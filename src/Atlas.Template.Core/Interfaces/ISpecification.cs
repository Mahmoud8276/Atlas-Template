using Atlas.Template.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Atlas.Template.Core.Interfaces
{
    public interface ISpecification<TModel, TKey> where TModel : BaseModel<TKey>
    {
        public Expression<Func<TModel, bool>> Criteria { get; }
        public List<Expression<Func<TModel, object>>> Includes { get; }
        public List<string> StringIncludes { get; }
        public Expression<Func<TModel, object>> OrderBy { get; }
        public Expression<Func<TModel, object>> OrderByDesc { get; }

        public int Skip { get; }
        public int Take { get; }
        public int Count { get; }
        public bool IsPagination { get; }
    }
}
