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

using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Application
{
    internal interface IApplicationSync : ISaveableSync<IApplication>, IDeletableSync, IAccountCreationSync
    {
        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
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
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
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
        /// Attempts to authenticate an account with the specified username and password.
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
        /// Gets the <see cref="IAccountStore"/> (either a Group or <see cref="SDK.Directory.IDirectory"/>)
        /// used to persist new accounts created by the application.
        /// </summary>
        /// <returns>The default <see cref="IAccountStore"/>,
        /// or <c>null</c> if no default <see cref="IAccountStore"/> has been designated.</returns>
        IAccountStore GetDefaultAccountStore();

        /// <summary>
        /// Verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// <para>Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will previously need to execute
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> again in order to obtain a new password reset token.</para>
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount"/> if the token is successfully validated.</param>
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        IAccount ResetPassword(string token, string newPassword);

        /// <summary>
        /// Sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="Sync.SyncPasswordResetTokenExtensions.GetAccount()"/>.</returns>
        /// <exception cref="SDK.Error.ResourceException">There is no account that matches the specified email address.</exception>
        IPasswordResetToken SendPasswordResetEmail(string email);

        /// <summary>
        /// Verifies a password reset token.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request paramter by your application.</param>
        /// <returns>The <see cref="IAccount"/> matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        IAccount VerifyPasswordResetToken(string token);
    }
}
