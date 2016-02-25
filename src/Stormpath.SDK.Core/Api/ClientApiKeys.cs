// <copyright file="ClientApiKeys.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Api
{
    /// <summary>
    /// Static entry point for working with <see cref="IClientApiKey">Client API Key</see> objects.
    /// </summary>
    public sealed class ClientApiKeys
    {
        /// <summary>
        /// Gets a new <see cref="IClientApiKeyBuilder"/> instance, used to fluently construct <see cref="IClientApiKey">Client API Key</see> instances to authenticate calls to Stormpath.
        /// </summary>
        /// <param name="logger">A logger instance for capturing trace output; pass <see langword="null"/> to disable logging.</param>
        /// <returns>A new <see cref="IClientApiKeyBuilder"/> instance.</returns>
        [Obsolete("Use IClientBuilder")]
        public static IClientApiKeyBuilder Builder(ILogger logger = null)
        {
            return new ShimClientApiKeyBuilder(logger);
        }
    }
}
