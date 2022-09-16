using System.Linq.Expressions;

namespace Kurilov.Data.PagedView
{
    public interface IPagedViewBuilder<TEntity> where TEntity : class
    {
        IPagedView<TEntity> Build();

        IPagedViewBuilder<TEntity> Key<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, SortDirection sortDirection = SortDirection.Ascending);
    }
}