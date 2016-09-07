using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.DocExamples.Blog
{
    /// <summary>
    /// Code listings for https://stormpath.com/blog/build-multi-tenant-dot-net-applications
    /// </summary>
    public class MultiTenantApplications
    {
        public async Task CreateOrganization()
        {
            IClient client = null;

            #region code
            var rebels = client.Instantiate<IOrganization>()
                .SetName("Rebel Alliance")
                .SetNameKey("rebels") // must be unique and follow DNS hostname rules
                .SetDescription("The Rebellion Against the Empire"); // optional
            await client.CreateOrganizationAsync(rebels);
            #endregion
        }

        public async Task UpdateOrganization()
        {
            IClient client = null;
            string organizationHref = null;

            #region code
            // organizationHref is the Stormpath href of the Organization
            var rebelsOrg = await client.GetOrganizationAsync(organizationHref);

            // Disable the Organization. Login attempts will be rejected.
            rebelsOrg.SetStatus(OrganizationStatus.Disabled);

            // Save changes
            await rebelsOrg.SaveAsync();
            #endregion
        }

        public async Task AddingAccountStores()
        {
            IOrganization rebels = null;
            IAccountStore myGroupOrDirectory = null;
            string hrefOfGroupOrDirectory = null;

            #region code
            // Add an existing IDirectory or IGroup instance:
            await rebels.AddAccountStoreAsync(myGroupOrDirectory);

            // Or, add by name:
            await rebels.AddAccountStoreAsync("My Group Or Directory Name");

            // Or, add by href:
            await rebels.AddAccountStoreAsync(hrefOfGroupOrDirectory);

            // Or, perform a lookup query that matches one Directory or Group:
            await rebels.AddAccountStoreAsync<IDirectory>(
                dirs => dirs.Where(d => d.Name.StartsWith("My Dir")));
            #endregion
        }

        public async Task UpdateDefaultAccountStore()
        {
            IOrganization rebels = null;
            IAccountStore myGroupOrDirectory = null;
            IDirectory myDirectory = null;

            #region code
            // Get the default Account Store for an Organization
            var defaultAccountStore = await rebels.GetDefaultAccountStoreAsync();
            // defaultAccountStore is an IDirectory or IGroup, or null

            // Set the default Account Store
            await rebels.SetDefaultAccountStoreAsync(myGroupOrDirectory);

            // Get the default Group Store for an Organization
            var defaultGroupStore = await rebels.GetDefaultGroupStoreAsync();
            // defaultGroupStore is an IDirectory, or null

            // Set the default Group Store
            await rebels.SetDefaultGroupStoreAsync(myDirectory);
            #endregion
        }

        public async Task AssignOrgToApp()
        {
            IApplication myApp = null;
            IOrganization rebels = null;

            #region code
            await myApp.AddAccountStoreAsync(rebels);
            #endregion
        }

        public async Task AuthenticateUsingOrganization()
        {
            IApplication myApp = null;
            IOrganization rebels = null;

            #region code
            // Using an IOrganization instance
            var loginResult1 = await myApp.AuthenticateAccountAsync(request =>
                {
                    request.SetUsernameOrEmail("lskywalker");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore(rebels); // restrict search to Rebel Alliance tenant only
                });

            // Using an Organization href string
            var loginResult2 = await myApp.AuthenticateAccountAsync(request =>
                {
                    request.SetUsernameOrEmail("lskywalker");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore(rebels.Href);
                });

            // Using an Organization nameKey
            // (This is handy if you're using a subdomain identifier
            // that you can quickly pull out of the request URL!)
            var loginResult3 = await myApp.AuthenticateAccountAsync(request =>
                {
                    request.SetUsernameOrEmail("lskywalker");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore("rebels"); // Tenant with nameKey "rebels" must exist
                });
            #endregion
        }
    }
}
