// <copyright file="Clients.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Client;
using Microsoft.Extensions.PlatformAbstractions;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Static entry point for working with <see cref="IClient">Client</see> objects.
    /// </summary>
    public sealed class Clients
    {
        /// <summary>
        /// Gets a new <see cref="IClientBuilder"/> instance, used to fluently construct <see cref="IClient">Client</see> instances.
        /// </summary>
        /// <returns>A new <see cref="IClientBuilder"/> instance.</returns>
        /// <example>
        /// <code>
        /// IClient client = Clients.Builder()
        ///     .SetApiKey(apiKey)
        ///     .Build();
        /// </code>
        /// </example>
        public static IClientBuilder Builder()
        {
            var language = string.Empty;
#if NET451
            var currentAssembly = System.Reflection.Assembly.GetCallingAssembly();
#else
#endif

            var userAgentBuilder = new DefaultUserAgentBuilder(
                runtimeEnvironment: PlatformServices.Default.Runtime,
                appEnvironment: PlatformServices.Default.Application,
                frameworkUserAgent: string.Empty, // todo - from StormpathConfiguration
                language: language);

            return new DefaultClientBuilder(userAgentBuilder);
        }
    }
}
