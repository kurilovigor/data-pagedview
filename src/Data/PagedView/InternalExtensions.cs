using System.Linq.Expressions;
using System.Reflection;

namespace Kurilov.Data.PagedView
{
    internal static class InternalExtensions
    {
        internal static MemberInfo MemberInfo(this Type type, Expression memberSelector)
        {
            if (memberSelector == null)
            {
                throw new ArgumentException("Expression can not be null");
            }

            if (memberSelector is LambdaExpression lambdaExpression)
            {
                if (lambdaExpression.Body is MemberExpression memberExpression)
                {
                    if (memberExpression.Member.MemberType == MemberTypes.Property ||
                        memberExpression.Member.MemberType == MemberTypes.Field)
                    {
                        return memberExpression.Member;
                    }
                }
            }
            throw new ArgumentException("Invalid expression");
        }

        internal static LambdaExpression MemberSelector<TEntity>(MemberInfo memberInfo)
            where TEntity : class
        {
            var inParamRecord = Expression.Parameter(typeof(TEntity));
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var extractProperty = Expression.Property(inParamRecord, (PropertyInfo)memberInfo);
                return Expression.Lambda(extractProperty, inParamRecord);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                var extractField = Expression.Field(inParamRecord, (FieldInfo)memberInfo);
                return Expression.Lambda(extractField, inParamRecord);
            }
            else
            {
                throw new ArgumentException("Member should be property of field");
            }
        }

        internal static Type MemberType(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)memberInfo).PropertyType;
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)memberInfo).FieldType;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> entities, MemberInfo memberInfo, SortDirection sortDirection)
            where TEntity : class
        {
            var memberType = MemberType(memberInfo);
            var query = Expression.Call(
                typeof(Queryable),
                sortDirection == SortDirection.Ascending ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(TEntity), memberType },
                entities.Expression,
                Expression.Quote(MemberSelector<TEntity>(memberInfo)));
            return (IOrderedQueryable<TEntity>)entities.Provider.CreateQuery<TEntity>(query);
        }

        internal static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> entities, MemberInfo memberInfo, SortDirection sortDirection)
            where TEntity : class
        {
            var memberType = MemberType(memberInfo);
            var query = Expression.Call(
                typeof(Queryable),
                sortDirection == SortDirection.Ascending ? "ThenBy" : "ThenByDescending",
                new Type[] { typeof(TEntity), memberType },
                entities.Expression,
                Expression.Quote(MemberSelector<TEntity>(memberInfo)));
            return (IOrderedQueryable<TEntity>)entities.Provider.CreateQuery<TEntity>(query);
        }
    }
}
