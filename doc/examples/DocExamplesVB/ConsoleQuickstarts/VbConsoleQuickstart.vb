Imports Stormpath.SDK
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Error

Public Class VbConsoleQuickstart

    Public Async Function MainAsync() As Task
        ' ## Create and Configure a Client
        ' The first step to working with Stormpath is creating a Stormpath Client and configuring it using your API Key.
        ' In production, use an apiKey.properties file, or environment variables!
        'Imports Stormpath.SDK
        'Imports Stormpath.SDK.Account
        'Imports Stormpath.SDK.Client
        'Imports Stormpath.SDK.Error

        Dim client = Clients.Builder() _
            .SetApiKeyId("4VPVLB6K44RYSKRRVAB4TEBOX") _
            .SetApiKeySecret("vgt/TVBvSdrS825z3mlDmXYgjLqdZ5tluYpHplQhBeo").Build()

        ' ## Retrieve Your Application
        ' You'll need an `Application` to work with users in Stormpath, so we've created one for you. Access it using `client.GetApplicationAsync()`.
        Dim application = Await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/7Ol377HU068lagCYk7U9XS")
        Console.WriteLine("Application name: " + application.Name)

        ' ## Create a User Account
        ' Use `client.Instantiate(Of IAccount)()` to create a new local Account object, and then `application.CreateAccountAsync()` to persist it on the server.
        Dim newAccount = client.Instantiate(Of IAccount)() _
            .SetGivenName("Joe") _
            .SetSurname("Stormtrooper") _
            .SetUsername("tk421") _
            .SetEmail("tk421@stormpath.com") _
            .SetPassword("Changeme123!")
        newAccount.CustomData.Put("favoriteColor", "white")

        Await application.CreateAccountAsync(newAccount)
        Console.WriteLine("User " + newAccount.FullName + " created!")

        ' ## Retrieve the Account
        ' Use the `application.GetAccounts()` collection to retrieve the account you just created.
        Dim account = Await application.GetAccounts() _
            .Where(Function(acct) acct.Username = "tk421") _
            .SingleAsync()

        Console.WriteLine("Account: " + account.Email)

        ' ## Authenticate the Account
        ' To log in with the account, use `application.AuthenticateAccountAsync()`.
        Try
            Dim loginResult = Await application.AuthenticateAccountAsync("tk421", "Changeme123!")

            ' If successful, the result will contain the account details:
            Dim accountDetails = Await loginResult.GetAccountAsync()
            Console.WriteLine("Success! " + accountDetails.FullName + " logged in.")
        Catch rex As ResourceException
            ' Bad login credentials!
            Console.WriteLine("Error logging in. " + rex.Message)
        End Try

        ' ## Send a Password Reset Email
        ' If the user forgets their password, use `application.SendPasswordResetEmailAsync()` to send them a password reset email.

        ' Trigger the password reset email and create a password reset token
        Dim token = Await application.SendPasswordResetEmailAsync("tk421@stormpath.com")

        ' The token value can be retrieved:
        Console.WriteLine("Reset token value: " + token.GetValue())

        ' The account in question can be retrieved from the token:
        Dim accountToReset = Await token.GetAccountAsync()

        ' ## Reset the User's Password
        ' The email should include a link to a page on your application where the user can enter a new password. Use the token and this password together with `application.ResetPasswordAsync()` to complete the password reset process.
        Try
            ' The token's value can be used to complete the reset operation:
            ' (in a production application, the user would click the email link and send the token in a request).
            Dim resetResult = Await application.ResetPasswordAsync(token.GetValue(), "newSecurePassword123!")

            ' resetResult is an IAccount with the account details:
            Console.WriteLine("Reset password for " + resetResult.Email)
        Catch rex As ResourceException
            ' The token has been used or is expired. User should request a new token.
            Console.WriteLine("Password reset failed.")
        End Try
    End Function

End Class
