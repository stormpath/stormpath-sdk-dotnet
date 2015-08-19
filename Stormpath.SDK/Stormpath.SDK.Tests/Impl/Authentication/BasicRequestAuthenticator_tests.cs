// <copyright file="BasicRequestAuthenticator_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Net.Http;
using System.Text;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Api;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Authentication
{
    public class BasicRequestAuthenticator_tests
    {
        private readonly IRequestAuthenticator authenticator;
        private readonly IClientApiKey apiKey;

        private readonly string fakeApiKeyId = "foo-api-key";
        private readonly string fakeApiKeySecret = "super-secret!1";

        public BasicRequestAuthenticator_tests()
        {
            authenticator = new BasicRequestAuthenticator();
            apiKey = new ClientApiKey(fakeApiKeyId, fakeApiKeySecret);
        }

        [Fact]
        public void Authenticates_request_with_basic_scheme()
        {
            var myRequest = new HttpRequestMessage();

            authenticator.Authenticate(myRequest, apiKey);

            // X-Stormpath-Date -> now in UTC
            var XStormpathDateHeader = Iso8601.Parse(myRequest.Headers.GetValues("X-Stormpath-Date").Single());
            XStormpathDateHeader.Year.ShouldBe(DateTimeOffset.UtcNow.Year);
            XStormpathDateHeader.Month.ShouldBe(DateTimeOffset.UtcNow.Month);
            XStormpathDateHeader.Day.ShouldBe(DateTimeOffset.UtcNow.Day);
            XStormpathDateHeader.Offset.ShouldBe(TimeSpan.Zero);

            // Authorization -> Basic [base64 stuff]
            var authenticationHeader = myRequest.Headers.Authorization;
            authenticationHeader.Scheme.ShouldBe("Basic");
            authenticationHeader.Parameter.FromBase64(Encoding.UTF8).ShouldBe($"{fakeApiKeyId}:{fakeApiKeySecret}");
        }
    }
}
