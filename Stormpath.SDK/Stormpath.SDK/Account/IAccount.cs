// <copyright file="IAccount.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// An Account is a unique identity within a <see cref="IDirectory"/>. Accounts within a <see cref="IDirectory"/> or Group mapped to an <see cref="Application.IApplication"/> may log in to that Application.
    /// </summary>
    public interface IAccount : IResource, ISaveable<IAccount>, IDeletable, IAuditable
    {
        /// <summary>
        /// Gets the account's username. Unless otherwise specified, this is the same as <see cref="Email"/>.
        /// </summary>
        /// <value>This account's username, guaranteed to be unique for all accounts within a <see cref="IDirectory"/>.</value>
        string Username { get; }

        /// <summary>
        /// Gets the account's email address.
        /// </summary>
        /// <value>This account's email address, guaranteed to be unique for all accounts within a <see cref="IDirectory"/>.</value>
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
        /// Gets the account's status.
        /// </summary>
        /// <value>This account's status. Accounts that are not <see cref="AccountStatus.Enabled"/> may not login to applications.</value>
        AccountStatus Status { get; }

        /// <summary>
        /// Sets the account's email address, which must be unique among all other accounts within a <see cref="IDirectory"/>.
        /// </summary>
        /// <param name="email">The account's email address.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="Error.ResourceException">if email is in use</exception>
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
        /// <exception cref="Error.ResourceException">if the username is in use</exception>
        IAccount SetUsername(string username);

        /// <summary>
        /// Gets the account's parent <see cref="IDirectory"/> (where the account is stored).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the directory.</returns>
        Task<IDirectory> GetDirectoryAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the Stormpath <see cref="ITenant"/> that owns this Account resource.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the tenant.</returns>
        Task<ITenant> GetTenantAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
