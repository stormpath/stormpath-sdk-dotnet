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
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Jwt
{
    public class Local_validation_tests
    {
        private static readonly string IDSiteAccessToken = "eyJraWQiOiJmYWtlX2FwaV9rZXlfaWQiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3TUpNUEk5cUZ1VzIxWU44d0RQZmpKIiwiaWF0IjoxNDUzNDIyNDU5LCJpc3MiOiJodHRwczovL2F3ZXNvbWUtdGVuYW50LmlkLnN0b3JtcGF0aC5pbyIsInN1YiI6Imh0dHBzOi8vYXBpLnN0b3JtcGF0aC5jb20vdjEvYWNjb3VudHMvZm9vYmFyQWNjb3VudCIsImV4cCI6MzQ1MzQyNjA1OSwicnRpIjoiN01KTVBFcGxMS0QzT2pISGp4WVA3RiJ9.snRIZf6ovY8TGv_f5wASHkzRmf_O_rtvRhPUnmoCfhU";

        private static readonly string ValidAccessToken = "eyJraWQiOiJmYWtlX2FwaV9rZXkiLCJzdHQiOiJhY2Nlc3MiLCJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxYjFaMTdLa0xBV09UTUg4cXA2aU1SIiwiaWF0IjoxNDcyNjY0NDU0LCJpc3MiOiJodHRwczovL2FwaS5zdG9ybXBhdGguY29tL3YxL2FwcGxpY2F0aW9ucy83T2wzNzdIVTA2OGxhZ0NZazdVOVhTIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hY2NvdW50cy80cVRYMlF5UlZoT05kNWVRcDdoVFEwIiwiZXhwIjozNDcyNjY4MDU0LCJydGkiOiI0NmZBSE90N1laNUVvOFIyMzVQa0YifQ.zTK5oHKkQZBg4yDkgbe2cmMAc-FsX5XZN3mERQYgdZk";

        private static readonly string ValidRefreshToken = "eyJraWQiOiJmYWtlX2FwaV9rZXkiLCJzdHQiOiJyZWZyZXNoIiwidHlwIjoiSldUIiwiYWxnIjoiSFMyNTYifQ.eyJqdGkiOiI0NmZBSE90N1laNUVvOFIyMzVQa0YiLCJpYXQiOjE0NzI1OTQzNjIsImlzcyI6Imh0dHBzOi8vYXBpLnN0b3JtcGF0aC5jb20vdjEvYXBwbGljYXRpb25zLzdPbDM3N0hVMDY4bGFnQ1lrN1U5WFMiLCJzdWIiOiJodHRwczovL2FwaS5zdG9ybXBhdGguY29tL3YxL2FjY291bnRzLzRxVFgyUXlSVmhPTmQ1ZVFwN2hUUTAiLCJleHAiOjM0Nzc3NzgzNjJ9.-VDFISWiqAHI47HKPG47O1phaarcIJqPqcRJDGSJdWs";

        private static IInternalDataStore GetFakeDataStore()
        {
            var fakeApiKey = ClientApiKeys.Builder()
                .SetId("fake_api_key")
                .SetSecret("fake_secret")
                .Build();

            var stubClient = Clients.Builder()
                .SetApiKey(fakeApiKey)
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();

            var fakeDataStore = TestDataStore.Create(
                requestExecutor: new StubRequestExecutor(FakeJson.Application, fakeApiKey).Object,
                client: stubClient);

            return fakeDataStore;
        }

        /// <summary>
        /// Regression test for the fix for stormpath/stormpath-sdk-dotnet#124
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous unit test.</returns>
        [Fact]
        public async Task Validating_token_with_specified_issuer()
        {
            // TODO: Need to get a new example of an ID Site access token
            Assertly.Todo();

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

            await Should.ThrowAsync<MismatchedClaimException>(authenticator.AuthenticateAsync(request));
        }

        [Fact]
        public async Task Validating_access_token_locally()
        {
            var fakeDataStore = GetFakeDataStore();
            var fakeApplication = await fakeDataStore.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var request = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(ValidAccessToken)
                .Build();

            IJwtAuthenticator authenticator = new DefaultJwtAuthenticator(fakeApplication, fakeDataStore);
            authenticator.WithLocalValidation();

            // Should not throw
            var result = await authenticator.AuthenticateAsync(request);
            result.Jwt.ShouldBe(ValidAccessToken);
        }

        [Fact]
        public async Task Refresh_token_cannot_be_used_as_access_token()
        {
            var fakeDataStore = GetFakeDataStore();
            var fakeApplication = await fakeDataStore.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var request = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(ValidRefreshToken)
                .Build();

            IJwtAuthenticator authenticator = new DefaultJwtAuthenticator(fakeApplication, fakeDataStore);
            authenticator.WithLocalValidation();

            await Should.ThrowAsync<InvalidJwtException>(authenticator.AuthenticateAsync(request));
        }
    }
}
