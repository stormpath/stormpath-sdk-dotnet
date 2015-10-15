using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;

namespace Stormpath.Demo
{
    class EmailVerificationDemo
    {
        private static readonly string NL = Environment.NewLine;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Running the email verification workflow demo.");
            Console.WriteLine("This assumes you have an application called 'EmailWorkflowDemo' in your tenant.");
            Console.WriteLine($"This application must be configured with the Verification Email workflow turned on.{NL}");

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

            var verifiedAccount = await client.VerifyAccountEmailAsync(createdAccount.EmailVerificationToken.GetValue());
            Console.WriteLine($"{createdAccount.Email} verified with token {createdAccount.EmailVerificationToken}{NL}");

            // Clean up
            await createdAccount.DeleteAsync();
        }
    }
}
