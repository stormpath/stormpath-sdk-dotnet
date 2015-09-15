using System.Linq.Expressions;

namespace Stormpath.SDK.Linq
{
    public interface IAsyncQueryProvider<T>
    {
        IAsyncQueryable<T> CreateQuery(Expression expression);
    }
}