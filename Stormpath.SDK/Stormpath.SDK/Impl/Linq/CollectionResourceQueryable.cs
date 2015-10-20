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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.QueryModel;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryable<T> : IOrderedAsyncQueryable<T>
    {
        private readonly CollectionResourceQueryProvider<T> queryProvider;

        private readonly Expression expression;

        private readonly string collectionHref;

        private CollectionResourceQueryModel compiledModel = null;

        private long totalItemsRetrieved = 0;

        private long currentOffset;

        private long currentLimit;

        private long currentSize;

        private IEnumerable<T> currentItems;

        public CollectionResourceQueryable(string collectionHref, IInternalDataStore dataStore)
        {
            this.collectionHref = collectionHref;
            this.queryProvider = new CollectionResourceQueryProvider<T>(collectionHref, dataStore);
        }

        // This constructor is called internally by LINQ
        public CollectionResourceQueryable(IAsyncQueryProvider<T> provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (!typeof(IAsyncQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            var concreteProvider = provider as CollectionResourceQueryProvider<T>;
            if (concreteProvider == null)
                throw new InvalidOperationException("LINQ queries must start from a supported provider.");

            this.queryProvider = concreteProvider;
            this.expression = expression;

            this.collectionHref = concreteProvider.CollectionHref;
        }

        private void NoResultsGuard()
        {
            bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
            if (!atLeastOnePageRetrieved)
                throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");
        }

        internal long Offset
        {
            get
            {
                this.NoResultsGuard();

                return this.currentOffset;
            }
        }

        internal long Limit
        {
            get
            {
                NoResultsGuard();

                return this.currentLimit;
            }
        }

        internal long Size
        {
            get
            {
                this.NoResultsGuard();

                return this.currentSize;
            }
        }

        IEnumerable<T> IAsyncQueryable<T>.CurrentPage
        {
            get
            {
                this.NoResultsGuard();

                return this.currentItems;
            }
        }

        internal string CurrentHref => this.GenerateRequestUrlFromModel();

        Expression IAsyncQueryable<T>.Expression => this.expression;

        IAsyncQueryProvider<T> IAsyncQueryable<T>.Provider => this.queryProvider;

        async Task<bool> IAsyncQueryable<T>.MoveNextAsync(CancellationToken cancellationToken)
        {
            this.CompileModelOrUseDefaultValues();

            if (this.AlreadyRetrievedEnoughItems())
                return false;

            this.AdjustPagingOffset();

            var url = this.GenerateRequestUrlFromModel();
            var response = await this.queryProvider.ExecuteCollectionAsync<T>(url, cancellationToken).ConfigureAwait(false);

            return this.DidUpdateWithNewResults(response);
        }

        internal bool MoveNext()
        {
            this.CompileModelOrUseDefaultValues();

            if (this.AlreadyRetrievedEnoughItems())
                return false;

            this.AdjustPagingOffset();

            var url = this.GenerateRequestUrlFromModel();
            var response = this.queryProvider.ExecuteCollection<T>(url);

            return this.DidUpdateWithNewResults(response);
        }

        private void CompileModelOrUseDefaultValues()
        {
            bool needToCompile = this.compiledModel == null;
            if (needToCompile)
            {
                bool shouldUseDefaultValues = this.expression == null;

                this.compiledModel = shouldUseDefaultValues
                    ? CollectionResourceQueryModel.Default
                    : QueryModelCompiler.Compile(this.expression);
            }
        }

        private bool AlreadyRetrievedEnoughItems()
        {
            return this.totalItemsRetrieved >= this.compiledModel.ExecutionPlan.MaxItems;
        }

        private void AdjustPagingOffset()
        {
            bool atLeastOnePageRetrieved = this.totalItemsRetrieved > 0;
            if (atLeastOnePageRetrieved)
            {
                if (!this.compiledModel.Offset.HasValue)
                    this.compiledModel.Offset = 0;
                this.compiledModel.Offset += this.currentItems.Count();
            }
        }

        private bool DidUpdateWithNewResults(CollectionResponsePage<T> response)
        {
            bool anyNewItems = response?.Items?.Any() ?? false;
            if (!anyNewItems)
                return false;

            this.currentOffset = response.Offset;
            this.currentLimit = response.Limit;
            this.currentSize = response.Size;
            this.currentItems = response.Items;

            this.totalItemsRetrieved += response.Items.Count;
            return true;
        }

        private string GenerateRequestUrlFromModel()
        {
            if (this.compiledModel == null)
                this.CompileModelOrUseDefaultValues();

            var argumentList = RequestBuilder.GetArguments(this.compiledModel);
            if (!argumentList.Any())
                return this.collectionHref;

            var arguments = string.Join("&", argumentList);
            return $"{this.collectionHref}?{arguments}";
        }
    }
}
