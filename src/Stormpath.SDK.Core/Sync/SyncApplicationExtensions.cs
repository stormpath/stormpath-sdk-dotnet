// <copyright file="SyncApplicationExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IApplication">Application</see>.
    /// </summary>
    public static class SyncApplicationExtensions
    {
        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="SDK.Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// <code>
        /// var loginRequest = new UsernamePasswordRequestBuilder();
        /// loginRequest.SetUsernameOrEmail("jsmith");
        /// loginRequest.SetPassword("Password123#");
        /// var result = myApp.AuthenticateAccount(loginRequest.Build());
        /// </code>
        /// </example>
        public static IAuthenticationResult AuthenticateAccount(this IApplication application, IAuthenticationRequest request)
            => (application as IApplicationSync).AuthenticateAccount(request);

        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>
        /// A public static  whose result is the result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="SDK.Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To request and cache the account details:
        /// <code>
        /// var loginRequest = new UsernamePasswordRequestBuilder();
        /// loginRequest.SetUsernameOrEmail("jsmith");
        /// loginRequest.SetPassword("Password123#");
        /// var result = myApp.AuthenticateAccount(loginRequest.Build(), response => response.Expand(x => x.GetAccount()));
        /// </code>
        /// </example>
        public static IAuthenticationResult AuthenticateAccount(this IApplication application, IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions)
            => (application as IApplicationSync).AuthenticateAccount(request, responseOptions);

        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="requestBuilder">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To attempt to authenticate against a specific account store:
        /// <code>
        /// var result = myApp.AuthenticateAccount(request =>
        /// {
        ///     request.SetUsernameOrEmail("jsmith");
        ///     request.SetPassword("Password123#");
        ///     request.SetAccountStore(myAccountStore);
        /// });
        /// </code>
        /// </example>
        public static IAuthenticationResult AuthenticateAccount(this IApplication application, Action<UsernamePasswordRequestBuilder> requestBuilder)
            => (application as IApplicationSync).AuthenticateAccount(requestBuilder);

        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="requestBuilder">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To attempt to authenticate against a specific account store, and cache the returned account details:
        /// <code>
        /// var result = await myApp.AuthenticateAccount(
        ///     request =>
        ///     {
        ///         request.SetUsernameOrEmail("jsmith");
        ///         request.SetPassword("Password123#");
        ///         request.SetAccountStore(myAccountStore);
        ///     },
        ///     response => response.Expand(x => x.GetAccountAsync));
        /// </code>
        /// </example>
        public static IAuthenticationResult AuthenticateAccount(this IApplication application, Action<UsernamePasswordRequestBuilder> requestBuilder, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions)
            => (application as IApplicationSync).AuthenticateAccount(requestBuilder, responseOptions);

        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// <code>
        /// var result = myApp.AuthenticateAccount("jsmith", "Password123#");
        /// </code>
        /// </example>
        public static IAuthenticationResult AuthenticateAccount(this IApplication application, string username, string password)
            => (application as IApplicationSync).AuthenticateAccount(username, password);

        /// <summary>
        /// Attempts to synchronously authenticate an account with the specified username and password.
        /// </summary>
        /// <remarks>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccount(IApplication, string, string)"/> instead.</remarks>
        /// <param name="application">The application.</param>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <returns><see langword="true"/> if the authentication attempt succeeded; <see langword="false"/> otherwise.</returns>
        /// <example>
        /// if (myApp.TryAuthenticateAccount("jsmith", "Password123#"))
        /// {
        ///     // Login successful
        /// }
        /// </example>
        public static bool TryAuthenticateAccount(this IApplication application, string username, string password)
            => (application as IApplicationSync).TryAuthenticateAccount(username, password);

        /// <summary>
        /// Synchronously executes an OAuth request against the <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">The request.</param>
        /// <returns>An <see cref="IOauthGrantAuthenticationResult">OAuth 2.0 response</see>.</returns>
        public static IOauthGrantAuthenticationResult ExecuteOauthRequest(this IApplication application, AbstractOauthGrantRequest request)
            => (application as IApplicationSync).ExecuteOauthRequest(request);

        /// <summary>
        /// Synchronously triggers the delivery of a new verification email for the specified account.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// <para>
        /// The <see cref="IEmailVerificationRequest"/> must contain the username or email identifying the account.
        /// </para>
        /// </remarks>
        /// <param name="application">The application</param>
        /// <param name="requestBuilderAction">Sets the options required for the verification email request.</param>
        public static void SendVerificationEmail(this IApplication application, Action<EmailVerificationRequestBuilder> requestBuilderAction)
            => (application as IApplicationSync).SendVerificationEmail(requestBuilderAction);

        /// <summary>
        /// Synchronously triggers the delivery of a new verification email for the specified account.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// </remarks>
        /// <param name="application">The application</param>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        public static void SendVerificationEmail(this IApplication application, string usernameOrEmail)
            => (application as IApplicationSync).SendVerificationEmail(usernameOrEmail);

        /// <summary>
        /// Creates a new <see cref="IdSite.IIdSiteSyncCallbackHandler"/> used to synchronously handle HTTP replies from your ID Site to your application's <c>callbackUri</c>,
        /// as described in the <see cref="IApplication.NewIdSiteUrlBuilder"/> method.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">
        /// An instance of <see cref="Http.IHttpRequest"/>.
        /// See the <see cref="HttpRequests"/> helper class to help build this from an existing request.
        /// </param>
        /// <returns>An <see cref="IdSite.IIdSiteAsyncCallbackHandler"/> that allows you customize how the <paramref name="request"/> will be handled.</returns>
        public static IdSite.IIdSiteSyncCallbackHandler NewIdSiteSyncCallbackHandler(this IApplication application, IHttpRequest request)
            => (application as IApplicationSync).NewIdSiteSyncCallbackHandler(request);

        /// <summary>
        /// Synchronously verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// </summary>
        /// <remarks>Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will need to execute
        /// <see cref="SendPasswordResetEmail(IApplication, string)"/> again in order to obtain a new password reset token.</remarks>
        /// <param name="application">The application.</param>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount">Account</see> if the token is successfully validated.</param>
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        public static IAccount ResetPassword(this IApplication application, string token, string newPassword)
            => (application as IApplicationSync).ResetPassword(token, newPassword);

        /// <summary>
        /// Synchronously sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount(IPasswordResetToken)"/>.</returns>
        /// <exception cref="SDK.Error.ResourceException">There is no account that matches the specified email address.</exception>
        public static IPasswordResetToken SendPasswordResetEmail(this IApplication application, string email)
            => (application as IApplicationSync).SendPasswordResetEmail(email);

        /// <summary>
        /// Synchronously sends a password reset email to an account in the specified <see cref="IAccountStore">Account Store</see> matching
        /// the specified <paramref name="email"/>. If the email does not match an account in the specified
        /// <see cref="IAccountStore">Account Store</see>, a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmail(IApplication, string)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <returns>A public static  whose result is the created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount(IPasswordResetToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified <see cref="IAccountStore">Account Store</see> is not mapped to this application, or there is no account that matches the specified email address in the specified <paramref name="accountStore"/>.
        /// </exception>
        public static IPasswordResetToken SendPasswordResetEmail(this IApplication application, string email, IAccountStore accountStore)
            => (application as IApplicationSync).SendPasswordResetEmail(email, accountStore);

        /// <summary>
        /// Synchronously sends a password reset email to an account matching the specified <paramref name="email"/>
        /// in the AccountStore or Organization matching the specified <paramref name="hrefOrNameKey"/>.
        /// If the email does not match an account in the specified AccountStore or Organization,
        /// a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmail(IApplication, string)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <returns>A public static  whose result is the created <see cref="IPasswordResetToken">Password Reset Token</see>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount(IPasswordResetToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified AccountStore or Organization is not mapped to this application, or there is no account that matches the specified email address in the AccountStore or Organization.
        /// </exception>
        public static IPasswordResetToken SendPasswordResetEmail(this IApplication application, string email, string hrefOrNameKey)
            => (application as IApplicationSync).SendPasswordResetEmail(email, hrefOrNameKey);

        /// <summary>
        /// Synchronously verifies a password reset token.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <returns>The <see cref="IAccount">Account</see> matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        public static IAccount VerifyPasswordResetToken(this IApplication application, string token)
            => (application as IApplicationSync).VerifyPasswordResetToken(token);

        /// <summary>
        /// Synchronously retrieves a Provider-based <see cref="IAccount">Account</see>. The account must exist in one of the Provider-based <see cref="Directory.IDirectory">Directory</see>
        /// assigned to the Application as an account store, and the Directory must also be <see cref="Directory.DirectoryStatus.Enabled">Enabled</see>.
        /// If not in an assigned account store, the retrieval attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">
        /// The <see cref="IProviderAccountRequest"/> representing the Provider-specific account access data
        /// (e.g. an <c>accessToken</c>) used to verify the identity.
        /// </param>
        /// <returns>The result of the access request.</returns>
        /// <exception cref="Error.ResourceException">The access attempt failed.</exception>
        public static IProviderAccountResult GetAccount(this IApplication application, IProviderAccountRequest request)
            => (application as IApplicationSync).GetAccount(request);

        /// <summary>
        /// Synchronously retrieves the <see cref="IOauthPolicy">OauthPolicy</see> associated with this <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The <see cref="IOauthPolicy">OauthPolicy</see> associated with this <see cref="IApplication">Application</see>.</returns>
        public static IOauthPolicy GetOauthPolicy(this IApplication application)
            => (application as IApplicationSync).GetOauthPolicy();

        /// <summary>
        /// Synchronously retrieves the <see cref="ISamlPolicy">SAML Policy</see> associated with this <see cref="IApplication">Application</see>.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The <see cref="ISamlPolicy">SAML Policy</see> associated with this <see cref="IApplication">Application</see>.</returns>
        public static ISamlPolicy GetSamlPolicy(this IApplication application)
            => (application as IApplicationSync).GetSamlPolicy();

        /// <summary>
        /// Synchronously creates a new <see cref="ISamlIdpUrlBuilder"/> that allows you to build a URL you can use to redirect
        /// your application users to an external SAML Identity Provider.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>A new <see cref="ISamlIdpUrlBuilder"/> that can build a URL to redirect your users to a SAML Identity Provider.</returns>
        public static ISamlIdpUrlBuilder NewSamlIdpUrlBuilder(this IApplication application)
            => (application as IApplicationSync).NewSamlIdpUrlBuilder();

        /// <summary>
        /// Creates a new <see cref="ISamlSyncCallbackHandler"/> used to handle HTTP replies from an external SAML Identity Provider to your
        /// application's <c>callbackUri</c>.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">
        /// An instance of <see cref="IHttpRequest"/>.
        /// See the <see cref="HttpRequests"/> helper class to help build this from an existing request.
        /// </param>
        /// <returns>A new <see cref="ISamlSyncCallbackHandler"/> that allows your to customize how the <paramref name="request"/> will be handled.</returns>
        public static ISamlSyncCallbackHandler NewSamlSyncCallbackHandler(this IApplication application, IHttpRequest request)
            => (application as IApplicationSync).NewSamlSyncCallbackHandler(request);

        /// <summary>
        /// Synchronously gets an <see cref="IApiKey">API Key</see>, by its ID, that belongs to an <see cref="IAccount">Account</see>
        /// that has access to this application by a mapped account store.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="apiKeyId">The API Key ID.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKeyId"/> is null or empty.</exception>
        /// <returns>The API Key, or <see langword="null"/> if no API Key could be found.</returns>
        [Obsolete("Use GetApiKeys() collection")]
        public static IApiKey GetApiKey(this IApplication application, string apiKeyId)
            => (application as IApplicationSync).GetApiKey(apiKeyId);

        /// <summary>
        /// Synchronously gets an <see cref="IApiKey">API Key</see>, by its ID, that belongs to an <see cref="IAccount">Account</see>
        /// that has access to this application by a mapped account store.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="apiKeyId">The API Key ID.</param>
        /// <param name="retrievalOptionsAction">The options to apply to the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiKeyId"/> is null or empty.</exception>
        /// <returns>The API Key, or <see langword="null"/> if no API Key could be found.</returns>
        [Obsolete("Use GetApiKeys() collection")]
        public static IApiKey GetApiKey(this IApplication application, string apiKeyId, Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction)
            => (application as IApplicationSync).GetApiKey(apiKeyId, retrievalOptionsAction);
    }
}
