// <copyright file="NetHttpRequestExecutor.cs" company="Stormpath, Inc.">
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;

namespace Stormpath.SDK.Impl.Http
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:DoNotUseRegions", Justification = "Reviewed.")]
    internal sealed class NetHttpRequestExecutor : IRequestExecutor
    {
        private readonly IClientApiKey apiKey;
        private readonly AuthenticationScheme authScheme;
        private readonly int connectionTimeout;
        private readonly HttpClient client;

        private bool disposed = false; // To detect redundant calls

        public NetHttpRequestExecutor(IClientApiKey apiKey, AuthenticationScheme authenticationScheme, int connectionTimeout)
        {
            this.apiKey = apiKey;
            this.authScheme = authenticationScheme;
            this.connectionTimeout = connectionTimeout;

            client = new HttpClient();
        }

        AuthenticationScheme IRequestExecutor.AuthenticationScheme => authScheme;

        int IRequestExecutor.ConnectionTimeout => connectionTimeout;

        Task<string> IRequestExecutor.GetAsync(string href, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        string IRequestExecutor.GetSync(string href)
        {
            throw new NotImplementedException();
        }

        #region IDisposable implementation

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
