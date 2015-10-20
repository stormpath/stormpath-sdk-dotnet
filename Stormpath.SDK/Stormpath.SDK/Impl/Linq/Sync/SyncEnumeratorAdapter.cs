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
    internal sealed class SyncCollectionEnumeratorAdapter<TResult> : IEnumerable<TResult>
    {
        private readonly CollectionResourceExecutor<TResult> executor;
        private readonly CancellationToken cancellationToken;

        public SyncCollectionEnumeratorAdapter(CollectionResourceExecutor<TResult> executor, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.executor = executor;
            this.cancellationToken = cancellationToken;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            while (this.executor.MoveNext())
            {
                this.cancellationToken.ThrowIfCancellationRequested();

                foreach (var item in this.executor.CurrentPage)
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
