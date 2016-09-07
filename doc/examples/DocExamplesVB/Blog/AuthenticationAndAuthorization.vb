Imports Stormpath.SDK
Imports Stormpath.SDK.Api
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Sync
Imports System.Linq
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.AccountStore
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Group

''' <summary>
''' Code listings for https://stormpath.com/blog/easy-authentication-and-authorization-in-visual-basic-dotnet
''' </summary>
Public Class AuthenticationAndAuthorization
    public sub CreateClient()
        Dim client = Clients.Builder _
            .SetApiKeyFilePath("[path-to-file]") _
            .Build()
    End sub

    public Async Function GetDefaultApplication() as Task
        Dim client As IClient

        Dim app = Await client.GetApplications _
            .Where(Function(a) a.Name = "My Application") _
            .FirstAsync()
    End Function

    public sub GetDefaultApplicationSynchronously()
        Dim client As IClient

        Dim app = client.GetApplications _
            .Synchronously() _
            .Where(Function(a) a.Name = "My Application") _
            .First()
    End sub

    public Async Function CreatingUser() As task
        dim client as IClient = Nothing
        dim app as IApplication = Nothing

        Dim joe = client.Instantiate(Of IAccount) _
            .SetGivenName("Joe") _
            .SetSurname("Stormtrooper") _
            .SetEmail("tk421@deathstar.co") _
            .SetPassword("Changeme!123")

        Await app.CreateAccountAsync(joe)
    End Function

    public Async Function AuthenticateUser() As Task
        dim app as IApplication

        Try
            Dim loginResult = Await app.AuthenticateAccountAsync("tk421@deathstar.co", "Changeme!123")
            Dim loginAccount = Await loginResult.GetAccountAsync()
            Console.WriteLine("User " & loginAccount.FullName & " logged in!")
        Catch rex As ResourceException
            Console.WriteLine("Could not log in. Error: " & rex.Message)
        End Try
    End Function

    public Async Function CreateGroups() as Task
        Dim client as IClient
        dim app as IApplication

        ' In a production application, these would be created beforehand and only once
        Dim demoUsers = client.Instantiate(Of IGroup) _
            .SetName("DemoUsers") _
            .SetDescription("Demo users who do not have administrator access.")
        Dim demoAdmins = client.Instantiate(Of IGroup) _
            .SetName("DemoAdmins") _
            .SetDescription("Demo users who have administrator access.")

        Await Task.WhenAll(
            app.CreateGroupAsync(demoUsers),
            app.CreateGroupAsync(demoAdmins))
    End Function

    public Async Function AddUserToGroup() As Task
        dim joe as IAccount
        dim demoUsers as IGroup

        Await joe.AddGroupAsync(demoUsers)
    End Function

    public Async Function CheckIfUserInGroup() As Task
        dim joe as IAccount

        Dim roleNames = (Await joe.GetGroups().ToListAsync()) _
            .Select(Function(g) g.Name)

        Console.WriteLine("Roles for " & joe.GivenName & ": " &
                            String.Join(", ", roleNames))
    End Function

    public Async Function AddCustomData() As Task
        dim joe as IAccount

        joe.CustomData.Put(New With {.read = True, .write = False})
        Await joe.SaveAsync()
    End Function

    public Async Function ReadCustomData() As Task
                dim joe as IAccount


        Dim joeCustomData = Await joe.GetCustomDataAsync()
        Dim canRead = CBool(joeCustomData("read"))
        Dim canWrite = CBool(joeCustomData("write"))
        Console.WriteLine("Can Joe read? " & canRead)
        Console.WriteLine("Can Joe write? " & canWrite)
    End Function
End Class
