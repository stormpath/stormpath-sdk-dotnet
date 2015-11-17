// <copyright file="Linq_test{T}.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Tests.Common.Fakes;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public abstract class Linq_test<T>
    {
        protected string BaseUrl { get; } = "http://f.oo/";

        protected IAsyncQueryable<T> Queryable { get; private set; }

        protected CollectionReturningHttpClient<T> FakeHttpClient { get; private set; }

        protected void InitializeClientWithCollection(IEnumerable<T> items)
        {
            this.FakeHttpClient = new CollectionReturningHttpClient<T>(this.BaseUrl, items);

            var client = Clients.Builder()
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetBaseUrl(this.BaseUrl)
                .SetHttpClient(this.FakeHttpClient)
                .Build();

            this.Queryable = new CollectionResourceQueryable<T>(this.BaseUrl, (client as DefaultClient).DataStore);
        }
    }
}
