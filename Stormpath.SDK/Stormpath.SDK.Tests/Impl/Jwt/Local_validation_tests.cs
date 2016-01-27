// <copyright file="Local_validation_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Jwt
{
    public class Local_validation_tests
    {
        private static readonly string IDSiteAccessToken = "eyJraWQiOiJmYWtlX2FwaV9rZXlfaWQiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3TUpNUEk5cUZ1VzIxWU44d0RQZmpKIiwiaWF0IjoxNDUzNDIyNDU5LCJpc3MiOiJodHRwczovL2F3ZXNvbWUtdGVuYW50LmlkLnN0b3JtcGF0aC5pbyIsInN1YiI6Imh0dHBzOi8vYXBpLnN0b3JtcGF0aC5jb20vdjEvYWNjb3VudHMvZm9vYmFyQWNjb3VudCIsImV4cCI6MzQ1MzQyNjA1OSwicnRpIjoiN01KTVBFcGxMS0QzT2pISGp4WVA3RiJ9.snRIZf6ovY8TGv_f5wASHkzRmf_O_rtvRhPUnmoCfhU";

        private static IInternalDataStore GetFakeDataStore()
        {
            var fakeApiKey = ClientApiKeys.Builder()
                .SetId("fake_api_key")
                .SetSecret("fake_secret")
                .Build();

            var stubClient = Clients.Builder()
                .SetApiKey(fakeApiKey)
                .Build();

            var fakeDataStore = TestDataStore.Create(
                requestExecutor: new StubRequestExecutor(FakeJson.Application, fakeApiKey).Object,
                client: stubClient);

            return fakeDataStore;
        }

        /// <summary>
        /// Regression test for stormpath/stormpath-sdk-dotnet#124
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous unit test.</returns>
        [Fact]
        public async Task Validating_ID_site_token_fails()
        {
            var fakeDataStore = GetFakeDataStore();
            var fakeApplication = await fakeDataStore.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var request = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(IDSiteAccessToken)
                .Build();

            IJwtAuthenticator authenticator = new DefaultJwtAuthenticator(fakeApplication, fakeDataStore);
            authenticator.WithLocalValidation();

            await Should.ThrowAsync<MismatchedClaimException>(authenticator.AuthenticateAsync(request));
        }

        /// <summary>
        /// Regression test for the fix for stormpath/stormpath-sdk-dotnet#124
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous unit test.</returns>
        [Fact]
        public async Task Validating_token_with_specified_issuer()
        {
            var fakeDataStore = GetFakeDataStore();
            var fakeApplication = await fakeDataStore.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var request = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(IDSiteAccessToken)
                .Build();

            IJwtAuthenticator authenticator = new DefaultJwtAuthenticator(fakeApplication, fakeDataStore);
            authenticator.WithLocalValidation(new JwtLocalValidationOptions()
            {
                Issuer = "https://awesome-tenant.id.stormpath.io"
            });

            // Should not throw
            var result = await authenticator.AuthenticateAsync(request);

            result.Jwt.ShouldBe(IDSiteAccessToken);
        }

        [Fact]
        public async Task Validating_token_with_bad_issuer()
        {
            var fakeDataStore = GetFakeDataStore();
            var fakeApplication = await fakeDataStore.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var request = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(IDSiteAccessToken)
                .Build();

            IJwtAuthenticator authenticator = new DefaultJwtAuthenticator(fakeApplication, fakeDataStore);
            authenticator.WithLocalValidation(new JwtLocalValidationOptions()
            {
                Issuer = "nope"
            });

            // Should not throw
            await Should.ThrowAsync<MismatchedClaimException>(authenticator.AuthenticateAsync(request));
        }
    }
}
