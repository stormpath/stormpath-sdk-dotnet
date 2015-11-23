// <copyright file="DefaultRetrievalOptions{T}.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class DefaultRetrievalOptions<T> : IRetrievalOptions<T>
    {
        internal IAsyncQueryable<T> Proxy = CollectionResourceQueryable<T>.Empty<T>();
        private bool dirty = false;

        private IRetrievalOptions<T> AsInterface => this;

        string ICreationOptions.GetQueryString()
        {
            if (!this.dirty)
                return string.Empty;

            var queryModel = QueryModelCompiler.Compile(this.Proxy.Expression);
            var arguments = QueryModelParser.GetArguments(queryModel);

            return string.Join(",", arguments);
        }

        public override string ToString()
            => this.AsInterface.GetQueryString();
    }
}
