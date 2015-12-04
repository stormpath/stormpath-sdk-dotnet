// <copyright file="IApplicationSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Group;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Application
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IApplication"/>.
    /// </summary>
    internal interface IApplicationSync : ISaveableWithOptionsSync<IApplication>, IDeletableSync, IExtendableSync, IAccountCreationActionsSync, IGroupCreationActionsSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AuthenticateAccountAsync(IAuthenticationRequest, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <returns>The result of the authentication.</returns>
        IAuthenticationResult AuthenticateAccount(IAuthenticationRequest request);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AuthenticateAccountAsync(IAuthenticationRequest, Action{IRetrievalOptions{IAuthenticationResult}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="request">Any supported <see cref="IAuthenticationRequest"/> object (e.g. created by <see cref="UsernamePasswordRequestBuilder"/>).</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>The result of the authentication.</returns>
        IAuthenticationResult AuthenticateAccount(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AuthenticateAccountAsync(Action{UsernamePasswordRequestBuilder}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="requestBuilder">Sets the login request parameters.</param>
        /// <returns>The result of the authentication.</returns>
        IAuthenticationResult AuthenticateAccount(Action<UsernamePasswordRequestBuilder> requestBuilder);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AuthenticateAccountAsync(Action{UsernamePasswordRequestBuilder}, Action{IRetrievalOptions{IAuthenticationResult}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="requestBuilder">Sets the login request parameters.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>The result of the authentication.</returns>
        IAuthenticationResult AuthenticateAccount(Action<UsernamePasswordRequestBuilder> requestBuilder, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AuthenticateAccountAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <returns>The result of the authentication.</returns>
        IAuthenticationResult AuthenticateAccount(string username, string password);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.TryAuthenticateAccountAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        /// <returns><see langword="true"/> if the authentication attempt succeeded; <see langword="false"/> otherwise.</returns>
        bool TryAuthenticateAccount(string username, string password);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendVerificationEmailAsync(Action{EmailVerificationRequestBuilder}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="requestBuilderAction">Sets the options required for the verification email request.</param>
        void SendVerificationEmail(Action<EmailVerificationRequestBuilder> requestBuilderAction);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendVerificationEmailAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="usernameOrEmail">The username or email identifying the account to send the verification email to.</param>
        void SendVerificationEmail(string usernameOrEmail);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.GetTenantAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Tenant.</returns>
        ITenant GetTenant();

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.GetDefaultAccountStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Account store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultAccountStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SetDefaultAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        void SetDefaultAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.GetDefaultGroupStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Group store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultGroupStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SetDefaultGroupStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        void SetDefaultGroupStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.CreateAccountStoreMappingAsync(IAccountStoreMapping, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping CreateAccountStoreMapping(IAccountStoreMapping mapping);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AddAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the Application's AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping AddAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AddAccountStoreAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="SDK.Directory.IDirectory"/> or <see cref="IGroup"/>.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping AddAccountStore(string hrefOrName);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.AddAccountStoreAsync{T}(Func{SDK.Linq.IAsyncQueryable{T}, SDK.Linq.IAsyncQueryable{T}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The type of resource (either a <see cref="SDK.Directory.IDirectory"/> or a <see cref="IGroup"/>) to query for.</typeparam>
        /// <param name="query">Query to search for a resource of type <typeparamref name="T"/> in the current Tenant.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <see langword="null"/> if there is no resource matching the query..</returns>
        IAccountStoreMapping AddAccountStore<T>(Func<IQueryable<T>, IQueryable<T>> query)
            where T : IAccountStore;

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.NewIdSiteAsyncCallbackHandler(IHttpRequest)"/>.
        /// </summary>
        /// <param name="request">
        /// An instance of <see cref="IHttpRequest"/>.
        /// See the <see cref="HttpRequests"/> helper class to help build this from an existing request.
        /// </param>
        /// <returns>An <see cref="SDK.IdSite.IIdSiteAsyncCallbackHandler"/> that allows you customize how the <paramref name="request"/> will be handled.</returns>
        SDK.IdSite.IIdSiteSyncCallbackHandler NewIdSiteSyncCallbackHandler(IHttpRequest request);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.ResetPasswordAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount"/> if the token is successfully validated.</param>
        /// <returns>The Account.</returns>
        IAccount ResetPassword(string token, string newPassword);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email, IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount"/> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <returns>The created <see cref="IPasswordResetToken"/>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email, string hrefOrNameKey);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.VerifyPasswordResetTokenAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <returns>The <see cref="IAccount"/> matching the specified token.</returns>
        IAccount VerifyPasswordResetToken(string token);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.GetAccountAsync(IProviderAccountRequest, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="request">
        /// The <see cref="IProviderAccountRequest"/> representing the Provider-specific account access data
        /// (e.g. an <c>accessToken</c>) used to verify the identity.
        /// </param>
        /// <returns>The result of the access request.</returns>
        IProviderAccountResult GetAccount(IProviderAccountRequest request);
    }
}
