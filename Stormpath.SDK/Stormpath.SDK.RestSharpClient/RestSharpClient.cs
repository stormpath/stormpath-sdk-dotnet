// <copyright file="RestSharpClient.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Http;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Extensions.Http.RestSharp
{
    public sealed class RestSharpClient : ISynchronousHttpClient, IAsynchronousHttpClient
    {
        private readonly int connectionTimeout;
        private readonly ILogger logger;

        private bool alreadyDisposed = false;

        bool IHttpClient.IsSynchronousSupported => false; // TODO

        bool IHttpClient.IsAsynchronousSupported => false; // TODO

        public RestSharpClient()
            : this(0, null)
        {
        }

        public RestSharpClient(int connectionTimeout, ILogger logger)
        {
            this.connectionTimeout = connectionTimeout;
            this.logger = logger;
        }

        IHttpResponse ISynchronousHttpClient.Execute(IHttpRequest request)
        {
            throw new NotImplementedException();
        }

        Task<IHttpResponse> IAsynchronousHttpClient.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                this.alreadyDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
