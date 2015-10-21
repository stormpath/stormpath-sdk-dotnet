// <copyright file="CollectionResourceExecutor.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

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

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceExecutor<TResult>
    {
        private readonly string collectionHref;
        private readonly IInternalAsyncDataStore asyncDataStore;
        private readonly IInternalSyncDataStore syncDataStore;
        private readonly Expression expression;

        private CollectionResourceQueryModel compiledModel = null;

        private bool enumeratedOnce = false;

        private long totalItemsRetrieved = 0;

        private long currentOffset;

        private long currentLimit;

        private long currentSize;

        private IEnumerable<TResult> currentItems;

        public CollectionResourceExecutor(string collectionHref, IInternalDataStore dataStore, Expression expression)
        {
            this.collectionHref = collectionHref;
            this.asyncDataStore = dataStore as IInternalAsyncDataStore;
            this.syncDataStore = dataStore as IInternalSyncDataStore;
            this.expression = expression;
        }

        public CollectionResourceExecutor(CollectionResourceExecutor<TResult> executor, Expression newExpression)
        {
            this.collectionHref = executor.collectionHref;
            this.asyncDataStore = executor.asyncDataStore;
            this.syncDataStore = executor.syncDataStore;
            this.expression = newExpression;
        }

        private void ThrowIfNotEnumerated()
        {
            if (!this.enumeratedOnce)
                throw new InvalidOperationException("Call MoveNextAsync() first to retrieve the collection.");
        }

        public long Offset
        {
            get
            {
                this.ThrowIfNotEnumerated();

                return this.currentOffset;
            }
        }

        public long Limit
        {
            get
            {
                this.ThrowIfNotEnumerated();

                return this.currentLimit;
            }
        }

        public long Size
        {
            get
            {
                this.ThrowIfNotEnumerated();

                return this.currentSize;
            }
        }

        public IEnumerable<TResult> CurrentPage
        {
            get
            {
                this.ThrowIfNotEnumerated();

                return this.currentItems;
            }
        }

        public CollectionResourceQueryModel CompiledModel
        {
            get
            {
                this.ThrowIfNotEnumerated();

                return this.compiledModel;
            }
        }

        public string CurrentHref
            => this.GenerateRequestUrlFromModel();

        public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            this.CompileModelOrUseDefaultValues();

            if (this.AlreadyRetrievedEnoughItems())
                return false;

            this.AdjustPagingOffset();

            var url = this.GenerateRequestUrlFromModel();
            var response = await this.asyncDataStore.GetCollectionAsync<TResult>(url, cancellationToken).ConfigureAwait(false);

            this.enumeratedOnce = true;

            return this.DidUpdateWithNewResults(response);
        }

        public bool MoveNext()
        {
            this.CompileModelOrUseDefaultValues();

            if (this.AlreadyRetrievedEnoughItems())
                return false;

            this.AdjustPagingOffset();

            var url = this.GenerateRequestUrlFromModel();
            var response = this.syncDataStore.GetCollection<TResult>(url);

            this.enumeratedOnce = true;

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

        private bool DidUpdateWithNewResults(CollectionResponsePage<TResult> response)
        {
            this.currentOffset = response.Offset;
            this.currentLimit = response.Limit;
            this.currentSize = response.Size;
            this.currentItems = response.Items;

            bool anyNewItems = response.Items.Any();
            if (!anyNewItems)
                return false;

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
