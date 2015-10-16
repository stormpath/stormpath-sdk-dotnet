// <copyright file="IApplicationSync.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Application
{
    internal interface IApplicationSync : ISaveableSync<IApplication>, IDeletableSync, IAccountCreationActionsSync, IExtendableSync
    {
        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. <see cref="UsernamePasswordRequest"/>).</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="Sync.SyncAuthenticationResultExtensions.GetAccount()"/>.
        /// </returns>
        /// <exception cref="SDK.Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        ///     var loginRequest = new UsernamePasswordRequest("jsmith", "Password123#");
        ///     var result = myApp.AuthenticateAccount(loginRequest);
        /// </example>
        IAuthenticationResult AuthenticateAccount(IAuthenticationRequest request);

        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="Sync.SyncAuthenticationResultExtensions.GetAccount()"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        ///     var result = myApp.AuthenticateAccount("jsmith", "Password123#");
        /// </example>
        IAuthenticationResult AuthenticateAccount(string username, string password);

        /// <summary>
        /// Attempts to synchronously authenticate an account with the specified username and password.
        /// <para>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccount(string, string)"/> instead.</para>
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <returns><c>true</c> if the authentication attempt succeeded; <c>false</c> otherwise.</returns>
        /// <example>
        ///     if (myApp.TryAuthenticateAccount("jsmith", "Password123#"))
        ///     {
        ///         // Login successful
        ///     }
        /// </example>
        bool TryAuthenticateAccount(string username, string password);

        /// <summary>
        /// Synchronously triggers the delivery of a new verification email for the specified account.
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// <para>
        /// The <see cref="IEmailVerificationRequest"/> must contain the username or email identifying the account.
        /// </para>
        /// </summary>
        /// <param name="requestBuilderAction">Sets the options required for the verification email request.</param>
        void SendVerificationEmail(Action<EmailVerificationRequestBuilder> requestBuilderAction);

        /// <summary>
        /// Synchronously triggers the delivery of a new verification email for the specified account.
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// </summary>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        void SendVerificationEmail(string usernameOrEmail);

        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> (either a <see cref="SDK.Group.IGroup"/> or <see cref="SDK.Directory.IDirectory"/>)
        /// used to persist new accounts created by the application.
        /// </summary>
        /// <returns>The default <see cref="IAccountStore"/>,
        /// or <c>null</c> if no default <see cref="IAccountStore"/> has been designated.</returns>
        IAccountStore GetDefaultAccountStore();

        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a Directory.
        /// However, this could be a Group in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <returns>The <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.</returns>
        IAccountStore GetDefaultGroupStore();

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Application resource.
        /// </summary>
        /// <returns>The tenant.</returns>
        ITenant GetTenant();

        /// <summary>
        /// Synchronously creates a new <see cref="IGroup"/> that may be used by this application in the application's <see cref="GetDefaultGroupStoreAsync(CancellationToken)"/>.
        /// <para>This is a convenience method. It merely delegates to the application's designated default group store.</para>
        /// </summary>
        /// <param name="group">The group to create/persist.</param>
        /// <returns>The new <see cref="IGroup"/> that may be used by this application.</returns>
        /// <exception cref="Error.ResourceException">
        /// The application does not have a designated default group store, or the
        /// designated default group store does not allow new groups to be created.
        /// </exception>
        IGroup CreateGroup(IGroup group);

        /// <summary>
        /// Synchronously verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// <para>Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will previously need to execute
        /// <see cref="SendPasswordResetEmail(string)"/> again in order to obtain a new password reset token.</para>
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount"/> if the token is successfully validated.</param>
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        IAccount ResetPassword(string token, string newPassword);

        /// <summary>
        /// Synchronously sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="Sync.SyncPasswordResetTokenExtensions.GetAccount()"/>.</returns>
        /// <exception cref="SDK.Error.ResourceException">There is no account that matches the specified email address.</exception>
        IPasswordResetToken SendPasswordResetEmail(string email);

        /// <summary>
        /// Synchronously verifies a password reset token.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request paramter by your application.</param>
        /// <returns>The <see cref="IAccount"/> matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        IAccount VerifyPasswordResetToken(string token);

        /// <summary>
        /// Synchronously retrieves a Provider-based <see cref="IAccount"/>. The account must exist in one of the Provider-based <see cref="Directory.IDirectory"/>
        /// assigned to the Application as an account store, and the Directory must also be <see cref="Directory.DirectoryStatus.Enabled"/>.
        /// If not in an assigned account store, the retrieval attempt will fail.
        /// </summary>
        /// <param name="request">
        /// The <see cref="IProviderAccountRequest"/> representing the Provider-specific account access data
        /// (e.g. an <c>accessToken</c>) used to verify the identity.
        /// </param>
        /// <returns>The result of the access request.</returns>
        /// <exception cref="Error.ResourceException">The access attempt failed.</exception>
        IProviderAccountResult GetAccount(IProviderAccountRequest request);
    }
}
