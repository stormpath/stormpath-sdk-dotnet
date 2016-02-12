// <copyright file="IRequestAuthenticator.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Api;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    /// <summary>
    /// Represents an abstract authenticator that can authenticate a request.
    /// </summary>
    internal interface IRequestAuthenticator
    {
        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="apiKey">The client API Key.</param>
        void Authenticate(IHttpRequest request, IClientApiKey apiKey);
    }
}
