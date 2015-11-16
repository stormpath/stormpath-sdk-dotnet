// <copyright file="SyncApplicationExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncApplicationExtensions
    {
        /// <summary>
        /// Synchronously authenticates an account's submitted principals and credentials (e.g. username and password).
        /// The account must be in one of the Application's assigned account stores.
        /// If not in an assigned account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. <see cref="UsernamePasswordRequest"/>).</param>
        /// <returns>
        /// The result of the authentication.
        /// The authenticated account can be obtained from <see cref="Sync.SyncAuthenticationResultExtensions.GetAccount()"/>.
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
        /// var result = myApp.AuthenticateAccount(loginRequest.Build(), response => response.Expand(x => x.GetAccount));
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
        /// A Task whose result is the result of the authentication.
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
        /// A Task whose result is the result of the authentication.
        /// The authenticated account can be obtained from <see cref="SyncAuthenticationResultExtensions.GetAccount(IAuthenticationResult)"/>.
        /// </returns>
        /// <exception cref="Error.ResourceException">The authentication attempt failed.</exception>
        /// <example>
        /// To attempt to authenticate against a specific account store, and cache the returned account details:
        /// <code>
        /// var result = await myApp.AuthenticateAccount(
        ///     request =>
        /// {
        ///     request.SetUsernameOrEmail("jsmith");
        ///     request.SetPassword("Password123#");
        ///     request.SetAccountStore(myAccountStore);
        /// },
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
        /// The authenticated account can be obtained from <see cref="Sync.SyncAuthenticationResultExtensions.GetAccount()"/>.
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
        /// <para>If you need to obtain the authenticated account details, use <see cref="AuthenticateAccount(IApplication, string, string)"/> instead.</para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <returns><c>true</c> if the authentication attempt succeeded; <c>false</c> otherwise.</returns>
        /// <example>
        /// if (myApp.TryAuthenticateAccount("jsmith", "Password123#"))
        /// {
        ///     // Login successful
        /// }
        /// </example>
        public static bool TryAuthenticateAccount(this IApplication application, string username, string password)
            => (application as IApplicationSync).TryAuthenticateAccount(username, password);

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
        /// <param name="application">The application</param>
        /// <param name="requestBuilderAction">Sets the options required for the verification email request.</param>
        public static void SendVerificationEmail(this IApplication application, Action<EmailVerificationRequestBuilder> requestBuilderAction)
            => (application as IApplicationSync).SendVerificationEmail(requestBuilderAction);

        /// <summary>
        /// Synchronously triggers the delivery of a new verification email for the specified account.
        /// <para>
        /// This method is useful in scenarios where the Account Registration and Verification workflow
        /// is enabled. If the welcome email has not been received by a newly registered account,
        /// then the user will not be able to login until the account is verified.
        /// </para>
        /// <para>This method re-sends the verification email and allows the user to verify the account.</para>
        /// </summary>
        /// <param name="application">The application</param>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        public static void SendVerificationEmail(this IApplication application, string usernameOrEmail)
            => (application as IApplicationSync).SendVerificationEmail(usernameOrEmail);

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Application resource.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>This account's tenant.</returns>
        public static ITenant GetTenant(this IApplication application)
            => (application as IApplicationSync).GetTenant();

        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>)
        /// used to persist new accounts created by the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The default <see cref="IAccountStore"/>,
        /// or <c>null</c> if no default <see cref="IAccountStore"/> has been designated.</returns>
        /// <example>
        /// Getting and using the default account store:
        /// <code>
        /// var accountStore = application.GetDefaultAccountStore();
        /// var accountStoreAsDirectory = accountStore as IDirectory;
        /// var accountStoreAsGroup = accountStore as IGroup;
        /// if (accountStoreAsDirectory != null)
        ///     // use as directory
        /// else if (accountStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        public static IAccountStore GetDefaultAccountStore(this IApplication application)
            => (application as IApplicationSync).GetDefaultAccountStore();

        /// <summary>
        /// Synchronously sets the <see cref="IAccountStore"/> (either a <see cref="IGroup"/> or a <see cref="Directory.IDirectory"/>)
        /// used to persist new accounts created by the Application.
        /// <para>
        /// Because an Application is not an <see cref="IAccountStore"/> itself, it delegates to a Group or Directory
        /// when creating accounts; this method sets the <see cref="IAccountStore"/> to which the Application delegates
        /// new account persistence.
        /// </para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        public static void SetDefaultAccountStore(this IApplication application, IAccountStore accountStore)
            => (application as IApplicationSync).SetDefaultAccountStore(accountStore);

        /// <summary>
        /// Synchronously gets the <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.
        /// <para>
        /// Stormpath's current REST API requires this to be a Directory.
        /// However, this could be a Group in the future, so do not assume it is always a
        /// Directory if you want your code to be function correctly if/when this support is added.
        /// </para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The <see cref="IAccountStore"/> used to persist new groups created by the application, or <c>null</c>
        /// if no account store has been designated.</returns>
        /// <example>
        /// Getting and using the default group store:
        /// <code>
        /// var groupStore = application.GetDefaultGroupStore();
        /// var groupStoreAsDirectory = groupStore as IDirectory;
        /// var groupStoreAsGroup = groupStore as IGroup;
        /// if (groupStoreAsDirectory != null)
        ///     // use as directory
        /// else if (groupStoreAsGroup != null)
        ///     // use as group
        /// </code>
        /// </example>
        public static IAccountStore GetDefaultGroupStore(this IApplication application)
            => (application as IApplicationSync).GetDefaultGroupStore();

        /// <summary>
        /// Synchronously sets the <see cref="IAccountStore"/> (a <see cref="Directory.IDirectory"/>)
        /// used to persist new groups created by the Application.
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
        /// <param name="application">The application.</param>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        public static void SetDefaultGroupStore(this IApplication application, IAccountStore accountStore)
            => (application as IApplicationSync).SetDefaultGroupStore(accountStore);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccountStoreMapping"/> for this Application, allowing the associated AccountStore
        /// to be used a source of accounts that may login to the Application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The AccountStoreMapping's ListIndex is negative, or the mapping could not be added to the Application.</exception>
        /// <example>
        /// Setting a new <see cref="IAccountStoreMapping"/>'s <see cref="IAccountStoreMapping.ListIndex"/> to <c>500</c> and then adding the mapping to
        /// an application with an existing 3-item list will automatically save the <see cref="IAccountStoreMapping"/>
        /// at the end of the list and set its <see cref="IAccountStoreMapping.ListIndex"/> value to <c>3</c> (items at index 0, 1, 2 were the original items,
        /// the new fourth item will be at index 3).
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroup();
        /// IAccountStoreMapping mapping = client.Instantiate<IAccountStoreMapping>();
        /// mapping.SetAccountStore(directoryOrGroup);
        /// mapping.SetListIndex(500);
        /// mapping = application.CreateAccountStoreMapping(mapping);
        /// </code>
        /// </example>
        public static IAccountStoreMapping CreateAccountStoreMapping(this IApplication application, IAccountStoreMapping mapping)
            => (application as IApplicationSync).CreateAccountStoreMapping(mapping);

        /// <summary>
        /// Synchronously adds a new <see cref="IAccountStore"/> to this Application and appends the resulting <see cref="IAccountStoreMapping"/>
        /// to the end of the Application's AccountStoreMapping list.
        /// <para>
        /// If you need to control the order of the added AccountStore, use the <see cref="CreateAccountStoreMapping(IApplication, IAccountStoreMapping)"/> method.
        /// </para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application.</exception>
        /// <example>
        /// <code>
        /// IAccountStore directoryOrGroup = GetDirectoryOrGroup();
        /// IAccountStoreMapping mapping = application.AddAccountStore(directoryOrGroup);
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore(this IApplication application, IAccountStore accountStore)
            => (application as IApplicationSync).AddAccountStore(accountStore);

        /// <summary>
        /// Synchronously adds a new <see cref="IAccountStore"/> to this Application. The given string can either be an <c>href</c> or a <c>name</c> of a
        /// <see cref="Directory.IDirectory"/> or <see cref="IGroup"/> belonging to the current <see cref="ITenant"/>.
        /// <para>
        /// If the provided value is an <c>href</c>, this method will get the proper Resource and add it as a new AccountStore in this
        /// Application without much effort. However, if the provided value is not an <c>href</c>, it will be considered as a <c>name</c>. In this case,
        /// this method will search for both a Directory and a Group whose names equal the provided <paramref name="hrefOrName"/>. If only
        /// one resource exists (either a Directory or a Group), then it will be added as a new AccountStore in this Application. However,
        /// if there are two resources (a Directory and a Group) matching that name, a <see cref="Error.ResourceException"/> will be thrown.
        /// </para>
        /// <para>
        /// Note: When using <c>names</c> this method is not efficient as it will search for both Directories and Groups within this Tenant
        /// for a matching name. In order to do so, some looping takes place at the client side: groups exist within directories, therefore we need
        /// to loop through every existing directory in order to find the required Group. In contrast, providing the Group's <c>href</c> is much more
        /// efficient as no actual search operation needs to be carried out.
        /// </para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="hrefOrName">Either the <c>href</c> or <c>name</c> of the desired <see cref="Directory.IDirectory"/> or <see cref="IGroup"/>.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        /// <exception cref="Error.ResourceException">The resource already exists as an account store in this Application.</exception>
        /// <exception cref="ArgumentException">The given <paramref name="hrefOrName"/> matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Providing an href:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = application.AddAccountStore("https://api.stormpath.com/v1/groups/2rwq022yMt4u2DwKLfzriP");
        /// </code>
        /// Providing a name:
        /// <code>
        /// IAccountStoreMapping accountStoreMapping = application.AddAccountStore("Foo Name");
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore(this IApplication application, string hrefOrName)
            => (application as IApplicationSync).AddAccountStore(hrefOrName);

        /// <summary>
        /// Synchronously adds a resource of type <typeparamref name="T"/> as a new <see cref="IAccountStore"/> to this Application. The provided <see cref="IAsyncQueryable{T}"/>
        /// must match a single <typeparamref name="T"/> in the current Tenant. If no compatible resource matches the query, this method will return <c>null</c>.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="query">Query to search for a resource of type <typeparamref name="T"/> in the current Tenant.</param>
        /// <typeparam name="T">The type of resource (either a <see cref="IDirectory"/> or a <see cref="IGroup"/>) to query for.</typeparam>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <c>null</c> if there is no resource matching the query.</returns>
        /// <exception cref="Error.ResourceException">The found resource already exists as an account store in the application.</exception>
        /// <exception cref="ArgumentException">The query matches more than one resource in the current Tenant.</exception>
        /// <example>
        /// Adding a directory by partial name:
        /// <code>
        /// IAccountStoreMapping mapping = application.AddAccountStore<IDirectory>(dirs => dirs.Where(d => d.Name.StartsWith(partialName)));
        /// </code>
        /// </example>
        public static IAccountStoreMapping AddAccountStore<T>(this IApplication application, Func<IQueryable<T>, IQueryable<T>> query)
            where T : IAccountStore
            => (application as IApplicationSync).AddAccountStore(query);

        /// <summary>
        /// Creates a new <see cref="IdSite.IIdSiteSyncCallbackHandler"/> used to synchronously handle HTTP replies from your ID Site to your application's <c>callbackUri</c>,
        /// as described in the <see cref="IApplication.NewIdSiteUrlBuilder"/> method.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="request">
        /// An instance of <see cref="Http.IHttpRequest"/>.
        /// See the <see cref="HttpRequests"/> helper class to help build this from an existing request.
        /// </param>
        /// <returns>An <see cref="IIdSiteAsyncCallbackHandler"/> that allows you customize how the <paramref name="request"/> will be handled.</returns>
        public static IdSite.IIdSiteSyncCallbackHandler NewIdSiteSyncCallbackHandler(this IApplication application, IHttpRequest request)
            => (application as IApplicationSync).NewIdSiteSyncCallbackHandler(request);

        /// <summary>
        /// Synchronously verifies the password reset token (received in the user's email) and immediately
        /// changes the password in the same request, if the token is valid.
        /// <para>Once the token has been successfully used, it is immediately invalidated and can't be used again.
        /// If you need to change the password again, you will previously need to execute
        /// <see cref="SendPasswordResetEmail(IApplication, string)"/> again in order to obtain a new password reset token.</para>
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount"/> if the token is successfully validated.</param>
        /// <returns>The account matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        public static IAccount ResetPassword(this IApplication application, string token, string newPassword)
            => (application as IApplicationSync).ResetPassword(token, newPassword);

        /// <summary>
        /// Synchronously sends a password reset email for the specified account email address.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount()"/>.</returns>
        /// <exception cref="SDK.Error.ResourceException">There is no account that matches the specified email address.</exception>
        public static IPasswordResetToken SendPasswordResetEmail(this IApplication application, string email)
            => (application as IApplicationSync).SendPasswordResetEmail(email);

        /// <summary>
        /// Synchronously sends a password reset email to an account in the specified <see cref="IAccountStore"/> matching
        /// the specified <paramref name="email"/>. If the email does not match an account in the specified
        /// <see cref="IAccountStore"/>, a <see cref="Error.ResourceException"/> will be thrown.
        /// If you are unsure of which of the application's mapped account stores might contain the account, use the more general
        /// <see cref="SendPasswordResetEmail(IApplication, string)"/> method instead.
        /// The email will contain a password reset link that the user can click or copy into their browser address bar.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <returns>A public static  whose result is the created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount()"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified <see cref="IAccountStore"/> is not mapped to this application, or there is no account that matches the specified email address in the specified <paramref name="accountStore"/>.
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
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <returns>A public static  whose result is the created <see cref="IPasswordResetToken"/>.
        /// You can obtain the associated account via <see cref="SyncPasswordResetTokenExtensions.GetAccount()"/>.</returns>
        /// <exception cref="Error.ResourceException">
        /// The specified AccountStore or Organization is not mapped to this application, or there is no account that matches the specified email address in the AccountStore or Organization.
        /// </exception>
        public static IPasswordResetToken SendPasswordResetEmail(this IApplication application, string email, string hrefOrNameKey)
            => (application as IApplicationSync).SendPasswordResetEmail(email, hrefOrNameKey);

        /// <summary>
        /// Synchronously verifies a password reset token.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="token">The verification token, usually obtained as a request paramter by your application.</param>
        /// <returns>The <see cref="IAccount"/> matching the specified token.</returns>
        /// <exception cref="SDK.Error.ResourceException">The token is not valid.</exception>
        public static IAccount VerifyPasswordResetToken(this IApplication application, string token)
            => (application as IApplicationSync).VerifyPasswordResetToken(token);

        /// <summary>
        /// Synchronously retrieves a Provider-based <see cref="IAccount"/>. The account must exist in one of the Provider-based <see cref="Directory.IDirectory"/>
        /// assigned to the Application as an account store, and the Directory must also be <see cref="Directory.DirectoryStatus.Enabled"/>.
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
    }
}
