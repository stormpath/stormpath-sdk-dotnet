// <copyright file="ISynchronousHttpClient.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// An HTTP client that can execute synchronous requests.
    /// </summary>
    public interface ISynchronousHttpClient : IHttpClient
    {
        /// <summary>
        /// Executes an HTTP request synchronously.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The HTTP response.</returns>
        IHttpResponse Execute(IHttpRequest request);
    }
}
