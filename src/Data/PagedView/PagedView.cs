using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kurilov.Data.PagedView
{
    internal class PagedView<TEntity> : IPagedView<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> entities;
        private readonly int pageSize;
        private readonly List<Key> keys;

        internal PagedView(
            IQueryable<TEntity> entities,
            int pageSize,
            List<Key> keys)
        {
            this.entities = entities;
            this.pageSize = pageSize;
            this.keys = keys;
        }
        public IQueryable<TEntity> Page<TKey>(TKey after)
        {
            return this
                .Sort(this.After(this.entities, after))
                .Take(this.pageSize);
        }

        private IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> entities)
        {
            if (this.keys == null || this.keys.Count == 0)
            {
                throw new ArgumentException("Keys should not be empty");
            }
            IOrderedQueryable<TEntity> orderedEntities = entities.OrderBy(keys[0].Member, keys[0].SortDirection);
            foreach (var key in keys.Skip(1))
            {
                orderedEntities = orderedEntities.ThenBy(key.Member, key.SortDirection);
            }
            return orderedEntities;
        }

        private IQueryable<TEntity> After<TKey>(IQueryable<TEntity> entities, TKey after)
        {
            var keyType = typeof(TKey);
            BinaryExpression? last = null;
            var inParam = Expression.Parameter(typeof(TEntity));
            foreach (var key in this.keys.AsEnumerable().Reverse())
            {
                var keyProp = keyType.GetProperty(key.Member.Name);
                if (keyProp != null)
                {
                    var constKey = Expression.Constant(keyProp.GetValue(after));
                    var extractProperty = Expression.PropertyOrField(inParam, keyProp.Name);
                    var afterOp = key.SortDirection == SortDirection.Ascending ?
                        Expression.GreaterThan(extractProperty, constKey) :
                        Expression.LessThan(extractProperty, constKey);
                    if (last == null)
                    {
                        last = afterOp;
                    }
                    else
                    {
                        var equalOp = Expression.Equal(extractProperty, constKey);
                        last = Expression.Or(afterOp, Expression.And(equalOp, last));
                    }
                }
            }

            if (last != null)
            {
                return entities.Where(Expression.Lambda<Func<TEntity, bool>>(last, inParam));
            }
            else
            {
                return entities;
            }
        }
    }
}
