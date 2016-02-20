// <copyright file="DefaultApplication.Oauth.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IIdSiteTokenAuthenticator IApplication.NewIdSiteTokenAuthenticator()
            => new DefaultIdSiteTokenAuthenticator(this, this.GetInternalDataStore());

        IPasswordGrantAuthenticator IApplication.NewPasswordGrantAuthenticator()
            => new DefaultPasswordGrantAuthenticator(this, this.GetInternalDataStore());

        IRefreshGrantAuthenticator IApplication.NewRefreshGrantAuthenticator()
            => new DefaultRefreshGrantAuthenticator(this, this.GetInternalDataStore());

        IJwtAuthenticator IApplication.NewJwtAuthenticator()
            => new DefaultJwtAuthenticator(this, this.GetInternalDataStore());
    }
}
