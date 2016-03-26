// <copyright file="SyncRefreshTokenExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous Refresh to the methods available on <see cref="IRefreshToken"/>.
    /// </summary>
    public static class SyncRefreshTokenExtensions
    {
        /// <summary>
        /// Synchronously retrieves the <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="refreshToken">The Refresh token.</param>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        public static IAccount GetAccount(this IRefreshToken refreshToken)
            => (refreshToken as IRefreshTokenSync).GetAccount();

        /// <summary>
        /// Synchronously retrieves the <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="refreshToken">The Refresh token.</param>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        /// <param name="retrievalOptions">The retrieval options.</param>
        public static IAccount GetAccount(this IRefreshToken refreshToken, Action<IRetrievalOptions<IAccount>> retrievalOptions)
            => (refreshToken as IRefreshTokenSync).GetAccount(retrievalOptions);

        /// <summary>
        /// Synchronously retrieves the <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="refreshToken">The Refresh token.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        public static IApplication GetApplication(this IRefreshToken refreshToken)
            => (refreshToken as IRefreshTokenSync).GetApplication();

        /// <summary>
        /// Synchronously retrieves the <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="refreshToken">The Refresh token.</param>
        /// <param name="retrievalOptions">The retrieval options.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        public static IApplication GetApplication(this IRefreshToken refreshToken, Action<IRetrievalOptions<IApplication>> retrievalOptions)
            => (refreshToken as IRefreshTokenSync).GetApplication(retrievalOptions);
    }
}
