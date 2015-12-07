// <copyright file="IApplication.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Application
{
    /// <summary>
    /// Represents a Stormpath registered application.
    /// </summary>
    public interface IApplication : IResource, ISaveableWithOptions<IApplication>, IDeletable, IAuditable, IExtendable, IAccountCreationActions, IGroupCreationActions
    {
        /// <summary>
        /// Gets the Application's name.
        /// </summary>
        /// <value>The application's name. An application's name must be unique across all other applications within a Stormpath <see cref="ITenant"/>.</value>
        string Name { get; }

        /// <summary>
        /// Gets the Application description.
        /// </summary>
        /// <value>The application's description text.</value>
        string Description { get; }

        /// <summary>
        /// Gets the Application's status.
        /// </summary>
        /// <value>
        /// The Application's status.
        /// Application users may login to an <see cref="ApplicationStatus.Enabled"/> application.
        /// They may not login to a <see cref="ApplicationStatus.Disabled"/> application.
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
        /// <param name="name">The Application's name. Application names must be unique within a Stormpath <see cref="ITenant"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        IApplication SetName(string name);

        /// <summary>
        /// Sets the Application's status.
        /// </summary>
        /// <param name="status">The Application's status.
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
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
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
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
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
        /// Authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="requestBuilder">Sets the login request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To attempt to authenticate against a specific account store:
        /// <code>
        /// var result = await myApp.AuthenticateAccountAsync(request =>
        /// {
        ///     request.SetUsernameOrEmail("jsmith");
        ///     request.SetPassword("Password123#");
        ///     request.SetAccountStore(myAccountStore);
        /// });
        /// </code>
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(Action<UsernamePasswordRequestBuilder> requestBuilder, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="requestBuilder">Sets the login request parameters.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="IAuthenticationResult.GetAccountAsync(CancellationToken)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To attempt to authenticate against a specific account store, and cache the returned account details:
        /// <code>
        /// var result = await myApp.AuthenticateAccountAsync(
        ///     request =>
        ///     {
        ///         request.SetUsernameOrEmail("jsmith");
        ///         request.SetPassword("Password123#");
        ///         request.SetAccountStore(myAccountStore);
        ///     },
        ///     response => response.Expand(x => x.GetAccountAsync));
        /// </code>
        /// </example>
        Task<IAuthenticationResult> AuthenticateAccountAsync(Action<UsernamePasswordRequestBuilder> requestBuilder, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions, CancellationToken cancellationToken = default(CancellationToken));

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
        /// <para>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccountAsync(string, string, CancellationToken)"/> instead.</para>
        /// </summary>
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendVerificationEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Stormpath <see cref="ITenant"/> that owns this Application resource.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This application's tenant.</returns>
        Task<ITenant> GetTenantAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IAccountStore"/> (either a <see cref="IGroup"/> or <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="Account.IAccount"/>s created by the Application, or <see langword="null"/> if no Account Store has been designated.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The default <see cref="IAccountStore"/>,
        /// or <see langword="null"/> if no default <see cref="IAccountStore"/> has been designated.</returns>
        /// <example>
        /// Getting and using the default account store:
        /// <code>
        /// var accountStore = await application.GetDefaultAccountStoreAsync();
        /// var accountStoreAsDirectory = accountStore as IDirectory;
        /// var accountStoreAsGroup = accountStore as IGroup;
        /// if (accountStoreAsDirectory != null)
        ///     // use as directory
        /// else if (accountStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        Task<IAccountStore> GetDefaultAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the <see cref="IAccountStore"/> (either a <see cref="IGroup"/> or a <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="IAccount"/>s created by the Application.
        /// <para>
        /// Because an Application is not an <see cref="IAccountStore"/> itself, it delegates to a Group or Directory
        /// when creating accounts; this method sets the <see cref="IAccountStore"/> to which the Application delegates
        /// new account persistence.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetDefaultAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IAccountStore"/> used to persist new <see cref="IGroup"/>s created by the Application, or <see langword="null"/>
        /// if no Account Store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a <see cref="Directory.IDirectory"/>.
        /// However, this could be a Group in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccountStore"/> used to persist new groups created by the application, or <see langword="null"/>
        /// if no account store has been designated.</returns>
        /// <example>
        /// Getting and using the default group store:
        /// <code>
        /// var groupStore = await application.GetDefaultGroupStoreAsync();
        /// var groupStoreAsDirectory = groupStore as IDirectory;
        /// var groupStoreAsGroup = groupStore as IGroup;
        /// if (groupStoreAsDirectory != null)
        ///     // use as directory
        /// else if (groupStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        Task<IAccountStore> GetDefaultGroupStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the <see cref="IAccountStore"/> (a <see cref="Directory.IDirectory"/>)
        /// used to persist new <see cref="IGroup"/>s created by the Application.
        /// <para>
        /// Stormpath's current REST API requires this to be a Directory. However, this could be a Group in the future,
        /// so do not assume it is always a Directory if you want your code to function properly if/when this support is added.
        /// </para>
        /// <para>
        /// Because an Application is not an <see cref="IAccountStore"/> itself, it delegates to a Directory
        /// when creating groups; this method sets the <see cref="IAccountStore"/> to which the Application delegates
        /// new group persistence.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetDefaultGroupStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccountStoreMapping"/> for this Application, allowing the associated Account Store
        /// to be used a source of accounts that may login to the Application.
        /// </summary>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The AccountStoreMapping's ListIndex is negative, or the mapping could not be added to the Application.</exception>
        /// <example>
        /// Setting a new <see cref="IAccountStoreMapping"/>'s <see cref="IAccountStoreMapping.ListIndex"/> to <c>500</c> and then adding the mapping to
        /// an application with an existing 3-item list will automatically save the <see cref="IAccountStoreMapping"/>
        /// at the end of the list and set its <see cref="IAccountStoreMapping.ListIndex"/> value to <c>3</c> (items at index 0, 1, 2 were the original items,
        /// the new fourth item will be at index 3):
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroupAsync();
        /// IAccountStoreMapping mapping = client.Instantiate&lt;IAccountStoreMapping&gt;();
        /// mapping.SetAccountStore(directoryOrGroup);
        /// mapping.SetListIndex(500);
        /// mapping = await application.CreateAccountStoreMappingAsync(mapping);
        /// </code>
        /// </example>
        Task<IAccountStoreMapping> CreateAccountStoreMappingAsync(IAccountStoreMapping mapping, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new <see cref="IAccountStore"/> to this Application and appends the resulting <see cref="IAccountStoreMapping"/>
        /// to the end of the Application's AccountStoreMapping list.
        /// <para>
        /// If you need to control the order of the added AccountStore, use the <see cref="CreateAccountStoreMappingAsync(IAccountStoreMapping, CancellationToken)"/> method.
        /// </para>
        /// </summary>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application.</exception>
        /// <example>
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroupAsync();
        /// IAccountStoreMapping mapping = await application.AddAccountStore(directoryOrGroup);
        /// </code>
        /// </example>
        Task<IAccountStoreMapping> AddAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new <see cref="IAccountStore"/> to this Application. The given string can either be an <c>href</c> or a name of a
        /// <see cref="Directory.IDirectory"/> or <see cref="IGroup"/> belonging to the current <see cref="ITenant"/>.
        /// <para>
        /// If the provided value is an <c>href</c>, this method will get the proper Resource and add it as a new AccountStore in this
        /// Application without much effort. However, if the provided value is not an <c>href</c>, it will be considered as a name. In this case,
        /// this method will search for both a Directory and a Group whose names equal the provided <paramref name="hrefOrName"/>. If only
        /// one resource exists (either a Directory or a Group), then it will be added as a new AccountStore in this Application. However,
        /// if there are two resources (a Directory and a Group) matching that name, a <see cref="Error.ResourceException"/> will be thrown.
        /// </para>
        /// <para>
        /// Note: When using names this method is not efficient as it will search for both Directories and Groups within this Tenant
        /// for a matching name. In order to do so, some looping takes place at the client side: groups exist within directories, therefore we need
        /// to loop through every existing directory in order to find the required Group. In contrast, providing the Group's <c>href</c> is much more
        /// efficient as no actual search operation needs to be carried out.
        /// </para>
        /// </summary>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="Directory.IDirectory"/> or <see cref="IGroup"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application.</exception>
        /// <exception cref="ArgumentException">The given <paramref name="hrefOrName"/> matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Providing an href:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("https://api.stormpath.com/v1/groups/2rwq022yMt4u2DwKLfzriP");
        /// </code>
        /// Providing a name:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = await application.AddAccountStoreAsync("Foo Name");
        /// </code>
        /// </example>
        Task<IAccountStoreMapping> AddAccountStoreAsync(string hrefOrName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a resource of type <typeparamref name="T"/> as a new <see cref="IAccountStore"/> to this Application. The provided <see cref="IAsyncQueryable{T}"/>
        /// must match a single <typeparamref name="T"/> in the current Tenant. If no compatible resource matches the query, this method will return <see langword="null"/>.
        /// </summary>
        /// <param name="query">Query to search for a resource of type <typeparamref name="T"/> in the current Tenant.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of resource (either a <see cref="Directory.IDirectory"/> or a <see cref="IGroup"/>) to query for.</typeparam>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <see langword="null"/> if there is no resource matching the query.</returns>
        /// <exception cref="Error.ResourceException">The found resource already exists as an account store in the application.</exception>
        /// <exception cref="ArgumentException">The query matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Adding a directory by partial name:
        /// <code>
        /// IAccountStoreMapping mapping = await application.AddAccountStoreAsync&lt;IDirectory&gt;(dirs => dirs.Where(d => d.Name.StartsWith(partialName)));
        /// </code>
        /// </example>
        Task<IAccountStoreMapping> AddAccountStoreAsync<T>(Func<IAsyncQueryable<T>, IAsyncQueryable<T>> query, CancellationToken cancellationToken = default(CancellationToken))
            where T : IAccountStore;

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
        /// See the <see cref="HttpRequests"/> helper class to help build this from an existing request.
        /// </param>
        /// <returns>An <see cref="IIdSiteAsyncCallbackHandler"/> that allows you customize how the <paramref name="request"/> will be handled.</returns>
        IIdSiteAsyncCallbackHandler NewIdSiteAsyncCallbackHandler(IHttpRequest request);

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
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="Error.ResourceException">The token is not valid.</exception>
        Task<IAccount> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">There is no account that matches the specified email address.</exception>
        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a password reset email to an account in the specified <see cref="IAccountStore"/> matching
        /// the specified <paramref name="email"/>. If the email does not match an account in the specified
        /// <see cref="IAccountStore"/>, a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmailAsync(string, CancellationToken)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="IPasswordResetToken.GetAccountAsync(CancellationToken)"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified <see cref="IAccountStore"/> is not mapped to this application, or there is no account that matches the specified email address in the specified <paramref name="accountStore"/>.
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
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
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
        /// <returns>The <see cref="IAccount"/> matching the specified token.</returns>
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
        /// <returns>The result of the access request.</returns>
        /// <exception cref="Error.ResourceException">The access attempt failed.</exception>
        Task<IProviderAccountResult> GetAccountAsync(IProviderAccountRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of all Accounts in this Application.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to asynchronously list or search <see cref="Account.IAccount"/>s.</returns>
        /// <example>
        /// var allAccounts = await myApp.GetAccounts().ToListAsync();
        /// </example>
        IAsyncQueryable<IAccount> GetAccounts();

        /// <summary>
        /// Gets a queryable list of all Groups accessible to this application.
        /// It will not only return any group associated directly as an <see cref="IAccountStore"/>
        /// but also every group that exists inside every directory associated as an Account Store.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search <see cref="IGroup"/>s.</returns>
        /// <example>
        /// var allGroups = await myApp.GetGroups().ToListAsync();
        /// </example>
        IAsyncQueryable<IGroup> GetGroups();

        /// <summary>
        /// Gets a queryable list of all Account Store Mappings accessible to the Application.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccountStoreMapping}"/> that may be used to asynchronously list or search <see cref="IAccountStoreMapping"/>s.</returns>
        IAsyncQueryable<IAccountStoreMapping> GetAccountStoreMappings();
    }
}
