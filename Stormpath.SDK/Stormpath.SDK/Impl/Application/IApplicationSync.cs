// <copyright file="IApplicationSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Group;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Application
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IApplication">Application</see>.
    /// </summary>
    internal interface IApplicationSync :
        IHasTenantSync,
        ISaveableWithOptionsSync<IApplication>,
        IDeletableSync,
        IExtendableSync,
        IAccountCreationActionsSync,
        IGroupCreationActionsSync,
        IAccountStoreContainerSync<IApplicationAccountStoreMapping>
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
        /// <param name="newPassword">The new password that will be set to the <see cref="IAccount">Account</see> if the token is successfully validated.</param>
        /// <returns>The Account.</returns>
        IAccount ResetPassword(string token, string newPassword);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="accountStore">The AccountStore expected to contain an account with the specified email address.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email, IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.SendPasswordResetEmailAsync(string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="email">An email address of an <see cref="IAccount">Account</see> that may login to the application.</param>
        /// <param name="hrefOrNameKey">The href of the AccountStore, or the name key of the Organization, expected to contain an account with the specified email address.</param>
        /// <returns>The created <see cref="IPasswordResetToken">Password Reset Token</see>.</returns>
        IPasswordResetToken SendPasswordResetEmail(string email, string hrefOrNameKey);

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.VerifyPasswordResetTokenAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="token">The verification token, usually obtained as a request parameter by your application.</param>
        /// <returns>The <see cref="IAccount">Account</see> matching the specified token.</returns>
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

        /// <summary>
        /// Synchronous counterpart to <see cref="IApplication.GetOauthPolicyAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The <see cref="IOauthPolicy">OauthPolicy</see> associated with this <see cref="IApplication">Application</see>.</returns>
        IOauthPolicy GetOauthPolicy();
    }
}
