// <copyright file="DefaultClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultClientBuilder_tests
    {
        private IClientBuilder builder;

        public DefaultClientBuilder_tests()
        {
            this.builder = new DefaultClientBuilder();
        }

        [Fact]
        public void Throws_for_missing_API_key()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var client = this.builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl("http://foobar")
                .SetConnectionTimeout(10)
                .Build();
            });
        }

        [Fact]
        public void Throws_for_invalid_API_key()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var fakeKey = Substitute.For<IClientApiKey>();
                fakeKey.IsValid().Returns(false);

                this.builder
                    .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                    .SetBaseUrl("http://foobar")
                    .SetConnectionTimeout(10)
                    .Build();
            });
        }

        [Fact]
        public void AuthenticationScheme_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = this.builder
                .SetApiKey(fakeKey)
                .SetBaseUrl("http://foobar")
                .SetConnectionTimeout(10)
                .Build();

            // Null is ok! (a default is selected in DefaultRequestAuthenticatorFactory)
            client.AuthenticationScheme.ShouldBe(null);
        }

        [Fact]
        public void BaseUrl_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = this.builder
                .SetApiKey(fakeKey)
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetConnectionTimeout(10)
                .Build();

            // Default value
            client.BaseUrl.ShouldBe("https://api.stormpath.com/v1");
        }

        [Fact]
        public void ConnectionTimeout_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = this.builder
                .SetApiKey(fakeKey)
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl("http://foobar")
                .Build();

            // Default value
            client.ConnectionTimeout.ShouldBe(20 * 1000);
        }

        [Fact]
        public void Throws_when_BaseUrl_is_empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = this.builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(string.Empty)
                .SetConnectionTimeout(10)
                .Build();
            });
        }

        [Fact]
        public void Throws_when_ConnectionTimeout_is_negative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var client = this.builder
                    .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                    .SetBaseUrl("foobar")
                    .SetConnectionTimeout(-1)
                    .Build();
            });
        }

        [Fact]
        public void Throws_when_no_JSON_serializer_can_be_found()
        {
            IJsonSerializer dummy = null;
            var fakeLoader = Substitute.For<IJsonSerializerLoader>();
            fakeLoader
                .TryLoad(out dummy)
                .Returns(false);

            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            IClientBuilder builder = new DefaultClientBuilder(fakeLoader);

            Assert.Throws<ApplicationException>(() =>
            {
                var client = builder
                    .SetApiKey(fakeKey)
                    .Build();
            });
        }

        [Fact]
        public void Throws_when_passed_cache_provider_is_null()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = this.builder
                    .SetApiKey(fakeKey);
                (this.builder as DefaultClientBuilder)
                    .SetCache(null)
                    .Build();
            });
        }
    }
}
