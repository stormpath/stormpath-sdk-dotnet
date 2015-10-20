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
        private readonly CollectionResourceQueryExecutor executor;

        public CollectionResourceQueryProvider(string collectionHref, IInternalDataStore dataStore)
        {
            this.collectionHref = collectionHref;
            this.dataStore = dataStore;
            this.executor = new CollectionResourceQueryExecutor(dataStore);
        }

        public string CollectionHref => this.collectionHref;

        // Collection-returning standard query operators call this method.
        IAsyncQueryable<TResult> IAsyncQueryProvider<TResult>.CreateQuery(Expression expression)
        {
            return new CollectionResourceQueryable<TResult>(this, expression);
        }

        public Task<CollectionResponsePage<T>> ExecuteCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public CollectionResponsePage<T> ExecuteCollection<T>(string href)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        // Queryable's "single value" standard query operators call this method.
        // It is also called from QueryableTerraServerData.GetEnumerator().
        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = typeof(TResult).Name == "IEnumerable`1";

            //return (TResult)SimpleExecutor.Execute(expression, IsEnumerable);
            throw new NotImplementedException();
        }


    }
}
