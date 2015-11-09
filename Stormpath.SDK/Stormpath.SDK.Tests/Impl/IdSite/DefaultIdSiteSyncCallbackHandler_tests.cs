// <copyright file="DefaultIdSiteSyncCallbackHandler_tests.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Tests.Impl.IdSite
{
    public class DefaultIdSiteSyncCallbackHandler_tests : IDisposable
    {
        private IInternalDataStore dataStore;

        public void Dispose()
        {
            this.dataStore.Dispose();
        }

        private static readonly string RegisteredResponse =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1w" +
            "YXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIi" +
            "wiYXVkIjoiMkVWNzBBSFJUWUYwSk9BN09FRk8zU00yOSIsImV4cCI6MjUwMjQ2NjY1MDAwLCJpYXQiOjE0MDcxOTg1NTAsImp0aSI6" +
            "IjQzNnZra0hnazF4MzA1N3BDUHFUYWgiLCJpcnQiOiIxZDAyZDMzNS1mYmZjLTRlYTgtYjgzNi04NWI5ZTJhNmYyYTAiLCJpc05ld1" +
            "N1YiI6ZmFsc2UsInN0YXR1cyI6IlJFR0lTVEVSRUQifQ.4_yCiF6Cik2wep3iwyinTTcn5GHAEvCbIezO1aA5Kkk";

        private static readonly string AuthenticatedResponse = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1w" +
            "YXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIi" +
            "wiYXVkIjoiMkVWNzBBSFJUWUYwSk9BN09FRk8zU00yOSIsImV4cCI6MjUwMjQ2NjY1MDAwLCJpYXQiOjE0MDcxOTg1NTAsImp0aSI6" +
            "IjQzNnZra0hnazF4MzA1N3BDUHFUYWgiLCJpcnQiOiIxZDAyZDMzNS1mYmZjLTRlYTgtYjgzNi04NWI5ZTJhNmYyYTAiLCJpc05ld1" +
            "N1YiI6ZmFsc2UsInN0YXR1cyI6IkFVVEhFTlRJQ0FURUQifQ.rpp0lsM1JDFeqkrOdwrtYOB1aitnLwhJuH3iaeuLIXY";

        private static readonly string LogoutResponse = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3N0dXJkeS1zaGllbGQuaWQuc3Rvcm1w" +
            "YXRoLmlvIiwic3ViIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hY2NvdW50cy83T3JhOEtmVkRFSVFQMzhLenJZZEFzIi" +
            "wiYXVkIjoiMkVWNzBBSFJUWUYwSk9BN09FRk8zU00yOSIsImV4cCI6MjUwMjQ2NjY1MDAwLCJpYXQiOjE0MDcxOTg1NTAsImp0aSI6" +
            "IjQzNnZra0hnazF4MzA1N3BDUHFUYWgiLCJpcnQiOiIxZDAyZDMzNS1mYmZjLTRlYTgtYjgzNi04NWI5ZTJhNmYyYTAiLCJpc05ld1" +
            "N1YiI6ZmFsc2UsInN0YXR1cyI6IkxPR09VVCJ9.T6ClI4znHCElk1gMQoBpVvE9Jc5Vf4BEjrQ0IWvKYIc";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { nameof(RegisteredResponse), RegisteredResponse, IdSiteResultStatus.Registered.Value, false, null };
            yield return new object[] { nameof(AuthenticatedResponse), AuthenticatedResponse, IdSiteResultStatus.Authenticated.Value, false, null };
            yield return new object[] { nameof(LogoutResponse), LogoutResponse, IdSiteResultStatus.Logout.Value, false, null };
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Handle_response(string id_, string jwtResponse, string expectedStatus, bool isNewAccount, string expectedState)
        {
            IAccountResult accountResultFromListener = null;

            var listener = new InlineIdSiteSyncResultListener(
                onAuthenticated: result =>
                {
                    if (expectedStatus == IdSiteResultStatus.Authenticated)
                        accountResultFromListener = result;
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                },
                onLogout: result =>
                {
                    if (expectedStatus == IdSiteResultStatus.Logout)
                        accountResultFromListener = result;
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                },
                onRegistered: result =>
                {
                    if (expectedStatus == IdSiteResultStatus.Registered)
                        accountResultFromListener = result;
                    else
                        throw new InvalidOperationException("This method should not have been executed");
                });

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

            IIdSiteSyncCallbackHandler callbackHandler = new DefaultIdSiteSyncCallbackHandler(this.dataStore, request);
            callbackHandler.SetResultListener(listener);

            var accountResult = callbackHandler.GetAccountResult();

            // Validate result
            (accountResult as DefaultAccountResult).Account.Href.ShouldBe("https://api.stormpath.com/v1/accounts/7Ora8KfVDEIQP38KzrYdAs");
            (accountResultFromListener as DefaultAccountResult).Account.Href.ShouldBe("https://api.stormpath.com/v1/accounts/7Ora8KfVDEIQP38KzrYdAs");
            accountResult.IsNewAccount.ShouldBe(isNewAccount);
            accountResultFromListener.IsNewAccount.ShouldBe(isNewAccount);
            accountResult.State.ShouldBe(expectedState);
            accountResultFromListener.State.ShouldBe(expectedState);
        }
    }
}
