// <copyright file="ICreateProviderRequestBuilder.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// A Builder to construct <see cref="ICreateProviderRequest"/>s.
    /// </summary>
    /// <typeparam name="T">The specific builder interface
    /// (e.g. <see cref="IFacebookCreateProviderRequestBuilder"/> or <see cref="IGoogleCreateProviderRequestBuilder"/>).</typeparam>
    public interface ICreateProviderRequestBuilder<T>
        where T : ICreateProviderRequestBuilder<T>
    {
        /// <summary>
        /// Sets the App ID of your Provider application
        /// (e.g. for Google it looks similar to "143482128708.apps.googleusercontent.com").
        /// </summary>
        /// <param name="clientId">The App ID of your Provider application.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetClientId(string clientId);

        /// <summary>
        /// Sets the App Secret of your Provider application
        /// (e.g. for Google it looks similar to "U-IdloztzwLn2_2M4QjpulPq").
        /// </summary>
        /// <param name="clientSecret">The App Secret of your Provider application.</param>
        /// <returns>This instance for method chaining.</returns>
        T SetClientSecret(string clientSecret);

        /// <summary>
        /// Creates a new <see cref="ICreateProviderRequest"/> instance based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="ICreateProviderRequest"/> instance based on the current builder state.</returns>
        ICreateProviderRequest Build();
    }
}
