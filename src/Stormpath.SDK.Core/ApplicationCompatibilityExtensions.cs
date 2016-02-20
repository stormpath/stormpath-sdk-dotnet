// <copyright file="ApplicationCompatibilityExtensions.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK
{
    /// <summary>
    /// Provides backwards-compatibility for certain methods to comply with semver.
    /// </summary>
    public static class ApplicationCompatibilityExtensions
    {
        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The interface.</param>
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
        public static Task<IAuthenticationResult> AuthenticateAccountAsync(this IApplication application, Action<UsernamePasswordRequestBuilder> requestBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new UsernamePasswordRequestBuilder();
            requestBuilder(builder);
            var request = builder.Build();

            return application.AuthenticateAccountAsync(request, cancellationToken);
        }

        /// <summary>
        /// Authenticates an account's submitted principals and credentials (e.g. username and password)
        /// against the specified account store.
        /// If the account does not exist in the account store, the authentication attempt will fail.
        /// </summary>
        /// <param name="application">The interface.</param>
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
        public static Task<IAuthenticationResult> AuthenticateAccountAsync(this IApplication application, Action<UsernamePasswordRequestBuilder> requestBuilder, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new UsernamePasswordRequestBuilder();
            requestBuilder(builder);
            var request = builder.Build();

            return application.AuthenticateAccountAsync(request, responseOptions, cancellationToken);
        }

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
        /// <para>
        /// The <see cref="IEmailVerificationRequest"/> must contain the username or email identifying the account.
        /// </para>
        /// </remarks>
        /// <param name="application">The interface.</param>
        /// <param name="requestBuilderAction">Sets the options required for the verification email request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SendVerificationEmailAsync(this IApplication application, Action<EmailVerificationRequestBuilder> requestBuilderAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new EmailVerificationRequestBuilder((application as DefaultApplication).GetInternalAsyncDataStore());
            requestBuilderAction(builder);

            if (string.IsNullOrEmpty(builder.Login))
            {
                throw new ArgumentNullException(nameof(builder.Login));
            }

            var href = $"{application.Href}/verificationEmails";

            return (application as DefaultApplication).GetInternalAsyncDataStore().CreateAsync(href, builder.Build(), cancellationToken);
        }
    }
}
