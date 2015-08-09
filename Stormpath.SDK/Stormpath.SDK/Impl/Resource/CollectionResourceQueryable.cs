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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal class CollectionResourceQueryable<T> : QueryableBase<T>, ICollectionResourceQueryable<T>
        where T : IResource
    {
        public CollectionResourceQueryable(string url, string resource, IDataStore dataStore)
            : base(ExtendedQueryParser.Create(), CreateQueryExecutor(url, resource, dataStore))
        {
            this.Url = url;
            this.Resource = resource;
            this.DataStore = dataStore;
        }

        // This constructor is called internally by LINQ
        public CollectionResourceQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        public string Url { get; private set; }

        public string Resource { get; private set; }

        public IDataStore DataStore { get; private set; }

        int ICollectionResourceQueryable<T>.Offset
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ICollectionResourceQueryable<T>.Limit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ICollectionResourceQueryable<T>.Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private static IQueryExecutor CreateQueryExecutor(string url, string resource, IDataStore dataStore)
        {
            return new CollectionResourceQueryExecutor(url, resource, dataStore);
        }
    }
}
