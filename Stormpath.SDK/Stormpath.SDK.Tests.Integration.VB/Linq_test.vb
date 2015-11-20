Option Strict On
Option Explicit On
Option Infer On
Imports Shouldly
Imports Stormpath.SDK
Imports Stormpath.SDK.Client
Imports Xunit

Public Class Linq_test
    <Fact>
    Public Async Function Where_left_test() As Task
        Dim client = Clients.Builder() _
            .Build()

        Dim application = Await client.GetApplications() _
            .Where(Function(x) x.Name = "My Application") _
            .SingleAsync()

        application.ShouldNotBeNull()
    End Function

    <Fact>
    Public Async Function Where_right_test() As Task
        Dim client = Clients.Builder() _
            .Build()

        Dim application = Await client.GetApplications() _
            .Where(Function(x) "My Application" = x.Name) _
            .SingleAsync()

        application.ShouldNotBeNull()
    End Function
End Class
