Imports Stormpath.SDK
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Organization

Namespace DocExamplesVB.ProductGuide

    Public Class Multitenancy

        Public Async Function SearchForGroupsByNameAndRole() As Task
            Dim myDirectory As IDirectory = Nothing

#Region "code/vbnet/multitenancy/search_groups_by_name_ex1.vb"

            Dim roleGroups = Await myDirectory.GetGroups() _
            .Where(Function(group) group.Name.StartsWith("bank-of-a.role.")) _
            .ToListAsync()

#End Region

#Region "code/vbnet/multitenancy/search_groups_by_name_ex2.vb"

            Dim tenantGroups = Await myDirectory.GetGroups() _
            .Where(Function(group) group.Name.StartsWith("bank-of-a.")) _
            .ToListAsync()

#End Region
        End Function

        Public Async Function CreateOrganization() As Task
            Dim client As IClient = Nothing

#Region "code/vbnet/multitenancy/create_org_req.vb"

            Dim bankOfAOrg = client.Instantiate(Of IOrganization)() _
            .SetName("Bank of A") _
            .SetNameKey("bank-of-a") _
            .SetStatus(OrganizationStatus.Enabled)

            Await bankOfAOrg.SaveAsync()

#End Region
        End Function

        Public Async Function AddAccountStoreToOrganization() As Task
            Dim bankOfAOrg As IOrganization = Nothing
            Dim existingDirectory As IDirectory = Nothing

#Region "code/vbnet/multitenancy/asm_to_org.vb"

            ' With a reference to an IDirectory:
            Dim newMapping = Await bankOfAOrg.AddAccountStoreAsync(existingDirectory)

            ' Or simply by href:
            newMapping = Await bankOfAOrg.AddAccountStoreAsync("directory_href")

#End Region

#Region "code/vbnet/multitenancy/asm_to_org_with_default_req.vb"

            newMapping.SetDefaultAccountStore(True) _
                .SetDefaultGroupStore(True)

            Await newMapping.SaveAsync()

#End Region
        End Function

        Public Async Function AddAccountStoreWithOptionalPropertiesToOrganization() As Task
            Dim client As IClient = Nothing
            Dim bankOfAOrg As IOrganization = Nothing
            Dim existingDirectory As IDirectory = Nothing

#Region "code/vbnet/multitenancy/create_oasm_full_req.vb"

            Dim newMapping = client.Instantiate(Of IOrganizationAccountStoreMapping)() _
                .SetAccountStore(existingDirectory) _
                .SetListIndex(-1) _
                .SetDefaultAccountStore(True) _
                .SetDefaultGroupStore(True)

            Await bankOfAOrg.CreateAccountStoreMappingAsync(newMapping)

#End Region
        End Function

        Public Async Function CreateAccountInOrganization() As Task
            Dim client As IClient = Nothing
            Dim bankOfAOrg As IOrganization = Nothing

#Region "code/vbnet/multitenancy/create_oasm_full_req.vb"

            Dim chewie = client.Instantiate(Of IAccount)() _
                .SetGivenName("Chewbacca") _
                .SetSurname("the Wookiee") _
                .SetUsername("rrwwgggh") _
                .SetEmail("chewie@kashyyyk.rim") _
                .SetPassword("Changeme123!")
            chewie.CustomData.Put("favoriteShip", "Millennium Falcon")

            Await bankOfAOrg.CreateAccountAsync(chewie)

#End Region
        End Function

    End Class

End Namespace