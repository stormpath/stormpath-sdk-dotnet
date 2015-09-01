// <copyright file="IAccountCreation.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Account
{
    public interface IAccountCreation
    {
        Task<IAccount> CreateAccountAsync(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccount> CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccount> CreateAccountAsync(IAccount account, CancellationToken cancellationToken = default(CancellationToken));

        Task<IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken = default(CancellationToken));
    }
}
