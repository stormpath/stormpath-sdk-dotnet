// <copyright file="DefaultClientBuilderTests.cs" company="Stormpath, Inc.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Client;

namespace Stormpath.SDK.Tests.Impl.Client
{
    [TestClass]
    public class DefaultClientBuilderTests
    {
        private IClientBuilder builder;

        [TestInitialize]
        public void Setup()
        {
            this.builder = new DefaultClientBuilder();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("Impl.Builders")]
        public void With_missing_API_key()
        {
            var client = builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl("http://foobar")
                .SetConnectionTimeout(10)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("Impl.Builders")]
        public void With_invalid_API_key()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(false);

            builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl("http://foobar")
                .SetConnectionTimeout(10)
                .Build();
        }

        [TestMethod]
        [TestCategory("Impl.Builders")]
        public void AuthenticationScheme_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = builder
                .SetApiKey(fakeKey)
                .SetBaseUrl("http://foobar")
                .SetConnectionTimeout(10)
                .Build();

            // Null is ok! (a default is selected in DefaultRequestAuthenticatorFactory)
            client.AuthenticationScheme.ShouldBe(null);
        }

        [TestMethod]
        [TestCategory("Impl.Builders")]
        public void BaseUrl_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = builder
                .SetApiKey(fakeKey)
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetConnectionTimeout(10)
                .Build();

            // Default value
            client.BaseUrl.ShouldBe("https://api.stormpath.com/v1");
        }

        [TestMethod]
        [TestCategory("Impl.Builders")]
        public void ConnectionTimeout_is_optional()
        {
            var fakeKey = Substitute.For<IClientApiKey>();
            fakeKey.IsValid().Returns(true);

            var client = builder
                .SetApiKey(fakeKey)
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl("http://foobar")
                .Build();

            // Default value
            client.ConnectionTimeout.ShouldBe(20 * 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("Impl.Builders")]
        public void BaseUrl_cannot_be_empty()
        {
            var client = builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(string.Empty)
                .SetConnectionTimeout(10)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("Impl.Builders")]
        public void ConnectionTimeout_cannot_be_negative()
        {
            var client = builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(string.Empty)
                .SetConnectionTimeout(-1)
                .Build();
        }
    }
}
