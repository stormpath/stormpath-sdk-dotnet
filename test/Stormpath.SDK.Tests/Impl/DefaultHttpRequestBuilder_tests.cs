// <copyright file="DefaultHttpRequestBuilder_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultHttpRequestBuilder_tests
    {
        [Fact]
        public void Throws_if_missing_method()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithUri("http://foo.bar/1");
            builder.WithBody("foobar!");

            Should.Throw<ArgumentNullException>(() =>
            {
                builder.Build();
            });
        }

        [Fact]
        public void Throws_if_missing_uri()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod("GET");
            builder.WithBody("foobar!");

            Should.Throw<ArgumentNullException>(() =>
            {
                builder.Build();
            });
        }

        [Fact]
        public void Setting_method_by_string()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod("GET");
            builder.WithUri("http://foo.bar/1");
            var result = builder.Build();

            result.Method.ShouldBe(HttpMethod.Get);
        }

        [Fact]
        public void Setting_method_by_enum()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1");
            var result = builder.Build();

            result.Method.ShouldBe(HttpMethod.Get);
        }

        [Fact]
        public void Setting_uri_by_string()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1");
            var result = builder.Build();

            result.CanonicalUri.ToString().ShouldBe("http://foo.bar/1");
        }

        [Fact]
        public void Setting_uri_by_uri()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri(new Uri("http://foo.bar/1"));
            var result = builder.Build();

            result.CanonicalUri.ToString().ShouldBe("http://foo.bar/1");
        }

        [Fact]
        public void Setting_uri_with_query_string()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1?qux=baz");
            var result = builder.Build();

            result.CanonicalUri.ToString().ShouldBe("http://foo.bar/1?qux=baz");
        }

        [Fact]
        public void Setting_headers()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1?qux=baz");

            var fakeHeaders = new Dictionary<string, object>() { ["Accept"] = "bar", ["baz"] = 123 };
            builder.WithHeaders(fakeHeaders);

            var result = builder.Build();

            result.Headers.Count().ShouldBe(2);
            result.Headers.Accept.ShouldBe("bar");
        }

        [Fact]
        public void Setting_body()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1?qux=baz");
            builder.WithBody("foobar!");
            var result = builder.Build();

            result.Body.ShouldBe("foobar!");
            result.HasBody.ShouldBeTrue();
        }

        [Fact]
        public void Setting_body_content_type()
        {
            IHttpRequestBuilder builder = new DefaultHttpRequestBuilder();

            builder.WithMethod(HttpMethod.Get);
            builder.WithUri("http://foo.bar/1?qux=baz");
            builder.WithBodyContentType("application/foobar");
            var result = builder.Build();

            result.BodyContentType.ShouldBe("application/foobar");
            result.HasBody.ShouldBeFalse();
        }
    }
}
