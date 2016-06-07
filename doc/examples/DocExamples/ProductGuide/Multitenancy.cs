using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Client;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Organization;

// ReSharper disable PossibleNullReferenceException
#pragma warning disable 219

namespace Stormpath.SDK.DocExamples.ProductGuide
{
    public class Multitenancy
    {
        public async Task SearchForGroupsByNameAndRole()
        {
            IDirectory myDirectory = null;

            #region code/csharp/multitenancy/search_groups_by_name_ex1.cs

            var roleGroups = await myDirectory.GetGroups()
                .Where(group => group.Name.StartsWith("bank-of-a.role."))
                .ToListAsync();

            #endregion

            #region code/csharp/multitenancy/search_groups_by_name_ex2.cs

            var tenantGroups = await myDirectory.GetGroups()
                .Where(group => group.Name.StartsWith("bank-of-a."))
                .ToListAsync();

            #endregion
        }

        public async Task CreateOrganization()
        {
            IClient client = null;

            #region code/csharp/multitenancy/create_org_req.cs

            var bankOfAOrg = client.Instantiate<IOrganization>()
                .SetName("Bank of A")
                .SetNameKey("bank-of-a")
                .SetStatus(OrganizationStatus.Enabled);

            await bankOfAOrg.SaveAsync();

            #endregion
        }

        public async Task AddAccountStoreToOrganization()
        {
            IOrganization bankOfAOrg = null;
            IDirectory existingDirectory = null;

            #region code/csharp/multitenancy/asm_to_org.cs

            // With a reference to an IDirectory:
            var newMapping = await bankOfAOrg.AddAccountStoreAsync(existingDirectory);

            // Or simply by href:
            newMapping = await bankOfAOrg.AddAccountStoreAsync("directory_href");

            #endregion

            #region code/csharp/multitenancy/asm_to_org_with_default_req.cs

            newMapping.SetDefaultAccountStore(true)
                .SetDefaultGroupStore(true);

            await newMapping.SaveAsync();

            #endregion
        }

        public async Task AddAccountStoreWithOptionalPropertiesToOrganization()
        {
            IClient client = null;
            IOrganization bankOfAOrg = null;
            IDirectory existingDirectory = null;

            #region code/csharp/multitenancy/create_oasm_req.cs

            var newMapping = client.Instantiate<IOrganizationAccountStoreMapping>()
                .SetAccountStore(existingDirectory)
                .SetListIndex(-1)
                .SetDefaultAccountStore(true)
                .SetDefaultGroupStore(true);

            await bankOfAOrg.CreateAccountStoreMappingAsync(newMapping);

            #endregion
        }

        public async Task CreateAccountInOrganization()
        {
            IClient client = null;
            IOrganization bankOfAOrg = null;

            #region code/csharp/multitenancy/create_oasm_full_req.cs

            var chewie = client.Instantiate<IAccount>()
                .SetGivenName("Chewbacca")
                .SetSurname("the Wookiee")
                .SetUsername("rrwwgggh")
                .SetEmail("chewie@kashyyyk.rim")
                .SetPassword("Changeme123!");
            chewie.CustomData.Put("favoriteShip", "Millennium Falcon");

            await bankOfAOrg.CreateAccountAsync(chewie);

            #endregion
        }
    }
}