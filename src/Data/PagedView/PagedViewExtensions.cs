using System.Linq;

namespace Kurilov.Data.PagedView
{
    public static class PagedViewExtensions
    {
        public static IPagedViewBuilder<TEntity> PagedView<TEntity>(this IQueryable<TEntity> entities, int pageSize)
            where TEntity : class
        {
            return new PagedViewBuilder<TEntity>(entities, pageSize);
        }
    }
}
