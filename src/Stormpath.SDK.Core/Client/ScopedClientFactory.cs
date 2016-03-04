// <copyright file="ScopedClientOptions.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.Client;

namespace Stormpath.SDK.Client
{
    /// <inheritdoc/>
    public sealed class ScopedClientFactory : IScopedClientFactory
    {
        private readonly DefaultClient baseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedClientFactory"/> class based on an existing <see cref="IClient">Client</see>.
        /// </summary>
        /// <param name="existing">The existing <see cref="IClient">Client</see>.</param>
        public ScopedClientFactory(IClient existing)
        {
            this.baseClient = existing as DefaultClient;

            if (this.baseClient == null)
            {
                throw new ArgumentException("Unknown client type.", nameof(existing));
            }
        }

        /// <inheritdoc/>
        public IClient Create(ScopedClientOptions options = null)
        {
            if (options == null)
            {
                options = new ScopedClientOptions();
            }

            // Create a new user agent builder
            var userAgentBuilder = new PrependingUserAgentBuilder(baseClient.UserAgentBuilder, options.UserAgent);

            return new DefaultClient(this.baseClient, userAgentBuilder, options.Identifier);
        }
    }
}
