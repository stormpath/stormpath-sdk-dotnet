// <copyright file="DefaultIdSiteAsyncCallbackHandler_exception_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.IdSite;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Jwt
{
    public class DefaultIdSiteAsyncCallbackHandler_exception_tests : IDisposable
    {
        private IInternalDataStore dataStore;

        public void Dispose()
        {
            this.dataStore.Dispose();
        }

        private static readonly string UnknownOrganizationException =
            "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcn" +
            "IiOnsiY29kZSI6MTEwMDEsImRldmVsb3Blck1lc3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIGJlY2F1c2UgdGhlIHNwZWNpZmllZCBv" +
            "cmdhbml6YXRpb24gbmFtZSBrZXkgZG9lcyBub3QgZXhpc3QgaW4geW91ciBTdG9ybXBhdGggVGVuYW50LiIsIm1lc3NhZ2UiOiJUb2" +
            "tlbiBpcyBpbnZhbGlkIiwibW9yZUluZm8iOiJodHRwOi8vZG9jcy5zdG9ybXBhdGguY29tL2Vycm9ycy8xMDAxMSIsInN0YXR1cyI6" +
            "NDAwfSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hcHBsaWNhdGlvbnMvMmVxYnlaOHFvMzRlREU0Z1RvMVI5My" +
            "IsImV4cCI6MzM1MDI0NjY2NTAwMCwiaWF0IjoxNDA3MTk4NTUwLCJqdGkiOiI0MzZ2a2tIZ2sxeDMwNTdwQ1BxVGFoIn0.SDf6NM5S" +
            "10fW7OiGwjcAEqWEPU-nd6YDkOZGBmw8G18";

        public static IEnumerable<object[]> ExceptionTests()
        {
            yield return new object[] { nameof(UnknownOrganizationException), UnknownOrganizationException, 11001, 400, "Token is invalid", "Token is invalid because the specified organization name key does not exist in your Stormpath Tenant." };
        }

        [Theory]
        [MemberData(nameof(ExceptionTests))]
        public async Task Handle_error(string id_, string jwtResponse, int expectedCode, int expectedStatus, string expectedMessage, string expectedDeveloperMessage)
        {
            var testApiKey = ClientApiKeys.Builder().SetId("2EV70AHRTYF0JOA7OEFO3SM29").SetSecret("goPUHQMkS4dlKwl5wtbNd91I+UrRehCsEDJrIrMruK8").Build();
            var fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            fakeRequestExecutor.ApiKey.Returns(testApiKey);

            this.dataStore = new DefaultDataStore(
                fakeRequestExecutor,
                "https://api.stormpath.com/v1",
                new JsonNetSerializer(),
                new NullLogger(),
                Caches.NewInMemoryCacheProvider().Build(),
                TimeSpan.FromMinutes(10));

            var request = new DefaultHttpRequest(HttpMethod.Get, new CanonicalUri($"https://foo.bar?{IdSiteClaims.JwtResponse}={jwtResponse}"));

            IIdSiteAsyncCallbackHandler callbackHandler = new DefaultIdSiteAsyncCallbackHandler(this.dataStore, request);

            try
            {
                var accountResult = await callbackHandler.GetAccountResultAsync(CancellationToken.None);

                throw new Exception("Should not reach here. Proper exception was not thrown.");
            }
            catch (IdSiteRuntimeException e)
            {
                e.Code.ShouldBe(expectedCode);
                e.HttpStatus.ShouldBe(expectedStatus);
                e.Message.ShouldBe(expectedMessage);
                e.DeveloperMessage.ShouldBe(expectedDeveloperMessage);
            }
        }
    }
}
