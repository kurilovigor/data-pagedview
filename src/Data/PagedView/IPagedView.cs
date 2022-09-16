namespace Kurilov.Data.PagedView
{
    public interface IPagedView<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Page<TKey>(TKey after);
    }
}