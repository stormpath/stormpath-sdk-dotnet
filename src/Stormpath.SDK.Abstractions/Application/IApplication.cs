// <copyright file="IApplication.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Api;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Saml;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Application
{
    /// <summary>
    /// Represents a Stormpath registered application.
    /// </summary>
    public interface IApplication :
        IResource,
        IHasTenant,
        ISaveableWithOptions<IApplication>,
        IDeletable,
        IAuditable,
        IExtendable,
        IAccountStoreContainer<IApplicationAccountStoreMapping>,
        IAccountCreationActions,
        IGroupCreationActions
    {
        /// <summary>
        /// Gets the Application's name.
        /// </summary>
        /// <value>The application's name. An application's name must be unique across all other applications within a Stormpath <see cref="ITenant">Tenant</see>.</value>
        string Name { get; }

        /// <summary>
        /// Gets the Application description.
        /// </summary>
        /// <value>The application's description text.</value>
        string Description { get; }

        /// <summary>
        /// Gets the authorized callback URIs for this <see cref="IApplication">Application</see>.
        /// </summary>
        /// <value>The authorized callback URIs for this <see cref="IApplication">Application</see>.</value>
        IReadOnlyList<string> AuthorizedCallbackUris { get; }

        /// <summary>
        /// Gets the Application's status.
        /// </summary>
        /// <value>
        /// The Application's status.
        /// Application users may login to an <see cref="ApplicationStatus.Enabled">Enabled</see> application.
        /// They may not login to a <see cref="ApplicationStatus.Disabled">Disabled</see> application.
        /// </value>
        ApplicationStatus Status { get; }

        /// <summary>
        /// Sets the Application description.
        /// </summary>
        /// <param name="description">The Application's description text.</param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetDescription(string description);

        /// <summary>
        /// Sets the Application's name.
        /// </summary>
        /// <param name="name">The Application's name. Application names must be unique within a Stormpath <see cref="ITenant">Tenant</see>.</param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetName(string name);

        /// <summary>
        /// Sets the Application's status.
        /// </summary>
        /// <param name="status">The Application's status.
        /// Application users may login to an <see cref="ApplicationStatus.Enabled">Enabled</see> application.
        /// They may not login to a <see cref="ApplicationStatus.Disabled">Disabled</see> application.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetStatus(ApplicationStatus status);

        /// <summary>
        /// Creates a new <see cref="IIdSiteUrlBuilder"/> that allows you to build a URL you can use to redirect your
        /// application users to a hosted login/registration/forgot-password site - what Stormpath calls an 'Identity Site'
        /// (or 'ID Site' for short) - for performing common user identity functionality.
        /// When the user is done (logging in, registering, etc), they will be redirected back to a <c>callbackUri</c> of your choice.
        /// </summary>
        /// <returns>A new <see cref="IIdSiteUrlBuilder"/> that allows you to build a URL you can use to redirect your application users to a hosted login/registration/forgot-password 'ID Site'.</returns>
        IIdSiteUrlBuilder NewIdSiteUrlBuilder();

        /// <summary>
        /// Creates a new <see cref="IIdSiteAsyncCallbackHandler"/> used to handle HTTP replies from your ID Site to your application's <c>callbackUri</c>,
        /// as described in the <see cref="NewIdSiteUrlBuilder"/> method.
        /// </summary>
        /// <param name="request">
        /// An instance of <see cref="IHttpRequest"/>.
        /// </param>
        /// <returns>An <see cref="IIdSiteAsyncCallbackHandler"/> that allows you customize how the <paramref name="request"/> will be handled.</returns>
        IIdSiteAsyncCallbackHandler NewIdSiteAsyncCallbackHandler(IHttpRequest request);

        /// <summary>
        /// Creates a new <see cref="IIdSiteTokenAuthenticator">ID Site Token Authenticator</see> that can
        /// exchange an ID Site result for an OAuth 2.0 access token.
        /// </summary>
        /// <returns>A new <see cref="IIdSiteTokenAuthenticator"/> instance.</returns>
        IIdSiteTokenAuthenticator NewIdSiteTokenAuthenticator();

        /// <summary>
        /// Creates a new <see cref="IPasswordGrantAuthenticator">Password Grant Authenticator</see> that allows you to
        /// authenticate an account and exchange its credentials for a valid OAuth 2.0 token.
        /// </summary>
        /// <returns>A new <see cref="IPasswordGrantAuthenticator"/></returns> instance.
        IPasswordGrantAuthenticator NewPasswordGrantAuthenticator();

        /// <summary>
        /// Creates a new <see cref="IRefreshGrantAuthenticator">Refresh Grant Authenticator</see> that allows you to
        /// refresh an OAuth 2.0 token created in Stormpath.
        /// </summary>
        /// <returns>A new <see cref="IRefreshGrantAuthenticator"/></returns> instance.
        IRefreshGrantAuthenticator NewRefreshGrantAuthenticator();

        /// <summary>
        /// Creates a new <see cref="IJwtAuthenticator">JWT Authenticator</see> that allows you to validate
        /// a JSON Web Token locally or against Stormpath.
        /// </summary>
        /// <returns>A new <see cref="IJwtAuthenticator"/></returns> instance.
        IJwtAuthenticator NewJwtAuthenticator();

        /// <summary>
        /// Creates a new <see cref="ISamlIdpUrlBuilder"/> that allows you to build a URL you can use to redirect
        /// your application users to an external SAML Identity Provider.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new <see cref="ISamlIdpUrlBuilder"/> that can build a URL to redirect your users to a SAML Identity Provider.</returns>
        Task<ISamlIdpUrlBuilder> NewSamlIdpUrlBuilderAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="ISamlAsyncCallbackHandler"/> used to handle HTTP replies from an external SAML Identity Provider to your
        /// application's <c>callbackUri</c>.
        /// </summary>
        /// <param name="request">
        /// An instance of <see cref="IHttpRequest"/>.
        /// </param>
        /// <returns>A new <see cref="ISamlAsyncCallbackHandler"/> that allows your to customize how the <paramref name="request"/> will be handled.</returns>
        ISamlAsyncCallbackHandler NewSamlAsyncCallbackHandler(IHttpRequest request);

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// <code>
        /// var loginRequest = new UsernamePasswordRequestBuilder();
        /// loginRequest.SetUsernameOrEmail("jsmith");
        /// loginRequest.SetPassword("Password123#");
        /// var result = await myApp.AuthenticateAccountAsync(loginRequest.Build());
        /// </code>
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(IAuthenticationRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> instance.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To request and cache the account details:
        /// <code>
        /// var loginRequest = new UsernamePasswordRequestBuilder();
        /// loginRequest.SetUsernameOrEmail("jsmith");
        /// loginRequest.SetPassword("Password123#");
        /// var result = await myApp.AuthenticateAccountAsync(loginRequest.Build(), response => response.Expand(x => x.GetAccountAsync));
        /// </code>
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// <code>
        /// var result = await myApp.AuthenticateAccountAsync("jsmith", "Password123#");
        /// </code>
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Attempts to authenticate an account with the specified username and password.
        /// </summary>
        /// <remarks>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccountAsync(string, string, CancellationToken)"/> instead.</remarks>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the authentication attempt succeeded; <see langword="false"/> otherwise.</returns>
        /// <example>
        /// if (await myApp.TryAuthenticateAccountAsync("jsmith", "Password123#"))
        /// {
        ///     // Login successful
        /// }
        /// </example>
        Task<bool> TryAuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers the delivery of a new verification email for the specified account.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// </remarks>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendVerificationEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// </summary>
        /// <remarks>
        /// Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will need to execute
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> again in order to obtain a new password reset token.
        /// </remarks>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount">Account</see> if the token is successfully validated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="Error.ResourceException">The token is not valid.</exception>
        Task<IAccount> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">There is no account that matches the specified email address.</exception>
        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email to an account in the specified <see cref="IAccountStore">Account Store</see> matching
        /// the specified <paramref name="email"/>. If the email does not match an account in the specified
        /// <see cref="IAccountStore">Account Store</see>, a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified <see cref="IAccountStore">Account Store</see> is not mapped to this application, or there is no account that matches the specified email address in the specified <paramref name="accountStore"/>.
        /// </exception>
        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email to an account matching the specified <paramref name="email"/>
        /// in the AccountStore or Organization matching the specified <paramref name="hrefOrNameKey"/>.
        /// If the email does not match an account in the specified AccountStore or Organization,
        /// a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified AccountStore or Organization is not mapped to this application, or there is no account that matches the specified email address in the AccountStore or Organization.
        /// </exception>
        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, string hrefOrNameKey, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies a password reset token.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccount">Account</see> matching the specified token.</returns>
        /// <exception cref="Error.ResourceException">The token is not valid.</exception>
        Task<IAccount> VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves a Provider-based <see cref="IAccount">Account</see>. The account must exist in one of the Provider-based <see cref="Directory.IDirectory">Directory</see>
        /// assigned to the Application as an account store, and the Directory must also be <see cref="Directory.DirectoryStatus.Enabled">Enabled</see>.
        /// If not in an assigned account store, the retrieval attempt will fail.
        /// </summary>
        /// <param name="request">
        /// The <see cref="IProviderAccountRequest"/> representing the Provider-specific account access data
        /// (e.g. an <c>accessToken</c>) used to verify the identity.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the access request.</returns>
        /// <exception cref="Error.ResourceException">The access attempt failed.</exception>
        Task<IProviderAccountResult> GetAccountAsync(IProviderAccountRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IOauthPolicy">OauthPolicy</see> associated with this <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IOauthPolicy">OauthPolicy</see> associated with this <see cref="IApplication">Application</see>.</returns>
        Task<IOauthPolicy> GetOauthPolicyAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="ISamlPolicy">SAML Policy</see> associated with this <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="ISamlPolicy">SAML Policy</see> associated with this <see cref="IApplication">Application</see>.</returns>
        Task<ISamlPolicy> GetSamlPolicyAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an <see cref="IApiKey">API Key</see>, by its ID, that belongs to an <see cref="IAccount">Account</see>
        /// that has access to this application by a mapped account store.
        /// </summary>
        /// <param name="apiKeyId">The API Key ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKeyId"/> is null or empty.</exception>
        /// <returns>The API Key, or <see langword="null"/> if no API Key could be found.</returns>
        Task<IApiKey> GetApiKeyAsync(string apiKeyId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an <see cref="IApiKey">API Key</see>, by its ID, that belongs to an <see cref="IAccount">Account</see>
        /// that has access to this application by a mapped account store.
        /// </summary>
        /// <param name="apiKeyId">The API Key ID.</param>
        /// <param name="retrievalOptionsAction">The options to apply to the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKeyId"/> is null or empty.</exception>
        /// <returns>The API Key, or <see langword="null"/> if no API Key could be found.</returns>
        Task<IApiKey> GetApiKeyAsync(string apiKeyId, Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of all Accounts in this Application.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to asynchronously list or search <see cref="Account.IAccount">Accounts</see>.</returns>
        /// <example>
        /// var allAccounts = await myApp.GetAccounts().ToListAsync();
        /// </example>
        IAsyncQueryable<IAccount> GetAccounts();

        /// <summary>
        /// Gets a queryable list of all Groups accessible to this application.
        /// It will not only return any group associated directly as an <see cref="IAccountStore">Account Store</see>
        /// but also every group that exists inside every directory associated as an Account Store.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search <see cref="IGroup">Groups</see>.</returns>
        /// <example>
        /// var allGroups = await myApp.GetGroups().ToListAsync();
        /// </example>
        IAsyncQueryable<IGroup> GetGroups();
    }
}
