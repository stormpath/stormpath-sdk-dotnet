// <copyright file="DefaultApplication.Saml.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Saml;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Saml;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        async Task<ISamlIdpUrlBuilder> IApplication.NewSamlIdpUrlBuilderAsync(CancellationToken cancellationToken)
        {
            var samlPolicy = await this.AsInterface.GetSamlPolicyAsync(cancellationToken).ConfigureAwait(false);
            var samlServiceProvider = await samlPolicy.GetSamlServiceProviderAsync(cancellationToken).ConfigureAwait(false);
            var ssoInitiationEndpoint = await samlServiceProvider.GetSsoInitiationEndpointAsync(cancellationToken).ConfigureAwait(false);

            return new DefaultSamlIdpUrlBuilder(
                this.GetInternalDataStore(),
                this.AsInterface.Href,
                ssoInitiationEndpoint.Href,
                new DefaultIdSiteJtiProvider(),
                new DefaultClock());
        }

        ISamlIdpUrlBuilder IApplicationSync.NewSamlIdpUrlBuilder()
        {
            var samlPolicy = this.AsInterface.GetSamlPolicy();
            var samlServiceProvider = samlPolicy.GetSamlServiceProvider();
            var ssoInitiationEndpoint = samlServiceProvider.GetSsoInitiationEndpoint();

            return new DefaultSamlIdpUrlBuilder(
                this.GetInternalDataStore(),
                this.AsInterface.Href,
                ssoInitiationEndpoint.Href,
                new DefaultIdSiteJtiProvider(),
                new DefaultClock());
        }

        Task<ISamlPolicy> IApplication.GetSamlPolicyAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<ISamlPolicy>(this.SamlPolicy.Href, cancellationToken);

        ISamlPolicy IApplicationSync.GetSamlPolicy()
            => this.GetInternalSyncDataStore().GetResource<ISamlPolicy>(this.SamlPolicy.Href);

        ISamlAsyncCallbackHandler IApplication.NewSamlAsyncCallbackHandler(IHttpRequest request)
            => new DefaultSamlAsyncCallbackHandler(this.GetInternalDataStore(), request);

        ISamlSyncCallbackHandler IApplicationSync.NewSamlSyncCallbackHandler(IHttpRequest request)
            => new DefaultSamlSyncCallbackHandler(this.GetInternalDataStore(), request);
    }
}
