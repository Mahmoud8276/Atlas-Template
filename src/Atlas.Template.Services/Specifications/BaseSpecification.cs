using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Atlas.Template.Services.Specifications
{
    public abstract class BaseSpecification<TModel, TKey> : ISpecification<TModel, TKey> where TModel : BaseModel<TKey>
    {
        public BaseSpecification() { }
        public BaseSpecification(Expression<Func<TModel, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<TModel, bool>> Criteria { get; }
        public List<Expression<Func<TModel, object>>> Includes { get; } = new List<Expression<Func<TModel, object>>>();
        public List<string> StringIncludes { get; } = new List<string>();
        public Expression<Func<TModel, object>> OrderBy { get; private set; }
        public Expression<Func<TModel, object>> OrderByDesc { get; private set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }
        public int Count { get; private set; }
        public bool IsPagination { get; private set; }


        protected void AddInclude(Expression<Func<TModel, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeExpression)
        {
            StringIncludes.Add(includeExpression);
        }

        protected void AddOrderBy(Expression<Func<TModel, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDesc(Expression<Func<TModel, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }

        protected void ApplyPagination(int skip, int take)
        {
            IsPagination = true;
            Skip = skip;
            Take = take;
        }
    }
}
