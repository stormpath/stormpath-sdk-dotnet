// <copyright file="DefaultFactorChallengeAuthenticator.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultFactorChallengeAuthenticator :
        AbstractGrantAuthenticator<IFactorChallengeAuthenticationRequest>,
        IFactorChallengeAuthenticator,
        IFactorChallengeAuthenticatorSync
    {
        public DefaultFactorChallengeAuthenticator(IApplication application, IInternalDataStore internalDataStore)
            : base(application, internalDataStore)
        {
        }

        public async Task<IOauthGrantAuthenticationResult> AuthenticateAsync(IFactorChallengeAuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken)
        {
            ThrowIfInvalid(authenticationRequest);

            var factorCodeExchangeData = CreateData(authenticationRequest);
            var headers = GetHeaderWithMediaType();

            return await InternalAsyncDataStore.CreateAsync<FactorChallengeAuthenticationData, IGrantAuthenticationToken>(
                $"{application.Href}{OauthTokenPath}",
                factorCodeExchangeData,
                headers,
                cancellationToken).ConfigureAwait(false);
        }

        public IOauthGrantAuthenticationResult Authenticate(IFactorChallengeAuthenticationRequest authenticationRequest)
        {
            ThrowIfInvalid(authenticationRequest);

            var factorCodeExchangeData = CreateData(authenticationRequest);
            var headers = GetHeaderWithMediaType();

            return InternalSyncDataStore.Create<FactorChallengeAuthenticationData, IGrantAuthenticationToken>(
                $"{application.Href}{OauthTokenPath}",
                factorCodeExchangeData,
                headers);
        }

        private static FactorChallengeAuthenticationData CreateData(IFactorChallengeAuthenticationRequest request)
            => new FactorChallengeAuthenticationData
            {
                Challenge = request.ChallengeHref,
                Code = request.Code,
            };


    }
}
