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
        private readonly string collectionHref;
        private readonly IInternalDataStore dataStore;
        private readonly CollectionResourceExecutor executor;

        public CollectionResourceQueryProvider(string collectionHref, IInternalDataStore dataStore)
        {
            this.collectionHref = collectionHref;
            this.dataStore = dataStore;
            this.executor = new CollectionResourceExecutor(dataStore);
        }

        public string CollectionHref => this.collectionHref;

        // Collection-returning standard query operators call this method.
        IAsyncQueryable<TResult> IAsyncQueryProvider<TResult>.CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable CreateQuery(Expression expression)
            => new CollectionResourceQueryable<TResult>(this, expression);

        public IQueryable<T> CreateQuery<T>(Expression expression)
            => new CollectionResourceQueryable<T>(this as IAsyncQueryProvider<T>, expression);

        public Task<CollectionResponsePage<T>> ExecuteCollectionAsync<T>(string href, CancellationToken cancellationToken)
            => this.executor.ExecuteCollectionAsync<T>(href, cancellationToken);

        public CollectionResponsePage<T> ExecuteCollection<T>(string href)
            => this.executor.ExecuteCollection<T>(href);

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(Expression expression)
        {
            bool isEnumerable = typeof(T).Name == "IEnumerable`1";

            var compiledModel = 

            //return (TResult)SimpleExecutor.Execute(expression, IsEnumerable);
            throw new NotImplementedException();
        }
    }
}
