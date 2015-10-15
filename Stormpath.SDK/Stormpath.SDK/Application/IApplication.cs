// <copyright file="IApplication.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Application
{
    /// <summary>
    /// Represents a Stormpath registered application.
    /// </summary>
    public interface IApplication : IResource, ISaveable<IApplication>, IDeletable, IAuditable, IExtendable, IAccountCreationActions
    {
        /// <summary>
        /// Gets the application's name.
        /// </summary>
        /// <value>This application's name. An application's name must be unique across all other applications in the owning <see cref="Tenant.ITenant"/>.</value>
        string Name { get; }

        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>This application's description text.</value>
        string Description { get; }

        /// <summary>
        /// Gets the application's status.
        /// </summary>
        /// <value>
        /// This application's status.
        /// Application users may login to an <see cref="ApplicationStatus.Enabled"/> application.
        /// They may not login to a <see cref="ApplicationStatus.Disabled"/> application.
        /// </value>
        ApplicationStatus Status { get; }

        /// <summary>
        /// Sets the application description.
        /// </summary>
        /// <param name="description">The application's description text.</param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetDescription(string description);

        /// <summary>
        /// Sets the application's name.
        /// </summary>
        /// <param name="name">The application's name. Application names must be unique within a <see cref="Tenant.ITenant"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetName(string name);

        /// <summary>
        /// Sets the application's status.
        /// </summary>
        /// <param name="status">The application's status.
        /// Application users may login to an <see cref="ApplicationStatus.Enabled"/> application.
        /// They may not login to a <see cref="ApplicationStatus.Disabled"/> application.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetStatus(ApplicationStatus status);

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. <see cref="UsernamePasswordRequest"/>).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task whose result is the result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        ///     var loginRequest = new UsernamePasswordRequest("jsmith", "Password123#");
        ///     var result = await myApp.AuthenticateAccountAsync(loginRequest);
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(IAuthenticationRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task whose result is the result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        ///     var result = await myApp.AuthenticateAccountAsync("jsmith", "Password123#");
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Attempts to authenticate an account with the specified username and password.
        /// <para>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccountAsync(string, string, CancellationToken)"/> instead.</para>
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is <c>true</c> if the authentication attempt succeeded; <c>false</c> otherwise.</returns>
        /// <example>
        ///     if (await myApp.TryAuthenticateAccountAsync("jsmith", "Password123#"))
        ///     {
        ///         // Login successful
        ///     }
        /// </example>
        Task<bool> TryAuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers the delivery of a new verification email for the specified account.
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task that determines when the operation is complete.</returns>
        Task SendVerificationEmailAsync(Action<EmailVerificationRequestBuilder> requestBuilderAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Triggers the delivery of a new verification email for the specified account.
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// </summary>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task that determines when the operation is complete.</returns>
        Task SendVerificationEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IAccountStore"/> (either a <see cref="IGroup"/> or <see cref="Directory.IDirectory"/>)
        /// used to persist new accounts created by the application, or <c>null</c> if no account store has been designated.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the default <see cref="IAccountStore"/>,
        /// or <c>null</c> if no default <see cref="IAccountStore"/> has been designated.</returns>
        Task<IAccountStore> GetDefaultAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a Directory.
        /// However, this could be a Group in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.</returns>
        Task<IAccountStore> GetDefaultGroupStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IGroup"/> that may be used by this application in the application's <see cref="GetDefaultGroupStoreAsync(CancellationToken)"/>.
        /// <para>This is a convenience method. It merely delegates to the application's designated default group store.</para>
        /// </summary>
        /// <param name="group">The group to create/persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the new <see cref="IGroup"/> that may be used by this application.</returns>
        /// <exception cref="Error.ResourceException">
        /// The application does not have a designated default group store, or the
        /// designated default group store does not allow new groups to be created.
        /// </exception>
        Task<IGroup> CreateGroupAsync(IGroup group, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// <para>Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will previously need to execute
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> again in order to obtain a new password reset token.</para>
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount"/> if the token is successfully validated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the account matching the specified token.</returns>
        /// <exception cref="Error.ResourceException">The token is not valid.</exception>
        Task<IAccount> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">There is no account that matches the specified email address.</exception>
        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies a password reset token.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request paramter by your application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the <see cref="IAccount"/> matching the specified token.</returns>
        /// <exception cref="Error.ResourceException">The token is not valid.</exception>
        Task<IAccount> VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves a Provider-based <see cref="IAccount"/>. The account must exist in one of the Provider-based <see cref="Directory.IDirectory"/>
        /// assigned to the Application as an account store, and the Directory must also be <see cref="Directory.DirectoryStatus.Enabled"/>.
        /// If not in an assigned account store, the retrieval attempt will fail.
        /// </summary>
        /// <param name="request">
        /// The <see cref="IProviderAccountRequest"/> representing the Provider-specific account access data
        /// (e.g. an <c>accessToken</c>) used to verify the identity.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the result of the access request.</returns>
        /// <exception cref="Error.ResourceException">The access attempt failed.</exception>
        Task<IProviderAccountResult> GetAccountAsync(IProviderAccountRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of all accounts in this application.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to asynchronously list or search accounts.</returns>
        /// <example>
        ///     var allAccounts = await myApp.GetAccounts().ToListAsync();
        /// </example>
        IAsyncQueryable<IAccount> GetAccounts();

        /// <summary>
        /// Gets a queryable list of all groups accessible to this application.
        /// It will not only return any group associated directly as an <see cref="IAccountStore"/>
        /// but also every group that exists inside every directory associated as an account store.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search groups.</returns>
        IAsyncQueryable<IGroup> GetGroups();

        /// <summary>
        /// Gets a queryable list of all account store mappings accessible to the application.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccountStoreMapping}"/> that may be used to asynchronously list or search account store mappings.</returns>
        IAsyncQueryable<IAccountStoreMapping> GetAccountStoreMappings();
    }
}
