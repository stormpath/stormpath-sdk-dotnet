// <copyright file="FakeHttpClient.cs" company="Stormpath, Inc.">
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public sealed class FakeHttpClient : IAsynchronousHttpClient, ISynchronousHttpClient
    {
        private static readonly List<HttpMethod> UnsupportedMethods =
            new List<HttpMethod>() { HttpMethod.Connect, HttpMethod.Head, HttpMethod.Options, HttpMethod.Patch, HttpMethod.Trace };

        private readonly string baseUrl;
        private readonly string defaultResponse;
        private readonly IDictionary<string, IHttpResponse> responses;

        private int calls;

        public FakeHttpClient(string baseUrl, string defaultResponse = null)
        {
            baseUrl = EnsureTrailingSlash(baseUrl);

            this.baseUrl = baseUrl;
            this.defaultResponse = defaultResponse;

            this.responses = new Dictionary<string, IHttpResponse>();
        }

        private ISynchronousHttpClient AsSyncInterface => this;

        string IHttpClient.BaseUrl => this.baseUrl;

        int IHttpClient.ConnectionTimeout => 0;

        bool IHttpClient.IsAsynchronousSupported => true;

        bool IHttpClient.IsSynchronousSupported => true;

        IWebProxy IHttpClient.Proxy => null;

        public int CallCount => this.calls;

        void IDisposable.Dispose()
        {
        }

        public FakeHttpClient SetupGet(string resource, int responseCode, string responseBody, string responseBodyContentType = null)
        {
            var key = CreateKey(HttpMethod.Get, MergeWithBaseUrl(this.baseUrl, resource));
            this.responses[key] = new FakeHttpResponse(responseCode, responseBody, responseBodyContentType);

            return this;
        }

        IHttpResponse ISynchronousHttpClient.Execute(IHttpRequest request)
        {
            if (!MatchesBaseUrl(request, this.baseUrl))
                throw new ArgumentException("BaseUrl does not match.");

            if (UnsupportedMethods.Contains(request.Method))
                throw new NotImplementedException($"The method {request.Method} is not supported by FakeHttpClient.");

            var key = CreateKey(request.Method, request.CanonicalUri.ToString());
            IHttpResponse response = null;
            this.calls++;

            if (this.responses.TryGetValue(key, out response))
                return response;

            return this.DefaultResponse();
        }

        Task<IHttpResponse> IAsynchronousHttpClient.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.AsSyncInterface.Execute(request));
        }

        private IHttpResponse DefaultResponse()
        {
            if (string.IsNullOrEmpty(this.defaultResponse))
                throw new ArgumentException("No default response set up in FakeHttpClient.");

            return new FakeHttpResponse(200, this.defaultResponse);
        }

        private static string CreateKey(HttpMethod method, string url)
            => $"{method} {url}";

        private static string EnsureTrailingSlash(string url)
        {
            if (!url.EndsWith("/"))
                url = url + "/";

            return url;
        }

        private static bool MatchesBaseUrl(IHttpRequest request, string baseUrl)
            => request.CanonicalUri.ToString().StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase);

        private static string MergeWithBaseUrl(string baseUrl, string resourceUrl)
            => baseUrl + resourceUrl.Replace(baseUrl, string.Empty).TrimStart('/');
    }
}
