// <copyright file="IHttpClientFactory.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents a factory that can create <see cref="IHttpClientBuilder">HTTP client builders</see>.
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Use the default HTTP client.
        /// </summary>
        /// <remarks>
        /// Dynamically loads the default HTTP client (currently <c>RestSharpClient</c>) by searching the application path.
        /// This method is implicitly called by <see cref="Client.IClientBuilder"/> unless a different <see cref="IHttpClientBuilder">client builder</see> is specified.
        /// </remarks>
        /// <seealso cref="Client.IClientBuilder.SetHttpClient(IHttpClientBuilder)"/>
        /// <returns>A <see cref="IHttpClientBuilder">builder</see> capable of constructing the default HTTP client.</returns>
        IHttpClientBuilder AutoDetect();
    }
}
