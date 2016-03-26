
Imports Stormpath.SDK
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Group

Public Class Account_Management
    Private client As IClient = Nothing

    Public Async Function CreateCloudDirectory() As Task
#Region "create_cloud_dir_req.vb"
        Dim captainsDirectory = Await client.CreateDirectoryAsync("Captains",
                                                                  "Captains from a variety of stories",
                                                                  DirectoryStatus.Enabled)
#End Region
    End Function

    Public Async Function SearchDirectoryGroupDescription1() As Task
        Dim myDirectory As IDirectory = Nothing

#Region "search_directory_group_description1.vb"
        Dim groupsInUS = Await myDirectory _
            .GetGroups() _
            .Where(Function(g) g.Description.Contains("/US")) _
            .ToListAsync()
#End Region
    End Function

    Public Async Function SearchDirectoryGroupDescription2() As Task
        Dim myDirectory As IDirectory = Nothing

#Region "search_directory_group_description2.vb"
        Dim groupsInUSEast = Await myDirectory _
            .GetGroups() _
            .Where(Function(g) g.Description.Contains("/US East")) _
            .ToListAsync()
#End Region
    End Function

    Public Async Function CreateDirectoryGroup() As Task
        Dim captainsDirectory As IDirectory = Nothing

#Region "create_group_req.vb"
        Dim officersGroup = Await captainsDirectory.CreateGroupAsync(
            "Starfleet Officers",
            "Commissioned officers in Starfleet")
#End Region
    End Function

    Public Async Function CreateDirectoryGroupDisabled() As Task
        Dim captainsDirectory As IDirectory = Nothing

#Region "create_disabled_group_req.vb"
        Dim officersGroup = client.Instantiate(Of IGroup)() _
            .SetName("Starfleet Officers") _
            .SetDescription("Commissioned officers in Starfleet") _
            .SetStatus(GroupStatus.Disabled)
        Await captainsDirectory.CreateGroupAsync(officersGroup)
#End Region
    End Function

    Public Async Function CreateDirectoryAccount() As Task
        Dim captainsDirectory As IDirectory = Nothing

#Region "create_account_in_dir_req.vb"
        Dim picard = client.Instantiate(Of IAccount)() _
            .SetUsername("jlpicard") _
            .SetEmail("capt@enterprise.com") _
            .SetGivenName("Jean-Luc") _
            .SetSurname("Picard") _
            .SetPassword("uGhd%a8Kl!")
        Await captainsDirectory.CreateAccountAsync(picard)
#End Region
    End Function

    Public Async Function AddAccountToGroup() As Task
        Dim officersGroup As IGroup = Nothing
        Dim picard As IAccount = Nothing

#Region "add_account_to_group_req.vb"
        Await officersGroup.AddAccountAsync(picard)
#End Region
    End Function

    Public Async Function CreateAccountWithDisabledWorkflow() As Task
        Dim myDirectory As IDirectory = Nothing
        Dim acct As IAccount = Nothing

#Region "create_account_disable_reg_workflow.vb"
        Await myDirectory.CreateAccountAsync(
            acct, Function(opt) opt.RegistrationWorkflowEnabled = False)
#End Region
    End Function

    Public Async Function CreateAccountWithMcfHash() As Task
        Dim myDirectory As IDirectory = Nothing
        Dim acct As IAccount = Nothing

#Region "create_account_mcf_hash.vb"
        Await myDirectory.CreateAccountAsync(
            acct, Function(opt) opt.PasswordFormat = PasswordFormat.MCF)
#End Region
    End Function

    Public Async Function AddCustomDataToAccount() As Task
        Dim picard As IAccount = Nothing

#Region "add_cd_to_account_req.vb"
        picard.CustomData("currentAssignment") = "USS Enterprise (NCC-1701-E)"
        Await picard.SaveAsync()
#End Region
    End Function

    Public Async Function SearchApplicationAccountsForWord() As Task
        Dim myApplication As IApplication = Nothing

#Region "search_app_accounts_for_word_req.vb"
        Dim accountsContainingLuc = Await myApplication _
            .GetAccounts() _
            .Filter("Luc") _
            .ToListAsync()
#End Region
    End Function

    Public Async Function SearchDirectoryAccountsForDisabled() As Task
        Dim myDirectory As IDirectory = Nothing

#Region "search_dir_accounts_for_disabled_req.vb"
        Dim disabledAccounts = Await myDirectory _
            .GetAccounts() _
            .Where(Function(acct) acct.Status = AccountStatus.Disabled) _
            .ToListAsync()
#End Region
    End Function

    Public Async Function SearchDirectoryAccountsByDatetimeWithin() As Task
        Dim myDirectory As IDirectory = Nothing

#Region "search_dir_accounts_for_create_date_req.vb"
        Dim accountsModifiedOnDec1 = Await myDirectory _
            .GetAccounts() _
            .Where(Function(acct) acct.ModifiedAt.Within(2015, 12, 1)) _
            .ToListAsync()
#End Region
    End Function

    Public Async Function SearchDirectoryAccountsByDatetimeGreaterThan() As Task
        Dim myDirectory As IDirectory = Nothing

#Region "search_dir_accounts_for_create_after_date_req.vb"
        Dim accountsCreatedAfterMidnightJan5 = Await myDirectory _
            .GetAccounts() _
            .Where(Function(acct) acct.CreatedAt > New DateTimeOffset(2016, 1, 5, 0, 0, 0, TimeSpan.Zero)) _
            .ToListAsync() ' Jan 5 2016, midnight GMT
#End Region
    End Function

    Public Async Function UpdateAccountPassword() As Task
        Dim picard As IAccount = Nothing

#Region "update_account_pwd.vb"
        picard.SetPassword("some_New+Value1234")
        Await picard.SaveAsync()
#End Region
    End Function

    Public Async Function ResetPassword() As Task
        Dim myApplication As IApplication = Nothing

#Region "reset1_trigger_req.vb"
        Dim token = Await myApplication.SendPasswordResetEmailAsync("phasma@empire.gov")
#End Region
    End Function

    Public Async Function ResetPasswordSpecifyAccountStore() As Task
        Dim myApplication As IApplication = Nothing
        Dim someAccountStore As Stormpath.SDK.AccountStore.IAccountStore = Nothing

#Region "reset1_trigger_req_accountstore.vb"
        Dim token = Await myApplication.SendPasswordResetEmailAsync(
            "phasma@empire.gov", someAccountStore)
#End Region
    End Function

    Public Async Function VerifyPasswordResetToken() As Task
        Dim myApplication As IApplication = Nothing
        Dim tokenFromRequest As String = Nothing

#Region "reset2_verify_token.vb"
        Try
            Dim account = Await myApplication.VerifyPasswordResetTokenAsync(tokenFromRequest)
        Catch rex As ResourceException
            ' Token is not valid!
        End Try
#End Region
    End Function

    Public Async Function ResetPasswordFinish() As Task
        Dim myApplication As IApplication = Nothing
        Dim tokenFromRequest As String = Nothing
        Dim newPassword As String = Nothing

#Region "reset3_update.vb"
        Await myApplication.ResetPasswordAsync(tokenFromRequest, newPassword)
#End Region
    End Function

    Public Async Function VerifyEmailFinish() As Task
        Dim tokenFromRequest As String = Nothing

#Region "verify_email_req.vb"
        Try
            Dim account = Await client.VerifyAccountEmailAsync(tokenFromRequest)
        Catch rex As ResourceException
            ' The token is invalid
        End Try
#End Region
    End Function

    Public Async Function ResendVerificationEmail() As Task
        Dim myApplication As IApplication = Nothing

#Region "resend_verification_email.vb"
        Await myApplication.SendVerificationEmailAsync("han@newrepublic.gov")
#End Region
    End Function
End Class