// <copyright file="CollectionResponsePage{T}.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class CollectionResponsePage<T> : IResource
    {
        private readonly IClient client;

        public CollectionResponsePage(IClient client, string href, long offset, long limit, long size, List<T> items)
        {
            this.Href = href;
            this.Offset = offset;
            this.Limit = limit;
            this.Size = size;
            this.Items = items;
            this.client = client;
        }

        public string Href { get; }

        public long Offset { get; }

        public long Limit { get; }

        public long Size { get; }

        public List<T> Items { get; }

        IClient IResource.Client => this.client;
    }
}
