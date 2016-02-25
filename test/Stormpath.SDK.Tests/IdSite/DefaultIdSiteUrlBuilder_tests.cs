// <copyright file="DefaultIdSiteUrlBuilder_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.IdSite
{
    public class DefaultIdSiteUrlBuilder_tests
    {
        private static readonly string FakeApiKeyId = "foobar";
        private static readonly string FakeApiKeySecret = "veryLONGsupersecret_string123";

        private static readonly string FakeApplicationHref = "https://api.stormpath.com/v1/applications/foobarApplication";
        private static readonly string FakeCallbackUri = "http://my.app/login";

        private static readonly string SsoUrl = "https://api.stormpath.com/sso";
        private static readonly string JwtArg = "?jwtRequest=";

        private readonly string fakeJti = "12345";
        private readonly IIdSiteJtiProvider fakeJtiProvider;

        private readonly DateTimeOffset fakeNow = new DateTimeOffset(2016, 01, 01, 00, 00, 00, TimeSpan.Zero);
        private readonly IClock fakeClock;

        private readonly IClient fakeClient;

        public DefaultIdSiteUrlBuilder_tests()
        {
            this.fakeJtiProvider = Substitute.For<IIdSiteJtiProvider>();
            this.fakeJtiProvider.NewJti().Returns(this.fakeJti);

            this.fakeClock = Substitute.For<IClock>();
            this.fakeClock.Now.Returns(this.fakeNow);

            var testApiKey = ClientApiKeys.Builder()
                .SetId(FakeApiKeyId)
                .SetSecret(FakeApiKeySecret)
                .Build();

            this.fakeClient = Clients.Builder()
                .SetApiKey(testApiKey)
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();
        }

        private static readonly string PlainRequest = $"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxMjM0NSIsImlhdCI6MTQ1MTYwNjQwMCwiaXNzIjoiZm9vYmFyIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hcHBsaWNhdGlvbnMvZm9vYmFyQXBwbGljYXRpb24iLCJjYl91cmkiOiJodHRwOi8vbXkuYXBwL2xvZ2luIn0.s5tkKAJX3SzAey9fvaG_MGJCESpcLfWdM8PVtrwx5KI";

        [Fact]
        public void Generates_url()
        {
            var builder = this.GetBuilder();

            var url = builder.Build();
            url.ShouldBe($"{SsoUrl}{JwtArg}{PlainRequest}");

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
        }

        [Fact]
        public void Generates_url_for_logout()
        {
            var builder = this.GetBuilder();
            builder.ForLogout();

            var url = builder.Build();
            url.ShouldBe($"{SsoUrl}/logout{JwtArg}{PlainRequest}");
        }

        [Fact]
        public void Generates_url_with_path()
        {
            var builder = this.GetBuilder();
            builder.SetPath("/foo");

            var url = builder.Build();

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("path").ShouldBe("/foo");
        }

        [Fact]
        public void Generates_url_with_organization_nameKey()
        {
            var builder = this.GetBuilder();
            builder.SetOrganizationNameKey("gonk");

            var url = builder.Build();

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("onk").ShouldBe("gonk");
        }

        [Fact]
        public void Generates_url_with_subdomain_flag()
        {
            var builder = this.GetBuilder();
            builder.SetUseSubdomain(true);

            var url = builder.Build();

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("usd").ShouldBe(true);
        }

        [Fact]
        public void Generates_url_with_organization_field_flag()
        {
            var builder = this.GetBuilder();
            builder.SetShowOrganizationField(true);

            var url = builder.Build();

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("sof").ShouldBe(true);
        }

        [Fact]
        public void Generates_url_with_state()
        {
            var builder = this.GetBuilder();
            builder.SetState("foobar123!");

            var url = builder.Build();

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("state").ShouldBe("foobar123!");
        }

        [Fact]
        public void Generates_url_with_all_the_things()
        {
            var builder = this.GetBuilder();
            builder.ForLogout();
            builder.SetOrganizationNameKey("FirstOrder");
            builder.SetPath("/blah");
            builder.SetShowOrganizationField(false);
            builder.SetState("123");
            builder.SetUseSubdomain(false);

            var url = builder.Build();
            url.ShouldStartWith($"{SsoUrl}/logout{JwtArg}");

            var jwt = this.ParseUrl(url);
            this.VerifyCommon(jwt);
            jwt.Body.GetClaim("onk").ShouldBe("FirstOrder");
            jwt.Body.GetClaim("path").ShouldBe("/blah");
            jwt.Body.GetClaim("sof").ShouldBe(false);
            jwt.Body.GetClaim("state").ShouldBe("123");
            jwt.Body.GetClaim("usd").ShouldBe(false);
        }

        private IIdSiteUrlBuilder GetBuilder()
        {
            var client = this.fakeClient as DefaultClient;

            IIdSiteUrlBuilder builder = new DefaultIdSiteUrlBuilder(
                client.DataStore, FakeApplicationHref, this.fakeJtiProvider, this.fakeClock);
            builder.SetCallbackUri(FakeCallbackUri);

            return builder;
        }

        private IJwt ParseUrl(string url)
            => this.fakeClient.NewJwtParser().Parse(url.Split('=')[1]);

        private void VerifyCommon(IJwt jwt)
        {
            jwt.Body.Issuer.ShouldBe(FakeApiKeyId);
            jwt.Body.Subject.ShouldBe(FakeApplicationHref);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
        }
    }
}
