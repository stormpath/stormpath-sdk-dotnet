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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultRequestExecutorBuilder : IRequestExecutorBuilder
    {
        private Type requestExecutorType;
        private IClientApiKey apiKey;
        private AuthenticationScheme authScheme;
        private int connectionTimeout;
        private ILogger logger;

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
            // if type is set, use that, then construct
            throw new NotImplementedException();
        }

        private IRequestExecutor Instantiate(Type targetType)
        {
            // match parameters up and create object
            throw new NotImplementedException();
        }
    }
}
