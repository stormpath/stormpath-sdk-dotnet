// <copyright file="BasicRequestAuthenticator.cs" company="Stormpath, Inc.">
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
using System.Text;
using Stormpath.SDK.Api;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    internal sealed class BasicRequestAuthenticator : IRequestAuthenticator
    {
        private static readonly string StormpathDateHeaderName = "X-Stormpath-Date";

        void IRequestAuthenticator.Authenticate(IHttpRequest request, IClientApiKey apiKey)
        {
            var now = DateTimeOffset.UtcNow;
            this.AuthenticateCore(request, apiKey, now);
        }

        internal void AuthenticateCore(IHttpRequest request, IClientApiKey apiKey, DateTimeOffset now)
        {
            request.Headers.Add(StormpathDateHeaderName, Iso8601.Format(now, withSeparators: false));

            var authorizationHeaderContent = $"{apiKey.GetId()}:{apiKey.GetSecret()}";
            var authorizationHeaderEncrypted = Base64.Encode(authorizationHeaderContent, Encoding.UTF8);
            request.Headers.Authorization = new AuthorizationHeaderValue("Basic", authorizationHeaderEncrypted);
        }
    }
}
