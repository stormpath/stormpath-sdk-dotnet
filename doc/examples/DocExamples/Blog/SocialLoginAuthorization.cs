using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.DocExamples.Blogs
{
    /// <summary>
    /// Code listings for https://stormpath.com/blog/new-dotnet-release-social-login-and-authorization
    /// </summary>
    public class SocialLoginAuthorization
    {
        private async Task CreateAccountWithCustomData()
        {
            IApplication myApp = null;

            #region code
            var tk421 = await myApp.CreateAccountAsync(
                "Joe", "Stormtrooper", "tk421@galacticempire.co", "Changeme123!",
                new { isAdmin = false, access = "read,write" });
            #endregion
        }

        private async Task GetCustomData()
        {
            IAccount tk421 = null;

            #region code
            var customData = await tk421.GetCustomDataAsync();
            var isAdmin = (bool)customData["isAdmin"];
            var accessFlags = (string)customData["access"];
            #endregion
        }

        private async Task UpdateCustomData()
        {
            IAccount tk421 = null;

            #region code
            // Add profile information to an existing user account
            tk421.CustomData.Put("location", "Death Star");
            tk421.CustomData.Put("cellblockNumber", 1138);
            tk421.CustomData.Put("troopType", "stormtrooper");
            tk421.CustomData.Put("alternateEmail", "tk421@imperialxchange.com");
            await tk421.SaveAsync();
            #endregion
        }

        private async Task CreateAndAddGroup()
        {
            IClient client = null;
            IDirectory directory = null;
            IAccount tk421 = null;

            #region code
            // Create a group
            var admins = client.Instantiate<IGroup>();
            admins.SetName("Administrators");
            admins.SetDescription("Users who have administrator access");
            await directory.CreateGroupAsync(admins);

            // Assign a user
            await tk421.AddGroupAsync(admins);
            #endregion
        }

        private async Task CheckGroup()
        {
            IAccount tk421 = null;

            #region code
            bool isAdmin = await tk421.IsMemberOfGroupAsync("Administrators");
            if (isAdmin)
            {
                // Just admin things
            }
            #endregion
        }

        private async Task FacebookLogin()
        {
            IClient client = null;
            string accessToken = null;
            IApplication application = null;

            #region code
            // accessToken = the callback token provided by Facebook
            var request = client.Providers().Facebook().Account()
                .SetAccessToken(accessToken)
                .Build();
            var result = await application.GetAccountAsync(request);

            // true if this account didn't previously exist in Stormpath
            bool isNewAccount = result.IsNewAccount;

            // the Stormpath account details, pulled from Facebook
            IAccount account = result.Account;
            #endregion
        }

        private async Task EmailVerification()
        {
            IClient client = null;
            string token = null;

            #region code
            // token = the ?sptoken parameter passed to your application
            var verifiedAccount = await client.VerifyAccountEmailAsync(token);
            #endregion
        }
    }
}
