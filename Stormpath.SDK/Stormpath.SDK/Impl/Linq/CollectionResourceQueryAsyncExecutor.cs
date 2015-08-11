// <copyright file="CollectionResourceQueryAsyncExecutor.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Remotion.Linq;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.Linq
{
    internal class CollectionResourceQueryAsyncExecutor : IAsyncQueryExecutor
    {
        public CollectionResourceQueryAsyncExecutor(string url, string resource, IDataStore dataStore)
        {
            this.Url = url;
            this.Resource = resource;
            this.DataStore = dataStore;
        }

        public string Url { get; private set; }

        public string Resource { get; private set; }

        public IDataStore DataStore { get; private set; }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteAsync<T>(QueryModel queryModel)
        {
            var arguments = QueryModelToArguments(queryModel);

            // return dataStore.GetCollectionAsync<T>($"{url}/{resource}?{arguments}");
            throw new NotImplementedException();
        }

        private static string QueryModelToArguments(QueryModel queryModel)
        {
            var model = CollectionResourceQueryModelVisitor.GenerateRequestModel(queryModel);
            var arguments = CollectionResourceRequestModelCompiler.GetArguments(model);
            var argumentString = string.Join("&", arguments);
            return argumentString;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty
                ? ExecuteCollection<T>(queryModel).SingleOrDefault()
                : ExecuteCollection<T>(queryModel).Single();
        }

        public Task<IEnumerable<T>> ExecuteCollectionAsync<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalarAsync<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteSingleAsync<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
