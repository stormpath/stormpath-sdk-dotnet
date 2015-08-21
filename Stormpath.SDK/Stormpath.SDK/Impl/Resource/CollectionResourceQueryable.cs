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
    internal class CollectionResourceQueryable<T> : QueryableBase<T>, ICollectionResourceQueryable<T>
        //where T : class, IResource
    {
        private readonly Expression expression;

        private readonly IDataStore dataStore;

        private readonly string baseHref;

        private CollectionResourceRequestModel compiledModel = null;

        private int totalItemsRetrieved = 0;

        private int currentOffset;

        private int currentLimit;

        private int currentSize;

        private IEnumerable<T> currentItems;

        public CollectionResourceQueryable(string collectionHref, IDataStore dataStore)
            : base(ExtendedQueryParser.Create(), CreateQueryExecutor(collectionHref, dataStore))
        {
            this.baseHref = collectionHref;
            this.dataStore = dataStore;
        }

        // This constructor is used for a synchronous wrapper via CollectionResourceQueryExecutor
        // TODO make this more SOLID and have an actual synchronous execution path that isn't a hack or wrapper
        public CollectionResourceQueryable(string collectionHref, IDataStore dataStore, CollectionResourceRequestModel existingRequestModel)
            : this(collectionHref, dataStore)
        {
            this.compiledModel = existingRequestModel;
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
            this.expression = expression;
        }

        private static IQueryExecutor CreateQueryExecutor(string href, IDataStore dataStore)
        {
            return new CollectionResourceQueryExecutor(href, dataStore);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Grouping internal methods above")]
        int ICollectionResourceQueryable<T>.Offset
        {
            get
            {
                bool atLeastOnePageRetrieved = totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentOffset;
            }
        }

        int ICollectionResourceQueryable<T>.Limit
        {
            get
            {
                bool atLeastOnePageRetrieved = totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentLimit;
            }
        }

        int ICollectionResourceQueryable<T>.Size
        {
            get
            {
                bool atLeastOnePageRetrieved = totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentSize;
            }
        }

        IEnumerable<T> IAsyncQueryable<T>.CurrentPage
        {
            get
            {
                bool atLeastOnePageRetrieved = totalItemsRetrieved > 0;
                if (!atLeastOnePageRetrieved)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentItems; // TODO ?? Enumerable.Empty<T> ?
            }
        }

        string ICollectionResourceQueryable<T>.CurrentHref
        {
            get
            {
                return GenerateRequestUrlFromModel();
            }
        }

        async Task<bool> IAsyncQueryable<T>.MoveNextAsync(CancellationToken cancellationToken)
        {
            if (this.compiledModel == null)
            {
                if (!CompileExpressionToRequestModel())
                {
                    // Use default model values
                    this.compiledModel = new CollectionResourceRequestModel();
                }
            }

            bool retrievedEnoughItems = totalItemsRetrieved >= compiledModel.ExecutionPlan.MaxItems;
            if (retrievedEnoughItems)
                return false;

            bool atLeastOnePageRetrieved = totalItemsRetrieved > 0;
            if (atLeastOnePageRetrieved)
            {
                if (!this.compiledModel.Offset.HasValue)
                    this.compiledModel.Offset = 0;
                this.compiledModel.Offset += this.currentItems.Count();
            }

            var url = GenerateRequestUrlFromModel();
            var result = await dataStore.GetCollectionAsync<T>(url, cancellationToken);

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
            bool noExpressionToCompile = this.expression == null;
            if (noExpressionToCompile)
                return false;

            var queryModel = ExtendedQueryParser.Create().GetParsedQuery(this.expression);
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
                CompileExpressionToRequestModel();

            var argumentList = CollectionResourceRequestModelCompiler.GetArguments(this.compiledModel);
            if (!argumentList.Any())
                return baseHref;

            var arguments = string.Join("&", argumentList);
            return $"{baseHref}?{arguments}";
        }
    }
}
