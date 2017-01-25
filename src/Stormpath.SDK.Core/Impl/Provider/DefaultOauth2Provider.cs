// <copyright file="DefaultOauth2Provider.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultOauth2Provider : AbstractProvider, IOauth2Provider
    {
        private const string AccessTokenTypePropertyName = "accessTokenType";
        private const string AuthorizationEndpointPropertyName = "authorizationEndpoint";
        private const string IdFieldPropertyName = "idField";
        private const string ResourceEndpointPropertyName = "resourceEndpoint";
        private const string TokenEndpointPropertyName = "tokenEndpoint";

        public DefaultOauth2Provider(ResourceData data)
            : base(data)
        {
        }

        public string AccessTokenType => GetStringProperty(AccessTokenTypePropertyName);

        public string AuthorizationEndpoint => GetStringProperty(AuthorizationEndpointPropertyName);

        public string IdField => GetStringProperty(IdFieldPropertyName);

        public string ResourceEndpoint => GetStringProperty(ResourceEndpointPropertyName);

        public string TokenEndpoint => GetStringProperty(TokenEndpointPropertyName);
    }
}
