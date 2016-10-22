// <copyright file="IPasswordGrantRequest.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Represents a request for Stormpath to authenticate an <see cref="Account.IAccount">Account</see> and exchange its
    /// credentials for a valid OAuth 2.0 access token.
    /// </summary>
    [Obsolete("Use new PasswordGrantRequest()")]
    public interface IPasswordGrantRequest : IOauthGrantRequest
    {
        /// <summary>
        /// Gets the username used for the request.
        /// </summary>
        /// <value>The username used for the request.</value>
        string Login { get; }

        /// <summary>
        /// Gets the plaintext password used for the request.
        /// </summary>
        /// <value>The plaintext password used for the request.</value>
        string Password { get; }

        /// <summary>
        /// Gets the <see cref="AccountStore.IAccountStore">Account Store</see> <c>href</c> used for this request, if any.
        /// </summary>
        /// <value>
        /// The <see cref="AccountStore.IAccountStore">Account Store</see> <c>href</c> used for this request,
        /// or <see langword="null"/> if no Account Store is set.
        /// </value>
        string AccountStoreHref { get; }

        /// <summary>
        /// Gets the <see cref="Organization.IOrganization">Organization</see> <c>nameKey</c> used for this request, if any.
        /// </summary>
        /// <value>
        /// The <see cref="Organization.IOrganization">Organization</see> <c>nameKey</c> used for this request,
        /// or <see langword="null"/> if no name key is set.
        /// </value>
        string OrganizationNameKey { get; }
    }
}
