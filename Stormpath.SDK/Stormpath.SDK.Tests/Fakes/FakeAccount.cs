// <copyright file="FakeAccount.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.CustomData;

namespace Stormpath.SDK.Tests.Fakes
{
    public class FakeAccount : IAccount
    {
        public string Href { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string GivenName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public AccountStatus Status { get; set; }

        public IEmbeddedCustomData CustomData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<Directory.IDirectory> GetDirectoryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Tenant.ITenant> GetTenantAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IAccount> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public IAccount SetEmail(string email)
        {
            throw new NotImplementedException();
        }

        public IAccount SetGivenName(string givenName)
        {
            throw new NotImplementedException();
        }

        public IAccount SetMiddleName(string middleName)
        {
            throw new NotImplementedException();
        }

        public IAccount SetPassword(string password)
        {
            throw new NotImplementedException();
        }

        public IAccount SetStatus(AccountStatus status)
        {
            throw new NotImplementedException();
        }

        public IAccount SetSurname(string surname)
        {
            throw new NotImplementedException();
        }

        public IAccount SetUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICustomData> GetCustomDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
