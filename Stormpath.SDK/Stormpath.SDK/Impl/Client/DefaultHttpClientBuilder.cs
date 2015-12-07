// <copyright file="DefaultHttpClientBuilder.cs" company="Stormpath, Inc.">
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
using System.Net;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultHttpClientBuilder : IHttpClientBuilder
    {
        private readonly ITypeLoader<IHttpClient> defaultLibraryLoader;

        private IHttpClient instance;
        private string baseUrl;
        private int connectionTimeout;
        private IWebProxy proxy;
        private ILogger logger;

        public DefaultHttpClientBuilder()
            : this(new DefaultHttpClientLoader())
        {
        }

        internal DefaultHttpClientBuilder(ITypeLoader<IHttpClient> defaultLibraryLoader)
        {
            this.defaultLibraryLoader = defaultLibraryLoader;
        }

        IHttpClientBuilder IHttpClientBuilder.SetHttpClient(IHttpClient client)
        {
            this.instance = client;
            return this;
        }

        IHttpClientBuilder IHttpClientBuilder.SetBaseUrl(string baseUrl)
        {
            this.baseUrl = baseUrl;
            return this;
        }

        IHttpClientBuilder IHttpClientBuilder.SetConnectionTimeout(int connectionTimeout)
        {
            this.connectionTimeout = connectionTimeout;
            return this;
        }

        IHttpClientBuilder IHttpClientBuilder.SetProxy(IWebProxy proxy)
        {
            this.proxy = proxy;
            return this;
        }

        IHttpClientBuilder ILoggerConsumer<IHttpClientBuilder>.SetLogger(ILogger logger)
        {
            if (logger != null)
            {
                this.logger = logger;
            }

            return this;
        }

        IHttpClient IHttpClientBuilder.Build()
        {
            if (this.instance != null)
            {
                return this.instance;
            }

            IHttpClient defaultClient = null;
            bool foundDefaultLibrary = this.defaultLibraryLoader.TryLoad(out defaultClient, new object[] { this.baseUrl, this.connectionTimeout, this.proxy, this.logger });
            if (!foundDefaultLibrary)
            {
                throw new ApplicationException("Could not find a valid HTTP client. Include Stormpath.SDK.RestSharpClient.dll in the application path.");
            }

            return defaultClient;
        }
    }
}
