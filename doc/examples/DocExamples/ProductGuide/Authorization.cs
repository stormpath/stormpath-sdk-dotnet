using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Client;
using Stormpath.SDK.CustomData;

// ReSharper disable PossibleNullReferenceException
#pragma warning disable 219

namespace Stormpath.SDK.DocExamples.ProductGuide
{
    public class Authorization
    {
        public async Task CustomDatPermissionsSimpleAsync()
        {
            IAccount account = null;

            #region code/csharp/authorization/example_perm_simple.cs

            var customData = await account.GetCustomDataAsync();

            customData.Put("create_admin", true);

            await customData.SaveAsync();

            #endregion
        }
        
        public async Task CustomDatPermissionsComplexAsync()
        {
            IAccount account = null;

            #region code/csharp/authorization/example_perm_complex.cs

            var customData = await account.GetCustomDataAsync();

            customData.Put("name", "createadmin");
            customData.Put("description", "This permission allows the account to create an admin");
            customData.Put("action", "read");
            customData.Put("resource", "/admin/create");
            customData.Put("effect", "allow");

            await customData.SaveAsync();

            #endregion
        }

        public async Task GetAccountWithCustomData()
        {
            IClient client = null;

            #region code/csharp/authorization/account_with_customdata_req.cs

            var account = await client.GetAccountAsync("account_href", req => req.Expand(acct => acct.GetCustomData()));
            var customData = await account.GetCustomDataAsync();

            #endregion
        }

        public async Task GetCustomDataDirectly()
        {
            IAccount account = null;

            #region code/csharp/authorization/account_customdata_only_req.cs

            var customData = await account.GetCustomDataAsync();

            #endregion
        }

        public async Task GetAccountGroups()
        {
            IAccount account = null;

            #region code/csharp/authorization/account_groups_req.cs

            var accountGroups = await account
                .GetGroups()
                .Expand(g => g.GetCustomData())
                .ToListAsync();

            #endregion

            #region code/csharp/authorization/get_first_group_customData.cs

            var groupCustomData = await accountGroups.First().GetCustomDataAsync();

            #endregion
        }
    }
}
