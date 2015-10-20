using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq.Sync
{
    internal sealed class SyncCollectionEnumeratorAdapter<T> : IEnumerable<T>
    {
        private readonly CollectionResourceQueryable<T> collection;
        private readonly CancellationToken cancellationToken;

        public SyncCollectionEnumeratorAdapter(CollectionResourceQueryable<T> collection, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.collection = collection;
            this.cancellationToken = cancellationToken;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (this.collection.MoveNext())
            {
                this.cancellationToken.ThrowIfCancellationRequested();

                foreach (var item in (this.collection as IAsyncQueryable<T>).CurrentPage)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
