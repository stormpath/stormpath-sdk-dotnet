// <copyright file="AbstractMockHttpClient.cs" company="Stormpath, Inc.">
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
    public abstract class AbstractMockHttpClient : IAsynchronousHttpClient, ISynchronousHttpClient
    {
        protected readonly string baseUrl;
        private readonly IDictionary<string, IHttpResponse> responses;

        private List<IHttpRequest> calls;

        public AbstractMockHttpClient(string baseUrl)
        {
            baseUrl = EnsureTrailingSlash(baseUrl);

            this.baseUrl = baseUrl;
            this.calls = new List<IHttpRequest>();
        }

        private ISynchronousHttpClient AsSyncInterface => this;

        string IHttpClient.BaseUrl => this.baseUrl;

        int IHttpClient.ConnectionTimeout => 0;

        bool IHttpClient.IsAsynchronousSupported => true;

        bool IHttpClient.IsSynchronousSupported => true;

        IWebProxy IHttpClient.Proxy => null;

        public IReadOnlyList<IHttpRequest> Calls => this.calls;

        void IDisposable.Dispose()
        {
        }

        protected abstract bool IsSupported(HttpMethod method);

        protected abstract IHttpResponse GetResponse(IHttpRequest request);

        IHttpResponse ISynchronousHttpClient.Execute(IHttpRequest request)
        {
            if (!MatchesBaseUrl(request, this.baseUrl))
            {
                throw new ArgumentException("BaseUrl does not match.");
            }

            if (!this.IsSupported(request.Method))
            {
                throw new NotImplementedException($"The method {request.Method} is not supported by {this.GetType().Name}.");
            }

            var response = this.GetResponse(request);
            if (response == null)
            {
                throw new ApplicationException($"{this.GetType().Name} cannot handle this request.");
            }

            this.calls.Add(request);
            return response;
        }

        Task<IHttpResponse> IAsynchronousHttpClient.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.AsSyncInterface.Execute(request));
        }

        private static string EnsureTrailingSlash(string url)
        {
            if (!url.EndsWith("/"))
            {
                url = url + "/";
            }

            return url;
        }

        private static bool MatchesBaseUrl(IHttpRequest request, string baseUrl)
            => request.CanonicalUri.ToString().StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase);

        protected static string MergeWithBaseUrl(string baseUrl, string resourceUrl)
            => baseUrl + resourceUrl.Replace(baseUrl, string.Empty).TrimStart('/');
    }
}
