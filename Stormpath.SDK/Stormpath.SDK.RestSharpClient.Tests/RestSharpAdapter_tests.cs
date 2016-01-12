// <copyright file="RestSharpAdapter_tests.cs" company="Stormpath, Inc.">
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

using NSubstitute;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Extensions.Http.RestSharpClient.Tests
{
    public class RestSharpAdapter_tests
    {
        private readonly string baseUrl = "http://api.foo.bar/v1";

        [Fact]
        public void Get_request_is_converted_properly()
        {
            var headers = new SDK.Http.HttpHeaders();
            headers.Authorization = new SDK.Http.AuthorizationHeaderValue("Basic", "foobarabc123");

            var request = Substitute.For<SDK.Http.IHttpRequest>();
            request.Method.Returns(SDK.Http.HttpMethod.Get);
            request.Body.Returns(string.Empty);
            request.BodyContentType.Returns(string.Empty);
            request.CanonicalUri.Returns(new SDK.Http.CanonicalUri("http://api.foo.bar/v1/baz"));
            request.Headers.Returns(headers);

            var adapter = new RestSharpAdapter();
            var convertedRequest = adapter.ToRestRequest(this.baseUrl, request);

            convertedRequest.Method.ShouldBe(RestSharp.Method.GET);
            convertedRequest.RequestFormat.ShouldBe(RestSharp.DataFormat.Json);
            convertedRequest.Resource.ShouldBe("/baz");

            convertedRequest.Parameters.ShouldContain(p =>
                p.Type == RestSharp.ParameterType.HttpHeader &&
                p.Name == "Authorization" &&
                (string)p.Value == "Basic foobarabc123");
        }

        [Fact]
        public void Post_request_is_converted_properly()
        {
            var headers = new SDK.Http.HttpHeaders();
            headers.Authorization = new SDK.Http.AuthorizationHeaderValue("Basic", "foobarabc123");

            var request = Substitute.For<SDK.Http.IHttpRequest>();
            request.Method.Returns(SDK.Http.HttpMethod.Post);

            var fakeBody = "{ 'data1': true, 'data2': 'foobar' }";
            request.Body.Returns(fakeBody);
            request.BodyContentType.Returns(string.Empty);
            request.HasBody.Returns(true);

            request.CanonicalUri.Returns(new SDK.Http.CanonicalUri("http://api.foo.bar/v1/baz"));
            request.Headers.Returns(headers);

            var adapter = new RestSharpAdapter();
            var convertedRequest = adapter.ToRestRequest(this.baseUrl, request);

            convertedRequest.Method.ShouldBe(RestSharp.Method.POST);
            convertedRequest.RequestFormat.ShouldBe(RestSharp.DataFormat.Json);
            convertedRequest.Resource.ShouldBe("/baz");

            convertedRequest.Parameters.ShouldContain(p =>
                p.Type == RestSharp.ParameterType.HttpHeader &&
                p.Name == "Authorization" &&
                (string)p.Value == "Basic foobarabc123");

            convertedRequest.Parameters.ShouldContain(p =>
                p.Type == RestSharp.ParameterType.RequestBody &&
                (string)p.Value == fakeBody);
        }

        [Fact]
        public void Resource_URLs_are_constructed_properly()
        {
            var request = Substitute.For<SDK.Http.IHttpRequest>();
            request.Method.Returns(SDK.Http.HttpMethod.Put);
            request.CanonicalUri.Returns(new SDK.Http.CanonicalUri("http://api.foo.bar/v1/baz"));

            var adapter = new RestSharpAdapter();
            var convertedRequest = adapter.ToRestRequest("http://api.foo.bar/v1", request);

            convertedRequest.Resource.ShouldBe("/baz");
        }
    }
}
