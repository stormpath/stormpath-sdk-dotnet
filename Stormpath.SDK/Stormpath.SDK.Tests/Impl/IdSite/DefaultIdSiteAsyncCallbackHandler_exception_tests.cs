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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Logging;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.IdSite
{
    public class DefaultIdSiteAsyncCallbackHandler_exception_tests : IDisposable
    {
        private IInternalDataStore dataStore;

        public void Dispose()
        {
            this.dataStore.Dispose();
        }

        private static readonly string UnknownOrganization =
            "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcn" +
            "IiOnsiY29kZSI6MTEwMDEsImRldmVsb3Blck1lc3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIGJlY2F1c2UgdGhlIHNwZWNpZmllZCBv" +
            "cmdhbml6YXRpb24gbmFtZSBrZXkgZG9lcyBub3QgZXhpc3QgaW4geW91ciBTdG9ybXBhdGggVGVuYW50LiIsIm1lc3NhZ2UiOiJUb2" +
            "tlbiBpcyBpbnZhbGlkIiwibW9yZUluZm8iOiJodHRwOi8vZG9jcy5zdG9ybXBhdGguY29tL2Vycm9ycy8xMDAxMSIsInN0YXR1cyI6" +
            "NDAwfSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hcHBsaWNhdGlvbnMvMmVxYnlaOHFvMzRlREU0Z1RvMVI5My" +
            "IsImV4cCI6MzM1MDI0NjY2NTAwMCwiaWF0IjoxNDA3MTk4NTUwLCJqdGkiOiI0MzZ2a2tIZ2sxeDMwNTdwQ1BxVGFoIn0.SDf6NM5S" +
            "10fW7OiGwjcAEqWEPU-nd6YDkOZGBmw8G18";

        private static readonly string IatAfterCurrentTime = "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcn" +
            "IiOnsiY29kZSI6MTAwMTIsImRldmVsb3Blck1lc3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIGJlY2F1c2UgdGhlIGlzc3VlZCBhdCB0" +
            "aW1lIChpYXQpIGlzIGFmdGVyIHRoZSBjdXJyZW50IHRpbWUuIiwibWVzc2FnZSI6IlRva2VuIGlzIGludmFsaWQiLCJtb3JlSW5mby" +
            "I6Imh0dHA6Ly9kb2NzLnN0b3JtcGF0aC5jb20vZXJyb3JzLzEwMDEyIiwic3RhdHVzIjo0MDB9LCJpc3MiOiJodHRwczovL2FwaS5z" +
            "dG9ybXBhdGguY29tL3YxL2FwcGxpY2F0aW9ucy8yZXFieVo4cW8zNGVERTRnVG8xUjkzIiwiZXhwIjozMzUwMjQ2NjY1MDAwLCJpYX" +
            "QiOjE0MDcxOTg1NTAsImp0aSI6IjQzNnZra0hnazF4MzA1N3BDUHFUYWgifQ.JT__dR0jC6fYZv9NYVC4k45mD5fAQfl_l7yElYm5JMk";

        private static readonly string OrganizationNameKeyNotAssigned = "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcnIiOnsiY" +
            "29kZSI6MTEwMDMsImRldmVsb3Blck1lc3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIGJlY2F1c2UgdGhlIHNwZWNpZmllZCBvcmdhbml" +
            "6YXRpb24gbmFtZUtleSBpcyBub3Qgb25lIG9mIHRoZSBhcHBsaWNhdGlvbidzIGFzc2lnbmVkIGFjY291bnQgc3RvcmVzLiIsIm1lc" +
            "3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIiwibW9yZUluZm8iOiJodHRwOi8vZG9jcy5zdG9ybXBhdGguY29tL2Vycm9ycy8xMTAwMyI" +
            "sInN0YXR1cyI6NDAwfSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hcHBsaWNhdGlvbnMvMmVxYnlaOHFvMzRlR" +
            "EU0Z1RvMVI5MyIsImV4cCI6MzM1MDI0NjY2NTAwMCwiaWF0IjoxNDA3MTk4NTUwLCJqdGkiOiI0MzZ2a2tIZ2sxeDMwNTdwQ1BxVGF" +
            "oIn0.rN7yWI1v9IzsOuooe3cC1WKM4vpqB_vsa00mnXvj3nw";

        private static readonly string SessionTimedOut = "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcnIiOnsiY" +
            "29kZSI6MTIwMDEsImRldmVsb3Blck1lc3NhZ2UiOiJUaGUgc2Vzc2lvbiBvbiBJRCBTaXRlIGhhcyB0aW1lZCBvdXQuIFRoaXMgY2F" +
            "uIG9jY3VyIGlmIHRoZSB1c2VyIHN0YXlzIG9uIElEIFNpdGUgd2l0aG91dCBsb2dnaW5nIGluLCByZWdpc3RlcmluZywgb3IgcmVzZ" +
            "XR0aW5nIGEgcGFzc3dvcmQuIiwibWVzc2FnZSI6IlRoZSBzZXNzaW9uIG9uIElEIFNpdGUgaGFzIHRpbWVkIG91dC4iLCJtb3JlSW5" +
            "mbyI6Im1haWx0bzpzdXBwb3J0QHN0b3JtcGF0aC5jb20iLCJzdGF0dXMiOjQwMX0sImlzcyI6Imh0dHBzOi8vYXBpLnN0b3JtcGF0a" +
            "C5jb20vdjEvYXBwbGljYXRpb25zLzJlcWJ5WjhxbzM0ZURFNGdUbzFSOTMiLCJleHAiOjMzNTAyNDY2NjUwMDAsImlhdCI6MTQwNzE" +
            "5ODU1MCwianRpIjoiNDM2dmtrSGdrMXgzMDU3cENQcVRhaCJ9.xuW4L7HPe0M__mVK7jndY6g9Mcnuc1kanw_7bolOK3Y";

        private static readonly string ExpiredJwt = "eyJ0eXAiOiJKV1QiLCJraWQiOiIyRVY3MEFIUlRZRjBKT0E3T0VGTzNTTTI5IiwiYWxnIjoiSFMyNTYifQ.eyJlcn" +
            "IiOnsiY29kZSI6MTEwMDEsImRldmVsb3Blck1lc3NhZ2UiOiJUb2tlbiBpcyBpbnZhbGlkIGJlY2F1c2UgdGhlIHNwZWNpZmllZCBv" +
            "cmdhbml6YXRpb24gbmFtZSBrZXkgZG9lcyBub3QgZXhpc3QgaW4geW91ciBTdG9ybXBhdGggVGVuYW50LiIsIm1lc3NhZ2UiOiJUb2" +
            "tlbiBpcyBpbnZhbGlkIiwibW9yZUluZm8iOiJodHRwOi8vZG9jcy5zdG9ybXBhdGguY29tL2Vycm9ycy8xMDAxMSIsInN0YXR1cyI6" +
            "NDAwfSwiaXNzIjoiaHR0cHM6Ly9hcGkuc3Rvcm1wYXRoLmNvbS92MS9hcHBsaWNhdGlvbnMvMmVxYnlaOHFvMzRlREU0Z1RvMVI5My" +
            "IsImV4cCI6MTQ0MDcwNTA2MCwiaWF0IjoxNDA3MTk4NTUwLCJqdGkiOiI0MzZ2a2tIZ2sxeDMwNTdwQ1BxVGFoIn0.OR7ho9XsZ7rC" +
            "RYGumvw-SO0UzD2kEXg-janTAkxD_bE";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { nameof(UnknownOrganization), UnknownOrganization, typeof(IdSiteRuntimeException), 11001, 400, "Token is invalid", "Token is invalid because the specified organization name key does not exist in your Stormpath Tenant." };
            yield return new object[] { nameof(IatAfterCurrentTime), IatAfterCurrentTime, typeof(InvalidIdSiteTokenException), 10012, 400, "Token is invalid", "Token is invalid because the issued at time (iat) is after the current time." };
            yield return new object[] { nameof(OrganizationNameKeyNotAssigned), OrganizationNameKeyNotAssigned, typeof(InvalidIdSiteTokenException), 11003, 400, "Token is invalid", "Token is invalid because the specified organization nameKey is not one of the application's assigned account stores." };
            yield return new object[] { nameof(SessionTimedOut), SessionTimedOut, typeof(IdSiteSessionTimeoutException), 12001, 401, "The session on ID Site has timed out.", "The session on ID Site has timed out. This can occur if the user stays on ID Site without logging in, registering, or resetting a password." };
            yield return new object[] { nameof(ExpiredJwt), ExpiredJwt, typeof(InvalidJwtException), 0, 0, "The JWT has already expired.", null };
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task Handle_error(string id_, string jwtResponse, Type expectedExceptionType, int expectedCode, int expectedStatus, string expectedMessage, string expectedDeveloperMessage)
        {
            var testApiKey = ClientApiKeys.Builder().SetId("2EV70AHRTYF0JOA7OEFO3SM29").SetSecret("goPUHQMkS4dlKwl5wtbNd91I+UrRehCsEDJrIrMruK8").Build();
            var fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            fakeRequestExecutor.ApiKey.Returns(testApiKey);

            this.dataStore = TestDataStore.Create(fakeRequestExecutor, Caches.NewInMemoryCacheProvider().Build());

            var request = new DefaultHttpRequest(HttpMethod.Get, new CanonicalUri($"https://foo.bar?{IdSiteClaims.JwtResponse}={jwtResponse}"));

            IIdSiteAsyncCallbackHandler callbackHandler = new DefaultIdSiteAsyncCallbackHandler(this.dataStore, request);

            try
            {
                var accountResult = await callbackHandler.GetAccountResultAsync(CancellationToken.None);

                throw new Exception("Should not reach here. Proper exception was not thrown.");
            }
            catch (IdSiteRuntimeException e) when (expectedExceptionType.IsAssignableFrom(e.GetType()))
            {
                e.Code.ShouldBe(expectedCode);
                e.HttpStatus.ShouldBe(expectedStatus);
                e.Message.ShouldBe(expectedMessage);
                e.DeveloperMessage.ShouldBe(expectedDeveloperMessage);
            }
            catch (Exception e) when (expectedExceptionType.IsAssignableFrom(e.GetType()))
            {
                e.Message.ShouldBe(expectedMessage);
            }
        }
    }
}
