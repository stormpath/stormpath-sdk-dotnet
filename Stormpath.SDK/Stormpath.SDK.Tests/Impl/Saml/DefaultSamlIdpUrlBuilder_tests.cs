// <copyright file="DefaultSamlIdpUrlBuilder_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Saml;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Saml;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Saml
{
    public class DefaultSamlIdpUrlBuilder_tests
    {
        private static readonly string FakeApiKeyId = "foobar";
        private static readonly string FakeApiKeySecret = "veryLONGsupersecret_string123";

        private static readonly string FakeApplicationHref = "https://api.stormpath.com/v1/applications/foobarApplication";
        private static readonly string FakeCallbackUri = "http://my.app/login";
        private static readonly string FakeSamlEndpoint = "https://api.stormpath.com/v1/applications/foobarApplication/provider";

        private static readonly string FakeJti = "12345";
        private readonly IIdSiteJtiProvider fakeJtiProvider;

        private readonly DateTimeOffset fakeNow = new DateTimeOffset(2016, 01, 01, 00, 00, 00, TimeSpan.Zero);
        private readonly IClock fakeClock;

        private readonly IClient fakeClient;

        public DefaultSamlIdpUrlBuilder_tests()
        {
            this.fakeJtiProvider = Substitute.For<IIdSiteJtiProvider>();
            this.fakeJtiProvider.NewJti().Returns(FakeJti);

            this.fakeClock = Substitute.For<IClock>();
            this.fakeClock.Now.Returns(this.fakeNow);

            var testApiKey = ClientApiKeys.Builder()
                .SetId(FakeApiKeyId)
                .SetSecret(FakeApiKeySecret)
                .Build();

            this.fakeClient = Clients.Builder()
                .SetApiKey(testApiKey)
                .Build();
        }

        [Fact]
        public void Throws_for_missing_redirect_uri()
        {
            Should.Throw<ApplicationException>(() =>
            {
                var builder = this.GetBuilder();
                builder.Build();
            });

            Should.Throw<ApplicationException>(() =>
            {
                var builder = this.GetBuilder();
                builder.SetCallbackUri((Uri)null);
                builder.Build();
            });

            Should.Throw<ApplicationException>(() =>
            {
                var builder = this.GetBuilder();
                builder.SetCallbackUri(string.Empty);
                builder.Build();
            });
        }

        [Fact]
        public void Generates_url_with_callback()
        {
            var builder = this.GetBuilder();

            var url = builder
                .SetCallbackUri(FakeCallbackUri)
                .Build();

            url.ShouldStartWith(FakeSamlEndpoint);

            var jwt = this.ParseJwtFromUrl(url);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
        }

        [Fact]
        public void Generates_url_with_path_and_state()
        {
            var builder = this.GetBuilder();

            var url = builder
                .SetCallbackUri(FakeCallbackUri)
                .SetPath("/sso-site")
                .SetState("someState")
                .Build();

            url.ShouldStartWith(FakeSamlEndpoint);

            var jwt = this.ParseJwtFromUrl(url);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
            jwt.Body.GetClaim("path").ShouldBe("/sso-site");
            jwt.Body.GetClaim("state").ShouldBe("someState");
        }

        [Fact]
        public void Generates_url_with_organization()
        {
            var builder = this.GetBuilder();

            var url = builder
                .SetCallbackUri(FakeCallbackUri)
                .SetOrganizationNameKey("first-order")
                .Build();

            url.ShouldStartWith(FakeSamlEndpoint);

            var jwt = this.ParseJwtFromUrl(url);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
            jwt.Body.GetClaim("onk").ShouldBe("first-order");
        }

        [Fact]
        public void Generates_url_with_spToken()
        {
            var builder = this.GetBuilder();

            var url = builder
                .SetCallbackUri(FakeCallbackUri)
                .SetSpToken("anSpToken")
                .Build();

            url.ShouldStartWith(FakeSamlEndpoint);

            var jwt = this.ParseJwtFromUrl(url);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
            jwt.Body.GetClaim("sp_token").ShouldBe("anSpToken");
        }

        [Fact]
        public void Generates_url_with_all_the_things()
        {
            var builder = this.GetBuilder();

            var url = builder
                .SetCallbackUri(FakeCallbackUri)
                .SetOrganizationNameKey("first-order")
                .SetPath("/sso-site")
                .SetSpToken("anSpToken")
                .SetState("someState")
                .AddProperty("unknown", "foobar")
                .Build();

            url.ShouldStartWith(FakeSamlEndpoint);

            var jwt = this.ParseJwtFromUrl(url);
            jwt.Body.GetClaim("cb_uri").ShouldBe(FakeCallbackUri);
            jwt.Body.GetClaim("onk").ShouldBe("first-order");
            jwt.Body.GetClaim("path").ShouldBe("/sso-site");
            jwt.Body.GetClaim("sp_token").ShouldBe("anSpToken");
            jwt.Body.GetClaim("state").ShouldBe("someState");
            jwt.Body.GetClaim("unknown").ShouldBe("foobar");
        }

        private ISamlIdpUrlBuilder GetBuilder()
        {
            var client = this.fakeClient as DefaultClient;

            ISamlIdpUrlBuilder builder = new DefaultSamlIdpUrlBuilder(
                client.DataStore, FakeApplicationHref, FakeSamlEndpoint, this.fakeJtiProvider, this.fakeClock);

            return builder;
        }

        private IJwt ParseJwtFromUrl(string url)
            => this.fakeClient.NewJwtParser().Parse(url.Split('=')[1]);

        private void VerifyCommon(IJwt jwt)
        {
            jwt.Body.Id.ShouldBe(FakeJti);
            jwt.Body.Issuer.ShouldBe(FakeApiKeyId);
            jwt.Body.Subject.ShouldBe(FakeApplicationHref);
        }
    }
}
