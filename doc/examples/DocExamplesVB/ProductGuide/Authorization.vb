Imports Stormpath.SDK
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Client

Namespace DocExamplesVB.ProductGuide

    Public Class Authorization

        Public Async Function CustomDatPermissionsSimple() As Task
            Dim account As IAccount = Nothing

#Region "code/vbnet/authorization/example_perm_simple.vb"

            Dim customData = Await account.GetCustomDataAsync()

            customData.Put("create_admin", True)

            Await customData.SaveAsync()

#End Region
        End Function

        Public Async Function CustomDatPermissionsComplex() As Task
            Dim account As IAccount = Nothing

#Region "code/vbnet/authorization/example_perm_complex.vb"

            Dim customData = Await account.GetCustomDataAsync()

            customData.Put("name", "createadmin")
            customData.Put("description", "This permission allows the account to create an admin")
            customData.Put("action", "read")
            customData.Put("resource", "/admin/create")
            customData.Put("effect", "allow")

            Await customData.SaveAsync()

#End Region
        End Function


        Public Async Function GetAccountWithCustomData() As Task
            Dim client As IClient = Nothing

#Region "code/vbnet/authorization/account_with_customdata_req.vb"

            Dim account = Await client.GetAccountAsync("account_href", Function(req) req.Expand(Function(acct) acct.GetCustomData()))
            Dim customData = Await account.GetCustomDataAsync()

#End Region
        End Function

        Public Async Function GetCustomDataDirectly() As Task
            Dim account As IAccount = Nothing

#Region "code/vbnet/authorization/account_customdata_only_req.vb"

            Dim customData = Await account.GetCustomDataAsync()

#End Region
        End Function


        Public Async Function GetAccountGroups() As Task
            Dim account As IAccount = Nothing

#Region "code/vbnet/authorization/account_groups_req.vb"

            Dim accountGroups = Await account.GetGroups() _
            .Expand(Function(g) g.GetCustomData()) _
            .ToListAsync()

#End Region

#Region "code/vbnet/authorization/get_first_group_customData.vb"

            Dim groupCustomData = Await accountGroups.First().GetCustomDataAsync()

#End Region
        End Function

    End Class

End Namespace