using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;

namespace Stormpath.Demo
{
    public class EmailVerificationDemo : IDemo
    {
        private static bool ShouldContinue()
        {
            Console.WriteLine("This demo assumes you have an application called 'EmailWorkflowDemo' in your tenant.");
            Console.WriteLine("This application must be configured with the Verification Email workflow turned on.");
            Console.Write("Do you want to continue? [no] ");

            var input = Console.ReadLine();

            return input.Equals("yes", StringComparison.CurrentCultureIgnoreCase);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!ShouldContinue())
                return;

            var apiKey = ClientApiKeys.Builder()
                .SetFileLocation("~\\.stormpath\\apiKey.properties")
                .Build();

            // Create an IClient object. Everything starts here!
            var client = Clients.Builder()
                .SetApiKey(apiKey)
                .Build();

            var application = await client.GetApplications()
                .Where(x => x.Name == "EmailWorkflowDemo")
                .SingleOrDefaultAsync(cancellationToken);

            if (application == null)
            {
                Console.WriteLine("Error: Could not find application. Skipping demo.");
                return;
            }

            var createdAccount = await application.CreateAccountAsync("Email", "Test", "test@foo.co", "Changeme123!", cancellationToken);
            Console.WriteLine($"Account {createdAccount.Email} created");

            await application.SendVerificationEmailAsync("test@foo.co");
            Console.WriteLine($"Verification email resent to {createdAccount.Email}");

            var token = createdAccount.EmailVerificationToken.GetValue();
            var verifiedAccount = await client.VerifyAccountEmailAsync(token);
            Console.WriteLine($"{createdAccount.Email} verified with token {token}{Strings.NL}");

            // Clean up
            await createdAccount.DeleteAsync();
        }

        public async Task CleanupAsync()
        {
            return;
        }
    }
}
