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

namespace Stormpath.SDK.Saml
{
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

        ISamlIdpUrlBuilder SetPath(string path);

        ISamlIdpUrlBuilder SetOrganizationNameKey(string organizationNameKey);

        ISamlIdpUrlBuilder SetSpToken(string spToken);

        ISamlIdpUrlBuilder AddProperty(string name, object value);

        /// <summary>
        /// Builds the fully-qualified URL representing the redirect to the SAML Identity Provider.
        /// </summary>
        /// <returns>The fully-qualified URL representing the redirect to the SAML Identity Provider.</returns>
        string Build();
    }
}
