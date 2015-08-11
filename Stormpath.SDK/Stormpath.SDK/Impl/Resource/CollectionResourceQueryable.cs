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
    internal class CollectionResourceQueryable<T> : AsyncQueryableBase<T>, ICollectionResourceQueryable<T>
    {
        private readonly Expression expression;

        private readonly IDataStore dataStore;

        private readonly string baseHref;

        private CollectionResourceRequestModel compiledModel = null;

        private bool retrievedAtLeastOnce = false;

        private int currentOffset;

        private int currentLimit;

        private int currentSize;

        private IEnumerable<T> currentItems;

        public CollectionResourceQueryable(string url, string resource, IDataStore dataStore)
            : base(ExtendedQueryParser.Create(), CreateQueryExecutor(url, resource, dataStore))
        {
            this.baseHref = $"{url}/{resource}";
            this.dataStore = dataStore;
        }

        // (This constructor is called internally by LINQ)
        public CollectionResourceQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
            var relinqProvider = provider as DefaultQueryProvider;
            var executor = relinqProvider?.Executor as CollectionResourceQueryAsyncExecutor;
            if (relinqProvider == null || executor == null)
                throw new InvalidOperationException("LINQ queries must start from a supported ICollectionResourceQueryable.");

            this.baseHref = $"{executor.Url}/{executor.Resource}";
            this.dataStore = executor.DataStore;
            this.expression = expression;
        }

        // (This is used by Relinq)
        private static IAsyncQueryExecutor CreateQueryExecutor(string url, string resource, IDataStore dataStore)
        {
            return new CollectionResourceQueryAsyncExecutor(url, resource, dataStore);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Grouping internal methods above")]
        int ICollectionResourceQueryable<T>.Offset
        {
            get
            {
                if (!retrievedAtLeastOnce)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentOffset;
            }
        }

        int ICollectionResourceQueryable<T>.Limit
        {
            get
            {
                if (!retrievedAtLeastOnce)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentLimit;
            }
        }

        int ICollectionResourceQueryable<T>.Size
        {
            get
            {
                if (!retrievedAtLeastOnce)
                    throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");

                return currentSize;
            }
        }

        IEnumerable<T> ICollectionResourceQueryable<T>.CurrentPage
        {
            get
            {
                if (!retrievedAtLeastOnce)
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
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsCompiled())
                CompileExpressionToRequestModel();

            var url = GenerateRequestUrlFromModel();
            var result = await dataStore.GetCollectionAsync<T>(url);

            if (!result.Items.Any())
                return false;

            this.currentOffset = result.Offset;
            this.currentLimit = result.Limit;
            this.currentSize = result.Size;
            this.currentItems = result.Items;

            return true;
        }

        private bool IsCompiled()
        {
            return this.compiledModel != null;
        }

        private void CompileExpressionToRequestModel()
        {
            var model = ExtendedQueryParser.Create().GetParsedQuery(this.expression);
            var visitor = new CollectionResourceQueryModelVisitor();
            visitor.VisitQueryModel(model);
            this.compiledModel = visitor.ParsedModel;
        }

        private string GenerateRequestUrlFromModel()
        {
            if (!IsCompiled())
                CompileExpressionToRequestModel();

            var argumentList = CollectionResourceRequestModelCompiler.GetArguments(this.compiledModel);
            var arguments = string.Join("&", argumentList);

            return $"{baseHref}?{arguments}";
        }
    }
}
