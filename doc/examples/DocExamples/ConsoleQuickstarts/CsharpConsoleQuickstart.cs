using System;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using Stormpath.SDK.Error;

namespace examples.ConsoleQuickstarts
{
    public class CsharpConsoleQuickstart
    {
        public static async Task MainAsync()
        {
            // ## Create and Configure a Client
            // The first step to working with Stormpath is creating a Stormpath Client and configuring it using your API Key.
            //using Stormpath.SDK;
            //using Stormpath.SDK.Account;
            //using Stormpath.SDK.Client;
            //using Stormpath.SDK.Error;

            var client = Clients.Builder()
                // In production, use an apiKey.properties file, or environment variables!
                .SetApiKeyId("4VPVLB6K44RYSKRRVAB4TEBOX")
                .SetApiKeySecret("vgt/TVBvSdrS825z3mlDmXYgjLqdZ5tluYpHplQhBeo")
                .Build();

            // ## Retrieve Your Application
            // You'll need an `Application` to work with users in Stormpath, so we've created one for you. Access it using `client.GetApplicationAsync()`.
            var application = await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/7Ol377HU068lagCYk7U9XS");
            Console.WriteLine("Application name: " + application.Name);

            // ## Create a User Account
            // Use `client.Instantiate<IAccount>()` to create a new local Account object, and then `application.CreateAccountAsync()` to persist it on the server.
            var newAccount = client.Instantiate<IAccount>()
            .SetGivenName("Joe")
            .SetSurname("Stormtrooper")
            .SetUsername("tk421")
            .SetEmail("tk421@stormpath.com")
            .SetPassword("Changeme123!");
            newAccount.CustomData.Put("favoriteColor", "white");

            await application.CreateAccountAsync(newAccount);
            Console.WriteLine("User " + newAccount.FullName + " created!");

            // ## Retrieve the Account
            // Use the `application.GetAccounts()` collection to retrieve the account you just created.
            var account = await application.GetAccounts()
                .Where(acct => acct.Username == "tk421")
                .SingleAsync();

            Console.WriteLine("Account: " + account.Email);

            // ## Authenticate the Account
            // To log in with the account, use `application.AuthenticateAccountAsync()`.
            try
            {
                var loginResult = await application.AuthenticateAccountAsync("tk421", "Changeme123!");

                // If successful, the result will contain the account details:
                var accountDetails = await loginResult.GetAccountAsync();
                Console.WriteLine("Success! " + accountDetails.FullName + " logged in.");
            }
            catch (ResourceException rex)
            {
                // Bad login credentials!
                Console.WriteLine("Error logging in. " + rex.Message);
            }

            // ## Send a Password Reset Email
            // If the user forgets their password, use `application.SendPasswordResetEmailAsync()` to send them a password reset email.

            // Trigger the password reset email and create a password reset token
            var token = await application.SendPasswordResetEmailAsync("tk421@stormpath.com");

            // The token value can be retrieved:
            Console.WriteLine("Reset token value: " + token.GetValue());

            // The account in question can be retrieved from the token:
            var accountToReset = await token.GetAccountAsync();

            // ## Reset the User's Password
            // The email should include a link to a page on your application where the user can enter a new password. Use the token and this password together with `application.ResetPasswordAsync()` to complete the password reset process.
            try
            {
                // The token's value can be used to complete the reset operation:
                // (in a production application, the user would click the email link and send the token in a request).
                var resetResult = await application.ResetPasswordAsync(token.GetValue(), "newSecurePassword123!");

                // resetResult is an IAccount with the account details:
                Console.WriteLine("Reset password for " + resetResult.Email);
            }
            catch (ResourceException rex)
            {
                // The token has been used or is expired. User should request a new token.
                Console.WriteLine("Password reset failed.");
            }
        }
    }
}
