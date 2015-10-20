using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryExecutor
    {
        private readonly IInternalAsyncDataStore asyncDataStore;
        private readonly IInternalSyncDataStore syncDataStore;

        public CollectionResourceQueryExecutor(IInternalDataStore dataStore)
        {
            this.asyncDataStore = dataStore as IInternalAsyncDataStore;
            this.syncDataStore = dataStore as IInternalSyncDataStore;
        }

        public Task<CollectionResponsePage<T>> ExecuteCollectionAsync<T>(string href, CancellationToken cancellationToken)
            => this.asyncDataStore.GetCollectionAsync<T>(href, cancellationToken);

        public CollectionResponsePage<T> ExecuteCollection<T>(string href)
            => this.syncDataStore.GetCollection<T>(href);
    }
}
