// <copyright file="BasicRequestAuthenticator_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Text;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Api;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Authentication
{
    public class BasicRequestAuthenticator_tests
    {
        private readonly UriQualifier uriQualifier;
        private readonly BasicRequestAuthenticator authenticator;
        private readonly IClientApiKey apiKey;

        private readonly string fakeBaseHref = "http://foobar";
        private readonly string fakeApiKeyId = "foo-api-key";
        private readonly string fakeApiKeySecret = "super-secret!1";

        public BasicRequestAuthenticator_tests()
        {
            this.uriQualifier = new UriQualifier(this.fakeBaseHref);
            this.authenticator = new BasicRequestAuthenticator();
            this.apiKey = new DefaultClientApiKey(this.fakeApiKeyId, this.fakeApiKeySecret);
        }

        [Fact]
        public void Adds_XStormpathDate_header()
        {
            var request = new DefaultHttpRequest(HttpMethod.Get, new CanonicalUri(this.uriQualifier.EnsureFullyQualified("/bar")));
            var now = new DateTimeOffset(2015, 08, 01, 06, 30, 00, TimeSpan.Zero);

            this.authenticator.AuthenticateCore(request, this.apiKey, now);

            // X-Stormpath-Date -> current time in UTC
            var stormpathDateHeader = Iso8601.Parse(request.Headers.GetFirst<string>("X-Stormpath-Date"));
            stormpathDateHeader.ShouldBe(now);
        }

        [Fact]
        public void Adds_Basic_authorization_header()
        {
            var request = new DefaultHttpRequest(HttpMethod.Get, new CanonicalUri(this.uriQualifier.EnsureFullyQualified("/bar")));
            var now = new DateTimeOffset(2015, 08, 01, 06, 30, 00, TimeSpan.Zero);

            this.authenticator.AuthenticateCore(request, this.apiKey, now);

            // Authorization: "Basic [base64 stuff]"
            var authenticationHeader = request.Headers.Authorization;
            authenticationHeader.Scheme.ShouldBe("Basic");
            Base64.Decode(authenticationHeader.Parameter, Encoding.UTF8).ShouldBe($"{this.fakeApiKeyId}:{this.fakeApiKeySecret}");
        }
    }
}
