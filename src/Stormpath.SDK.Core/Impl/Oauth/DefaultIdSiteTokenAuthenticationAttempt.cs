﻿// <copyright file="DefaultIdSiteTokenAuthenticationAttempt.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    [Obsolete("Remove in 1.0")]
    internal sealed class DefaultIdSiteTokenAuthenticationAttempt : AbstractResource, IIdSiteTokenAuthenticationAttempt
    {
        private static readonly string TokenPropertyName = "token";
        private static readonly string GrantTypePropertyName = "grant_type";

        public DefaultIdSiteTokenAuthenticationAttempt(ResourceData data)
            : base(data)
        {
            this.SetProperty<string>(GrantTypePropertyName, "stormpath_token");
        }

        string IIdSiteTokenAuthenticationAttempt.Token
            => this.GetStringProperty(TokenPropertyName);

        void IIdSiteTokenAuthenticationAttempt.SetToken(string token)
            => this.SetProperty(TokenPropertyName, token);
    }
}
