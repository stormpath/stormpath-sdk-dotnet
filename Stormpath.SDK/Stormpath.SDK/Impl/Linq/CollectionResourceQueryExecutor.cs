// <copyright file="CollectionResourceQueryExecutor.cs" company="Stormpath, Inc.">
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
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryExecutor : IQueryExecutor
    {
        internal CollectionResourceQueryExecutor(string href, IInternalDataStore dataStore)
        {
            this.Href = href;
            this.DataStore = dataStore;
        }

        public string Href { get; private set; }

        public IInternalDataStore DataStore { get; private set; }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var model = GenerateRequestModel(queryModel);

            return this.ExecuteCollection<T>(model);
        }

        public IEnumerable<T> ExecuteCollection<T>(CollectionResourceRequestModel requestModel)
        {
            return this.ExecuteCollection(requestModel, typeof(T)) as IEnumerable<T>;
        }

        private IEnumerable<object> ExecuteCollection(CollectionResourceRequestModel requestModel, Type collectionType)
        {
            var collectionResourceQueryableType = typeof(CollectionResourceQueryable<>).MakeGenericType(collectionType);
            var syncAdapterType = typeof(Sync.SyncCollectionEnumeratorAdapter<>).MakeGenericType(collectionType);

            var asyncCollection = Activator.CreateInstance(collectionResourceQueryableType, new object[] { this.Href, this.DataStore, requestModel });
            var adapter = Activator.CreateInstance(syncAdapterType, new object[] { asyncCollection, null });

            return adapter as IEnumerable<object>;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var model = GenerateRequestModel(queryModel);

            if (queryModel.ResultOperators.FirstOrDefault() is AnyResultOperator)
                return (T)((object)this.ExecuteCollection(model, queryModel.MainFromClause.ItemType).Any());

            if (queryModel.ResultOperators.FirstOrDefault() is CountResultOperator ||
                queryModel.ResultOperators.FirstOrDefault() is LongCountResultOperator)
            {
                var iterator = this.ExecuteCollection(model, queryModel.MainFromClause.ItemType);
                var collection = iterator
                    .GetType()
                    .GetField("collection", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(iterator);
                var collectionType = collection.GetType();

                var moveNextResult = (bool)collectionType
                    .GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(collection, null);

                var count = collectionType
                    .GetProperty("Size", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(collection);

                if (typeof(T) == typeof(int))
                    return (T)(object)Convert.ToInt32(count);
                else
                    return (T)count;
            }

            throw new NotSupportedException("One or more scalar result operators are unsupported.");
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty
                ? ExecuteCollection<T>(queryModel).SingleOrDefault()
                : ExecuteCollection<T>(queryModel).Single();
        }

        private static CollectionResourceRequestModel GenerateRequestModel(QueryModel queryModel)
        {
            return CollectionResourceQueryModelVisitor.GenerateRequestModel(queryModel);
        }
    }
}
