// <copyright file="AbstractHttpClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class AbstractHttpClientBuilder_tests
    {
        [Fact]
        public void Constructs_instance_from_specified_type()
        {
            IHttpClientBuilder builder = new AbstractHttpClientBuilder<DummyHttpClient>();

            var fakeUrl = "http://foo.bar";
            var fakeTimeout = 101;
            var fakeProxy = Substitute.For<System.Net.IWebProxy>();
            var fakeLogger = Substitute.For<ILogger>();

            builder.SetBaseUrl(fakeUrl);
            builder.SetConnectionTimeout(fakeTimeout);
            builder.SetLogger(fakeLogger);
            builder.SetProxy(fakeProxy);

            var instance = builder.Build();
            instance.ShouldBeAssignableTo<IHttpClient>();
            instance.ShouldNotBeNull();
            instance.BaseUrl.ShouldBe(fakeUrl);
            instance.ConnectionTimeout.ShouldBe(fakeTimeout);
            instance.Proxy.ShouldBe(fakeProxy);
            (instance as DummyHttpClient).Logger.ShouldBe(fakeLogger);
        }

        [Fact]
        public void Throws_when_type_is_null()
        {
            Should.Throw<NotSupportedException>(() => new AbstractHttpClientBuilder<IHttpClient>(null));
        }

        public class DummyHttpClient : IHttpClient
        {
            public string BaseUrl { get; private set; }

            public int ConnectionTimeout { get; private set; }

            public System.Net.IWebProxy Proxy { get; private set; }

            public ILogger Logger { get; private set; }

            public bool IsSynchronousSupported => false;

            public bool IsAsynchronousSupported => false;

            public DummyHttpClient(string baseUrl, int timeout, System.Net.IWebProxy proxy, ILogger logger)
            {
                this.BaseUrl = baseUrl;
                this.ConnectionTimeout = timeout;
                this.Proxy = proxy;
                this.Logger = logger;
            }

            public void Dispose()
            {
            }
        }
    }
}
