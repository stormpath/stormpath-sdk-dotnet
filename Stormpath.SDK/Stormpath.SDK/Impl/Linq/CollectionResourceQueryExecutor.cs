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
using Remotion.Linq;
using Stormpath.SDK.DataStore;

namespace Stormpath.SDK.Impl.Linq
{
    internal class CollectionResourceQueryExecutor : IQueryExecutor
    {
        private readonly string url;
        private readonly string resource;
        private readonly IDataStore dataStore;

        public CollectionResourceQueryExecutor(string url, string resource, IDataStore dataStore)
        {
            this.url = url;
            this.resource = resource;
            this.dataStore = dataStore;
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var model = CollectionResourceQueryModelVisitor.GenerateRequestModel(queryModel);
            var arguments = CollectionResourceRequestModelCompiler.GetArguments(model);
            var argumentString = string.Join("&", arguments);

            return dataStore.GetCollection<T>($"{url}/{resource}?{argumentString}");
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
    }
}
