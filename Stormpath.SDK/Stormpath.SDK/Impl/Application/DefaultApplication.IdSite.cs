// <copyright file="DefaultApplication.IdSite.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        SDK.IdSite.IIdSiteUrlBuilder IApplication.NewIdSiteUrlBuilder()
            => new DefaultIdSiteUrlBuilder(this.GetInternalDataStore(), this.AsInterface.Href, new DefaultIdSiteJtiProvider(), new DefaultClock());

        SDK.IdSite.IIdSiteAsyncCallbackHandler IApplication.NewIdSiteAsyncCallbackHandler(SDK.Http.IHttpRequest request)
            => new DefaultIdSiteAsyncCallbackHandler(this.GetInternalDataStore(), request);

        SDK.IdSite.IIdSiteSyncCallbackHandler IApplicationSync.NewIdSiteSyncCallbackHandler(SDK.Http.IHttpRequest request)
            => new DefaultIdSiteSyncCallbackHandler(this.GetInternalDataStore(), request);
    }
}
