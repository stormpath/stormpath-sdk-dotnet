// <copyright file="BasicRequestAuthenticator.cs" company="Stormpath, Inc.">
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

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    internal sealed class BasicRequestAuthenticator : IRequestAuthenticator
    {
        private static readonly string StormpathDateHeaderName = "X-Stormpath-Date";

        void IRequestAuthenticator.Authenticate(HttpRequestMessage request, IClientApiKey apiKey)
        {
            var utcNow = DateTimeOffset.UtcNow;
            request.Headers.Add(StormpathDateHeaderName, Iso8601.Format(utcNow));

            var authorizationHeaderContent = $"{apiKey.GetId()}:{apiKey.GetSecret()}";
            var authorizationHeader = authorizationHeaderContent.ToBase64(Encoding.UTF8);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
        }
    }
}
