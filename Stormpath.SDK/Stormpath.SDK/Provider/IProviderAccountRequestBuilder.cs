// <copyright file="IProviderAccountRequestBuilder.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// A builder to construct <see cref="IProviderAccountRequest"/>s.
    /// </summary>
    /// <typeparam name="T">The specific builder interface
    /// (e.g. <see cref="IFacebookAccountRequestBuilder"/> or <see cref="IGoogleAccountRequestBuilder"/>).</typeparam>
    public interface IProviderAccountRequestBuilder<T>
        where T : IProviderAccountRequestBuilder<T>
    {
        /// <summary>
        /// Sets the Provider App authorization code.
        /// </summary>
        /// <param name="accessToken">The authorization code.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetAccessToken(string accessToken);

        /// <summary>
        /// Creates a new <see cref="IProviderAccountRequest"/> based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IProviderAccountRequest"/> based on the current builder state.</returns>
        IProviderAccountRequest Build();
    }
}
