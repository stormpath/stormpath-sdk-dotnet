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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Linq;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class CollectionResourceQueryable<T> : QueryableBase<T>, IOrderedAsyncQueryable<T>, IAsyncQueryProvider<T>
    {
        private readonly IQueryable<T> proxy;

        private readonly Expression expressionOverride;

        private readonly IInternalDataStore dataStore;

        private readonly string baseHref;

        private CollectionResourceRequestModel compiledModel = null;

        private int totalItemsRetrieved = 0;

        private int currentOffset;

        private int currentLimit;

        private int currentSize;

        private IEnumerable<T> currentItems;

        public CollectionResourceQueryable(string collectionHref, IInternalDataStore dataStore)
            : base(ExtendedQueryParser.Create(), CreateQueryExecutor(collectionHref, dataStore))
        {
            this.baseHref = collectionHref;
            this.dataStore = dataStore;
            this.proxy = CreateProxy();
            this.expressionOverride = null;
        }

        internal CollectionResourceQueryable(CollectionResourceQueryable<T> existing, IQueryable<T> updatedProxy)
            : this(existing.baseHref, existing.dataStore)
        {
            this.proxy = updatedProxy;
        }

        // This constructor is used for a synchronous wrapper via CollectionResourceQueryExecutor
        public CollectionResourceQueryable(string collectionHref, IInternalDataStore dataStore, CollectionResourceRequestModel existingRequestModel)
            : this(collectionHref, dataStore)
        {
            this.compiledModel = existingRequestModel;
            this.proxy = CreateProxy();
            this.expressionOverride = null;
        }

        // (This constructor is called internally by LINQ)
        public CollectionResourceQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
            var relinqProvider = provider as DefaultQueryProvider;
            var executor = relinqProvider?.Executor as CollectionResourceQueryExecutor;
            if (relinqProvider == null || executor == null)
                throw new InvalidOperationException("LINQ queries must start from a supported ICollectionResourceQueryable.");

            this.baseHref = executor.Href;
            this.dataStore = executor.DataStore;
            this.proxy = CreateProxy();
            this.expressionOverride = expression;
        }

        private static IQueryable<T> CreateProxy()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        private static IQueryExecutor CreateQueryExecutor(string href, IInternalDataStore dataStore)
        {
            return new CollectionResourceQueryExecutor(href, dataStore);
        }

        private IAsyncQueryable<T> AsAsyncQueryable => this;

        internal int Offset
        {
            get
            {
                bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return this.currentOffset;
            }
        }

        internal int Limit
        {
            get
            {
                bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return this.currentLimit;
            }
        }

        internal int Size
        {
            get
            {
                bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return this.currentSize;
            }
        }

        IEnumerable<T> IAsyncQueryable<T>.CurrentPage
        {
            get
            {
                bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return this.currentItems;
            }
        }

        internal string CurrentHref => this.GenerateRequestUrlFromModel();

        Expression IAsyncQueryable<T>.Expression
        {
            get
            {
                if (this.expressionOverride != null)
                    return this.expressionOverride;

                return this.proxy.Expression;
            }
        }

        IAsyncQueryProvider<T> IAsyncQueryable<T>.Provider => this;

        async Task<bool> IAsyncQueryable<T>.MoveNextAsync(CancellationToken cancellationToken)
        {
            if (this.compiledModel == null)
            {
                if (!this.CompileExpressionToRequestModel())
                {
                    // Use default model values
                    this.compiledModel = new CollectionResourceRequestModel();
                }
            }

            bool retrievedEnoughItems = this.totalItemsRetrieved >= this.compiledModel.ExecutionPlan.MaxItems;
            if (retrievedEnoughItems)
                return false;

            bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
            if (atLeastOnePageRetrieved)
            {
                if (!this.compiledModel.Offset.HasValue)
                    this.compiledModel.Offset = 0;
                this.compiledModel.Offset += this.currentItems.Count();
            }

            var url = this.GenerateRequestUrlFromModel();
            var result = await this.dataStore.GetCollectionAsync<T>(url, cancellationToken).ConfigureAwait(false);

            bool anyNewItems = result?.Items?.Any() ?? false;
            if (!anyNewItems)
                return false;

            this.currentOffset = result.Offset;
            this.currentLimit = result.Limit;
            this.currentSize = result.Size;
            this.currentItems = result.Items;

            this.totalItemsRetrieved += result.Items.Count;
            return true;
        }

        private bool CompileExpressionToRequestModel()
        {
            bool noExpressionToCompile = this.AsAsyncQueryable.Expression == null;
            if (noExpressionToCompile)
                return false;

            var queryModel = ExtendedQueryParser.Create().GetParsedQuery(this.AsAsyncQueryable.Expression);
            this.compiledModel = ParseQueryModelToRequestModel(queryModel);
            return true;
        }

        private static CollectionResourceRequestModel ParseQueryModelToRequestModel(QueryModel model)
        {
            var visitor = new CollectionResourceQueryModelVisitor();
            visitor.VisitQueryModel(model);
            return visitor.ParsedModel;
        }

        private string GenerateRequestUrlFromModel()
        {
            if (this.compiledModel == null)
                this.CompileExpressionToRequestModel();

            var argumentList = CollectionResourceRequestModelCompiler.GetArguments(this.compiledModel);
            if (!argumentList.Any())
                return this.baseHref;

            var arguments = string.Join("&", argumentList);
            return $"{this.baseHref}?{arguments}";
        }

        IAsyncQueryable<T> IAsyncQueryProvider<T>.CreateQuery(Expression expression)
        {
            return new CollectionResourceQueryable<T>(this, this.proxy.Provider.CreateQuery<T>(expression));
        }
    }
}
