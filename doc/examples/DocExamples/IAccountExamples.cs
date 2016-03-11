// <copyright file="AccountCreationOptionsBuilderExamples.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Account;

namespace DocExamples
{
    public class IAccountExamples
    {
        private readonly IAccount account = null;

        public async Task Queryable()
        {
            #region QueryableApiKeys
            var allApiKeys = await account.GetApiKeys().ToListAsync();
            #endregion

            #region QueryableApplications
            var allApplications = await account.GetApplications().ToListAsync();
            #endregion

            #region QueryableGroups
            var allGroups = await account.GetGroups().ToListAsync();
            #endregion

            #region QueryableGroupMemberships
            var allGroupMemberships = await account.GetGroupMemberships().ToListAsync();
            #endregion

            #region QueryableAccessTokens
            var activeAccessTokens = await account.GetAccessTokens().ToListAsync();
            #endregion

            #region QueryableRefreshTokens
            var activeRefreshTokens = await account.GetRefreshTokens().ToListAsync();
            #endregion
        }
    }
}
