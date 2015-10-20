using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryProvider<TResult> : IAsyncQueryProvider<TResult>, IQueryProvider
    {
        private CollectionResourceExecutor<TResult> executor;

        public CollectionResourceQueryProvider(CollectionResourceExecutor<TResult> executor)
        {
            this.executor = executor;
        }

        public CollectionResourceExecutor<TResult> Executor
        {
            get { return this.executor; }
            set { this.executor = value; }
        }

        // Collection-returning standard query operators call this method.
        IAsyncQueryable<TResult> IAsyncQueryProvider<TResult>.CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable<T> CreateQuery<T>(Expression expression)
            => new CollectionResourceQueryable<T>(this as IAsyncQueryProvider<T>, expression);

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(Expression expression)
        {
            bool isEnumerable = typeof(T).Name == "IEnumerable`1";

            //var compiledModel = 

            //return (TResult)SimpleExecutor.Execute(expression, IsEnumerable);
            throw new NotImplementedException();
        }
    }
}
