// <copyright file="ClientProviderExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Provider;

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Provides a set of static methods for making Provider-based requests.
    /// </summary>
    public static class ClientProviderExtensions
    {
        /// <summary>
        /// Returns a new <see cref="IProviderFactory"/>, used to construct Provider-based requests.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>A new <see cref="IProviderFactory"/>.</returns>
        public static IProviderFactory Providers(this IClient client)
            => new DefaultProviderFactory((client as DefaultClient).DataStore);
    }
}
