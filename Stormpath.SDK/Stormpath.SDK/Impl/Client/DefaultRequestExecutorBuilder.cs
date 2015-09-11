// <copyright file="DefaultRequestExecutorBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultRequestExecutorBuilder : IRequestExecutorBuilder
    {
        private readonly IRequestExecutorLoader defaultLibraryLoader;

        private Type requestExecutorType;
        private IClientApiKey apiKey;
        private AuthenticationScheme authScheme;
        private int connectionTimeout;
        private ILogger logger;

        public DefaultRequestExecutorBuilder()
            : this(new DefaultRequestExecutorLoader())
        {
        }

        internal DefaultRequestExecutorBuilder(IRequestExecutorLoader defaultLibraryLoader)
        {
            this.defaultLibraryLoader = defaultLibraryLoader;
        }

        IRequestExecutorBuilder IRequestExecutorBuilder.SetRequestExecutorType(Type requestExecutorType)
        {
            this.requestExecutorType = requestExecutorType;
            return this;
        }

        IRequestExecutorBuilder IRequestExecutorBuilder.SetApiKey(IClientApiKey apiKey)
        {
            this.apiKey = apiKey;
            return this;
        }

        IRequestExecutorBuilder IRequestExecutorBuilder.SetAuthenticationScheme(AuthenticationScheme authScheme)
        {
            this.authScheme = authScheme;
            return this;
        }

        IRequestExecutorBuilder IRequestExecutorBuilder.SetConnectionTimeout(int connectionTimeout)
        {
            this.connectionTimeout = connectionTimeout;
            return this;
        }

        IRequestExecutorBuilder IRequestExecutorBuilder.SetLogger(ILogger logger)
        {
            this.logger = logger;
            return this;
        }

        IRequestExecutor IRequestExecutorBuilder.Build()
        {
            Type requestExecutorType = null;

            if (this.requestExecutorType != null)
            {
                requestExecutorType = this.requestExecutorType;
            }
            else
            {
                bool foundDefaultLibrary = this.defaultLibraryLoader.TryLoad(out requestExecutorType);
                if (!foundDefaultLibrary)
                    throw new ApplicationException("Could not find a valid HTTP client. Include Stormpath.SDK.RestSharpClient.dll in the application path.");
            }

            try
            {
                var instance = this.Instantiate(requestExecutorType);
                if (instance == null)
                    throw new ApplicationException("Created instance was null");

                return instance;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not create a valid HTTP client. See the inner exception for details.", e);
            }
        }

        private IRequestExecutor Instantiate(Type targetType)
        {
            return Activator.CreateInstance(targetType, this.apiKey, this.authScheme, this.connectionTimeout, this.logger) as IRequestExecutor;
        }
    }
}
