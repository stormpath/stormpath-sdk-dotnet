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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Application
{
    public interface IApplication : IResource, ISaveable<IApplication>, IDeletable, IAuditable, IAccountCreation
    {
        string Name { get; }

        string Description { get; }

        ApplicationStatus Status { get; }

        IApplication SetDescription(string description);

        IApplication SetName(string name);

        IApplication SetStatus(ApplicationStatus status);

        Task<IAuthenticationResult> AuthenticateAccountAsync(IAuthenticationRequest request, CancellationToken cancellationToken = default(CancellationToken));

        Task<IAuthenticationResult> AuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> TryAuthenticateAccountAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        ICollectionResourceQueryable<IAccount> GetAccounts();

        ICollectionResourceQueryable<IAccountStoreMapping> GetAccountStoreMappings(CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccountStore> GetDefaultAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccountStore> GetDefaultGroupStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccount> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default(CancellationToken));

        Task<IPasswordResetToken> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccount> VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default(CancellationToken));
    }
}
