// <copyright file="DefaultIdSiteAsyncCallbackHandler_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Jwt
{
    public class DefaultIdSiteAsyncCallbackHandler_tests
    {
        [Fact]
        public async Task When_registered()
        {
            var jwtResponse = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1w" +
                            "YXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIi" +
                            "wiYXVkIjoiMkVWNzBBSFJUWUYwSk9BN09FRk8zU00yOSIsImV4cCI6MjUwMjQ2NjY1MDAwLCJpYXQiOjE0MDcxOTg1NTAsImp0aSI6" +
                            "IjQzNnZra0hnazF4MzA1N3BDUHFUYWgiLCJpcnQiOiIxZDAyZDMzNS1mYmZjLTRlYTgtYjgzNi04NWI5ZTJhNmYyYTAiLCJpc05ld1" +
                            "N1YiI6ZmFsc2UsInN0YXR1cyI6IlJFR0lTVEVSRUQifQ.4_yCiF6Cik2wep3iwyinTTcn5GHAEvCbIezO1aA5Kkk";
            await this.TestListener(jwtResponse, IdSiteResultStatus.Registered);
        }

        private async Task TestListener(string jwtResponse, IdSiteResultStatus expectedStatus)
        {
            IAccountResult accountResultFromListener = null;

            // Wire up dummy handler
            var stubListener = Substitute.For<IIdSiteResultAsyncListener>();
            stubListener
                .When(x => x.OnAuthenticatedAsync(Arg.Any<IAuthenticationResult>()))
                .Do(x =>
                {
                    if (expectedStatus == IdSiteResultStatus.Authenticated)
                        accountResultFromListener = x.Arg<IAuthenticationResult>();
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                });
            stubListener
                .When(x => x.OnLogoutAsync(Arg.Any<ILogoutResult>()))
                .Do(x =>
                {
                    if (expectedStatus == IdSiteResultStatus.Logout)
                        accountResultFromListener = x.Arg<ILogoutResult>();
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                });
            stubListener
                .When(x => x.OnRegisteredAsync(Arg.Any<IRegistrationResult>()))
                .Do(x =>
                {
                    if (expectedStatus == IdSiteResultStatus.Registered)
                        accountResultFromListener = x.Arg<IRegistrationResult>();
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                });

            var testApiKey = ClientApiKeys.Builder().SetId("2EV70AHRTYF0JOA7OEFO3SM29").SetSecret("goPUHQMkS4dlKwl5wtbNd91I+UrRehCsEDJrIrMruK8").Build();
            var fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            fakeRequestExecutor.ApiKey.Returns(testApiKey);

            var dataStore = new DefaultDataStore(
                fakeRequestExecutor,
                "https://api.stormpath.com/v1",
                new JsonNetSerializer(),
                new NullLogger(),
                Caches.NewDisabledCacheProvider(),
                TimeSpan.FromMinutes(10));

            var application = Substitute.For<IApplication>();
            var request = new DefaultHttpRequest(HttpMethod.Get, new CanonicalUri($"https://foo.bar?{IdSiteClaims.JwtResponse}={jwtResponse}"));

            IIdSiteAsyncCallbackHandler callbackHandler = new DefaultIdSiteAsyncCallbackHandler(dataStore, application, request);
            callbackHandler.SetResultListener(stubListener);

            var accountResult = await callbackHandler.ProcessRequestAsync(CancellationToken.None);

            // Verify some things
            throw new NotImplementedException();
        }
    }
}
