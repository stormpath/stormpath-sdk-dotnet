// <copyright file="FormUrlEncoder_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class FormUrlEncoder_tests
    {
        [Fact]
        public void Encodes_password_grant_attempt()
        {
            var dataStore = TestDataStore.Create();

            var createGrantAttempt = dataStore.Instantiate<IPasswordGrantAuthenticationAttempt>();
            createGrantAttempt.SetLogin("tom@stormpath.com");
            createGrantAttempt.SetPassword("Secret1");
            createGrantAttempt.SetAccountStore("https://api.stormpath.com/v1/directories/1bcd23ec1d0a8wa6");

            var properties = (createGrantAttempt as AbstractResource).GetResourceData().GetUpdatedProperties().ToDictionary();
            var result = new FormUrlEncoder(properties)
                .ToString()
                .Split('&');

            result.ShouldContain("grant_type=password");
            result.ShouldContain("username=tom%40stormpath.com");
            result.ShouldContain("password=Secret1");
            result.ShouldContain("accountStore=https%3A%2F%2Fapi.stormpath.com%2Fv1%2Fdirectories%2F1bcd23ec1d0a8wa6");
        }
    }
}
