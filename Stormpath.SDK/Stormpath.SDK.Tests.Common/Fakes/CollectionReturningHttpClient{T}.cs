// <copyright file="CollectionReturningHttpClient{T}.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public sealed class CollectionReturningHttpClient<T> : AbstractMockHttpClient
    {
        private static readonly int DefaultLimit = 25;
        private static readonly int DefaultOffset = 0;

        private readonly List<T> items;

        public CollectionReturningHttpClient(string baseUrl, IEnumerable<T> items)
            : base(baseUrl)
        {
            this.items = items.ToList();
        }

        protected override bool IsSupported(HttpMethod method)
            => method == HttpMethod.Get;

        protected override IHttpResponse GetResponse(IHttpRequest request)
        {
            var limit = request.CanonicalUri.QueryString["limit"].ToInt32() ?? DefaultLimit;
            var offset = request.CanonicalUri.QueryString["offset"].ToInt32() ?? DefaultOffset;

            var responseItems = this.items
                .Skip(offset)
                .Take(limit)
                .ToList();

            var collectionResponse = new CollectionResponse<T>
            {
                Href = request.CanonicalUri.ResourcePath.ToString(),
                Size = this.items.Count,
                Limit = limit,
                Offset = offset,
                Items = responseItems
            };

            return new FakeHttpResponse(
                200, Serialize(collectionResponse), "application/json");
        }

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private class CollectionResponse<TItem>
        {
            public string Href { get; set; }

            public int Size { get; set; }

            public int Limit { get; set; }

            public int Offset { get; set; }

            public List<TItem> Items { get; set; }
        }
    }
}
