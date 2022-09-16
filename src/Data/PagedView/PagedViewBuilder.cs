using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kurilov.Data.PagedView
{
    internal class PagedViewBuilder<TEntity> : IPagedViewBuilder<TEntity> where TEntity : class
    {
        private readonly List<Key> keys = new();
        private readonly IQueryable<TEntity> entities;
        private readonly int pageSize;

        internal PagedViewBuilder(IQueryable<TEntity> entities, int pageSize)
        {
            this.entities = entities;
            this.pageSize = pageSize;
        }

        public IPagedViewBuilder<TEntity> Key<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var key = new Key(typeof(TEntity).MemberInfo(propertySelector), sortDirection);
            this.keys.Add(key);
            return this;
        }

        public IPagedView<TEntity> Build()
        {
            return new PagedView<TEntity>(this.entities, this.pageSize, this.keys);
        }
    }
}
