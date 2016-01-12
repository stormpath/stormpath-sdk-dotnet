// <copyright file="IAccount.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// An Account is a unique identity within a <see cref="IDirectory">Directory</see>. Accounts within a <see cref="IDirectory">Directory</see> or <see cref="IGroup">Group</see> mapped to an <see cref="Application.IApplication">Application</see> may log in to that Application.
    /// </summary>
    public interface IAccount :
        IResource,
        IHasTenant,
        ISaveableWithOptions<IAccount>,
        IDeletable,
        IAuditable,
        IExtendable
    {
        /// <summary>
        /// Gets the account's username. Unless otherwise specified, this is the same as <see cref="Email"/>.
        /// </summary>
        /// <value>This account's username, guaranteed to be unique for all accounts within a <see cref="IDirectory">Directory</see>.</value>
        string Username { get; }

        /// <summary>
        /// Gets the account's email address.
        /// </summary>
        /// <value>This account's email address, guaranteed to be unique for all accounts within a <see cref="IDirectory">Directory</see>.</value>
        string Email { get; }

        /// <summary>
        /// Gets the account's full name, per Western cultural conventions. This is a computed property.
        /// </summary>
        /// <value>The result of combining the <see cref="GivenName"/> (aka 'first name' in Western cultures)
        /// followed by the <see cref="MiddleName"/> (if any) followed by the <see cref="Surname"/>
        /// (aka 'last name' in Western cultures).</value>
        string FullName { get; }

        /// <summary>
        /// Gets the account's given name (aka 'first name' in Western cultures).
        /// </summary>
        /// <value>This account's given name (aka 'first name' in Western cultures).</value>
        string GivenName { get; }

        /// <summary>
        /// Gets the account's middle name(s).
        /// </summary>
        /// <value>This account's middle name(s).</value>
        string MiddleName { get; }

        /// <summary>
        /// Gets the account's surname (aka 'last name' in Western cultures).
        /// </summary>
        /// <value>This account's surname (aka 'last name' in Western cultures).</value>
        string Surname { get; }

        /// <summary>
        /// Gets the account's email verification token. This will only be non-null if the account holder
        /// has been asked to verify their email account by clicking a link in an email.
        /// </summary>
        /// <value>An <see cref="IEmailVerificationToken"/>, or <see langword="null"/> if this account does not need to verify its email address.</value>
        IEmailVerificationToken EmailVerificationToken { get; }

        /// <summary>
        /// Gets the account's status.
        /// </summary>
        /// <value>This account's status. Accounts that are not <see cref="AccountStatus.Enabled"/> may not login to applications.</value>
        AccountStatus Status { get; }

        /// <summary>
        /// Sets the account's email address, which must be unique among all other accounts within a <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <param name="email">The account's email address.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="Error.ResourceException">Email is in use.</exception>
        IAccount SetEmail(string email);

        /// <summary>
        /// Sets the account's given name (aka 'first name' in Western cultures').
        /// </summary>
        /// <param name="givenName">The account's given name.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccount SetGivenName(string givenName);

        /// <summary>
        /// Sets the account's middle name(s).
        /// </summary>
        /// <param name="middleName">The account's middle name(s).</param>
        /// <returns>This instance for method chaining.</returns>
        IAccount SetMiddleName(string middleName);

        /// <summary>
        /// Sets (changes) the account's password to the specified raw (plaintext) password.
        /// <b>Only</b> call this method if you legitimately want to set the account password directly.
        /// When possible, it is advisable to use Stormpath's account password reset email workflow instead.
        /// <para>After calling this method, you must call <see cref="ISaveable{T}.SaveAsync(CancellationToken)"/>
        /// to propagate the change to the Stormpath servers.</para>
        /// </summary>
        /// <param name="password">The account's new raw (plaintext) password.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccount SetPassword(string password);

        /// <summary>
        /// Sets the account's status. Accounts that are not <see cref="AccountStatus.Enabled"/> may not login to applications.
        /// </summary>
        /// <param name="status">The account's status.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccount SetStatus(AccountStatus status);

        /// <summary>
        /// Sets the account's surname (aka 'last name' in Western cultures).
        /// </summary>
        /// <param name="surname">The account's surname.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccount SetSurname(string surname);

        /// <summary>
        /// Sets the account's username, which must be unique among all other accounts within a Directory.
        /// If you do not have need of a username, it is best to set the username to equal the <see cref="Email"/>.
        /// </summary>
        /// <param name="username">The account's username.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="Error.ResourceException">The username is in use.</exception>
        IAccount SetUsername(string username);

        /// <summary>
        /// Gets the account's parent <see cref="IDirectory">Directory</see> (where the account is stored).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This account's directory.</returns>
        Task<IDirectory> GetDirectoryAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns this account to the specified <see cref="IGroup">Group</see>.
        /// </summary>
        /// <param name="group">The Group this account will be added to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership">Group Membership</see> resource created reflecting
        /// the account-to-group association.
        /// </returns>
        Task<IGroupMembership> AddGroupAsync(IGroup group, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Assigns this account to the specified <see cref="IGroup">Group</see> represented
        /// by its (case-insensitive) name or <c>href</c>.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The new <see cref="IGroupMembership">Group Membership</see> resource created reflecting
        /// the account-to-group association.
        /// </returns>
        Task<IGroupMembership> AddGroupAsync(string hrefOrName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes this <see cref="IAccount">Account</see> from the specified <see cref="IGroup">Group</see>.
        /// </summary>
        /// <param name="group">The group object from which the account must be removed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The account does not belong to the specified group.</exception>
        Task<bool> RemoveGroupAsync(IGroup group, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes this <see cref="IAccount">Account</see> from the specified <see cref="IGroup">Group</see>
        /// represented by its (case-insensitive) name or <c>href</c>.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group object from which the account must be removed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <exception cref="System.InvalidOperationException">The account does not belong to the specified group.</exception>
        Task<bool> RemoveGroupAsync(string hrefOrName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets whether this account belongs to the group whose name or <c>href</c> is
        /// (case-insensitive) equal to the specified value.
        /// </summary>
        /// <param name="hrefOrName">The <c>href</c> or name of the group to check.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the account belongs to the specified group; <see langword="false"/> otherwise.</returns>
        Task<bool> IsMemberOfGroupAsync(string hrefOrName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the ProviderData Resource belonging to the account.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The ProviderData Resource belonging to the account.</returns>
        Task<IProviderData> GetProviderDataAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of the <see cref="IApplication">Applications</see> the Account may log in to.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IApplication}"/> that may be used to asynchronously list or search Applications.</returns>
        /// <example>
        /// <code>
        /// var allApplications = await account.GetApplications().ToListAsync();
        /// </code>
        /// </example>
        IAsyncQueryable<IApplication> GetApplications();

        /// <summary>
        /// Gets a queryable list of the account's assigned <see cref="IGroup">Groups</see>.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search Groups.</returns>
        /// <example>
        /// <code>
        /// var allGroups = await account.GetGroups().ToListAsync();
        /// </code>
        /// </example>
        IAsyncQueryable<IGroup> GetGroups();

        /// <summary>
        /// Returns all <see cref="IGroupMembership">Group Memberships</see> that reflect this account.
        /// This method is an alternative to <see cref="GetGroups"/> that returns the actual
        /// association entity representing the Account and a <see cref="IGroup">Group</see>.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroupMembership}"/> that may be used to asynchronously list or search Group Memberships.</returns>
        IAsyncQueryable<IGroupMembership> GetGroupMemberships();

        /// <summary>
        /// Gets a queryable list of the active <see cref="IAccessToken">Access Tokens</see> that belong to the account.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccessToken}"/> that may be used to asynchronously list or search Access Tokens.</returns>
        /// <example>
        /// <code>
        /// var activeAccessTokens = await account.GetAccessTokens().ToListAsync();
        /// </code>
        /// </example>
        IAsyncQueryable<IAccessToken> GetAccessTokens();

        /// <summary>
        /// Gets a queryable list of the active <see cref="IRefreshToken">Refresh Tokens</see> that belong to the account.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IRefreshToken}"/> that may be used to asynchronously list or search Refresh Tokens.</returns>
        /// <example>
        /// <code>
        /// var refreshTokens = await account.GetRefreshTokens().ToListAsync();
        /// </code>
        /// </example>
        IAsyncQueryable<IRefreshToken> GetRefreshTokens();
    }
}
