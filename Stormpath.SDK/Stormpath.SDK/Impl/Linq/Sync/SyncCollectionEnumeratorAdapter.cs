// <copyright file="SyncCollectionEnumeratorAdapter.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Stormpath.SDK.Impl.Resource;
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
