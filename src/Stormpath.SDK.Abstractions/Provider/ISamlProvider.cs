// <copyright file="ILinkedInProvider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// SAML <see cref="IProvider">Provider</see> Resource.
    /// </summary>
    public interface ISamlProvider : IProvider
    {
        /// <summary>
        /// Returns the URL at the SAML Identity Provider where end-users should be redirected to login. This is often called
        /// an "SSO URL", "Login URL" or "Sign-in URL" for the Identity Provider (IdP).
        /// </summary>
        /// <value>The SSO login URL.</value>
        string SsoLoginUrl { get; }

        /// <summary>
        /// Returns the URL at the SAML Identity Provider where end-users should be redirected to logout of all applications.
        /// This is often called a "Logout URL", "Global Logout URL" or "Single Logout URL" (SLO).
        /// </summary>
        /// <value>The SSO logout URL.</value>
        string SsoLogoutUrl { get; }

        /// <summary>
        /// Returns the algorithm used by the SAML Identity Provider to sign SAML assertions. If signatures are used, this
        /// value is usually either <c>RSA-SHA1</c> <c>RSA-SHA256</c>.
        /// </summary>
        /// <value>The algorithm used to sign SAML assertions.</value>
        string EncodedX509SigningCertificate { get; }

        /// <summary>
        /// Returns the <a href="https://en.wikipedia.org/wiki/Privacy-enhanced_Electronic_Mail">PEM</a>-formatted X.509
        /// certificate used to validate the SAML Identity Provider's signed SAML assertions.
        /// </summary>
        /// <value>The X.509 certificate used to validate the signed SAML assertions.</value>
        string RequestSignatureAlgorithm { get; }

        // TODO AttributeStatementMappingRules

        // TODO ServiceProviderMetadata
    }
}
