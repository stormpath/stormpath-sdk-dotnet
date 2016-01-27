// <copyright file="ISamlIdpUrlBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Saml
{
    /// <summary>
    /// Used to construct a URL you can use to redirect application users to a SAML authentication site (Identity Provider or IdP) for performing
    /// common user identity functionality. When users are done, they will be redirected back to a <c>callbackUri</c> of your choice.
    /// </summary>
    public interface ISamlIdpUrlBuilder
    {
        /// <summary>
        /// Sets the location where the user will be sent when returning from the SAML Identity Provider. This property is mandatory and must be set.
        /// </summary>
        /// <remarks>
        /// The <paramref name="callbackUri"/> must be one of <see cref="Application.IApplication.AuthorizedCallbackUris"/>.
        /// Use <see cref="Application.IApplication.NewSamlAsyncCallbackHandler(Http.IHttpRequest)"/> to process requests to your <paramref name="callbackUri"/>.
        /// </remarks>
        /// <param name="callbackUri">The final destination the browser will be redirected to.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetCallbackUri(string callbackUri);

        /// <summary>
        /// Sets the location where the user will be sent when returning from the SAML Identity Provider. This property is mandatory and must be set.
        /// </summary>
        /// <remarks>
        /// The <paramref name="callbackUri"/> must be one of <see cref="Application.IApplication.AuthorizedCallbackUris"/>.
        /// Use <see cref="Application.IApplication.NewSamlAsyncCallbackHandler(Http.IHttpRequest)"/> to process requests to your <paramref name="callbackUri"/>.
        /// </remarks>
        /// <param name="callbackUri">The final destination the browser will be redirected to.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetCallbackUri(Uri callbackUri);

        /// <summary>
        /// Sets application-specific state data that should be retained and made available to your <c>callbackUri</c>
        /// when the user returns from the SAML Identity Provider.
        /// </summary>
        /// <param name="state">Application-specific state data that should be retained and made available to your <c>callbackUri</c> when the user returns from the SAML Identity Provider.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetState(string state);

        /// <summary>
        /// Sets the initial path in the SAML Identity Provider where the user should be sent. If unspecified, this defaults to <c>/</c>,
        /// implying that the SAML Identity Provider's landing/home page is the desired location.
        /// </summary>
        /// <param name="path">The initial path in the ID Site where the user should be sent.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetPath(string path);

        /// <summary>
        /// Attempts to use first directory mapped to the specified <see cref="Organization.IOrganization">Organization</see>
        /// as a SAML Account Store.
        /// </summary>
        /// <param name="organizationNameKey">The unique identifier of the <see cref="Organization.IOrganization">Organization</see> to use as a SAML Account Store.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetOrganizationNameKey(string organizationNameKey);

        /// <summary>
        /// Attempts to use the specified <see cref="IAccountStore">Account Store</see> as a SAML Account Store.
        /// </summary>
        /// <param name="accountStore">The account store.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Attempts to use the specified Account Store <paramref name="href"/> as a SAML Account Store.
        /// </summary>
        /// <param name="href">The account store <c>href</c>.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetAccountStore(string href);

        /// <summary>
        /// Sets the <c>sp_token</c> property used by the SAML Identity Provider to complete an account password reset workflow.
        /// </summary>
        /// <remarks>
        /// The <paramref name="spToken"/> must correspond to an actual password reset token issued to one of the accounts of this
        /// <see cref="Application.IApplication">Application</see>. If <paramref name="spToken"/> property is present and valid, the user will be redirected to
        /// the SAML Identity Provider URL (included the configured path, see <see cref="SetPath(string)"/>), with the <paramref name="spToken"/> embedded in
        /// the signed JWT.
        /// </remarks>
        /// <param name="spToken">A unique token used to reset a password.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder SetSpToken(string spToken);

        /// <summary>
        /// Sets any key value in the SAML request payload.
        /// </summary>
        /// <remarks>
        /// If possible, prefer using the available <c>Set</c> methods, since these are type-safe.
        /// </remarks>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>This instance for method chaining.</returns>
        ISamlIdpUrlBuilder AddProperty(string name, object value);

        /// <summary>
        /// Builds the fully-qualified URL representing the redirect to the SAML Identity Provider.
        /// </summary>
        /// <returns>The fully-qualified URL representing the redirect to the SAML Identity Provider.</returns>
        string Build();
    }
}
