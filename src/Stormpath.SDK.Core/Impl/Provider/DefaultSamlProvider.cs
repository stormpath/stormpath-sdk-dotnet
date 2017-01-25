// <copyright file="DefaultLinkedInProvider.cs" company="Stormpath, Inc.">
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
    internal sealed class DefaultSamlProvider : AbstractProvider, ISamlProvider
    {
        private const string SsoLoginUrlPropertyName = "ssoLoginUrl";
        private const string SsoLogoutUrlPropertyName = "ssoLogoutUrl";
        private const string SigningCertificatePropertyName = "encodedX509SigningCert";
        private const string RequestSignatureAlgorithmPropertyName = "requestSignatureAlgorithm";

        public DefaultSamlProvider(ResourceData data)
            : base(data)
        {
        }

        string ISamlProvider.SsoLoginUrl => GetStringProperty(SsoLoginUrlPropertyName);

        string ISamlProvider.SsoLogoutUrl => GetStringProperty(SsoLogoutUrlPropertyName);

        string ISamlProvider.EncodedX509SigningCertificate => GetStringProperty(SigningCertificatePropertyName);

        string ISamlProvider.RequestSignatureAlgorithm => GetStringProperty(RequestSignatureAlgorithmPropertyName);
    }
}
