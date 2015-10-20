// <copyright file="CollectionResourceQueryable.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.QueryModel;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryable<TResult> : IOrderedAsyncQueryable<TResult>, IOrderedQueryable<TResult>
    {
        private readonly CollectionResourceExecutor<TResult> executor;
        private readonly CollectionResourceQueryProvider<TResult> queryProvider;
        private readonly Expression expression;

        public CollectionResourceQueryable(string collectionHref, IInternalDataStore dataStore)
        {
            this.expression = Expression.Constant(this);
            this.executor = new CollectionResourceExecutor<TResult>(collectionHref, dataStore, this.expression);
            this.queryProvider = new CollectionResourceQueryProvider<TResult>(this.executor);
        }

        // This constructor is called internally by LINQ
        public CollectionResourceQueryable(IAsyncQueryProvider<TResult> provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (expression == null)
                throw new ArgumentNullException("expression");

            var concreteProvider = provider as CollectionResourceQueryProvider<TResult>;
            if (concreteProvider == null)
                throw new InvalidOperationException("LINQ queries must start from a supported provider.");

            this.expression = expression;
            this.queryProvider = concreteProvider;
            this.executor = new CollectionResourceExecutor<TResult>(this.queryProvider.Executor, expression);
            this.queryProvider.Executor = this.executor;
        }

        Expression IAsyncQueryable<TResult>.Expression => this.expression;

        IAsyncQueryProvider<TResult> IAsyncQueryable<TResult>.Provider => this.queryProvider;

        Expression IQueryable.Expression => this.expression;

        Type IQueryable.ElementType => typeof(TResult);

        IQueryProvider IQueryable.Provider => this.queryProvider;

        IEnumerable<TResult> IAsyncQueryable<TResult>.CurrentPage
            => this.executor.CurrentPage;

        Task<bool> IAsyncQueryable<TResult>.MoveNextAsync(CancellationToken cancellationToken)
            => this.executor.MoveNextAsync(cancellationToken);

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
            => new Sync.SyncCollectionEnumeratorAdapter<TResult>(this.executor).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => new Sync.SyncCollectionEnumeratorAdapter<TResult>(this.executor).GetEnumerator();

        public string CurrentHref
            => this.executor.CurrentHref;

        public long Size
            => this.executor.Size;

        public long Offset
            => this.executor.Offset;

        public long Limit
            => this.executor.Limit;
    }
}
