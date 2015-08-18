// <copyright file="DefaultAccount.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultAccount : IAccount
    {
        DateTimeOffset IAuditable.CreatedAt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.Email
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.GivenName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IResource.Href
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.MiddleName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        DateTimeOffset IAuditable.ModifiedAt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        AccountStatus IAccount.Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.Surname
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IAccount.Username
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Task<ICustomData> IExtendable.GetCustomDataAsync()
        {
            throw new NotImplementedException();
        }

        Task<IDirectory> IAccount.GetDirectoryAsync()
        {
            throw new NotImplementedException();
        }

        Task<IGroupList> IAccount.GetGroupsAsync()
        {
            throw new NotImplementedException();
        }

        Task<ITenant> IAccount.GetTenantAsync()
        {
            throw new NotImplementedException();
        }
    }
}
