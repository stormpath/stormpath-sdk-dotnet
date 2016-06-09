// <copyright file="Account_Management.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Mail;

namespace DocExamples.ProductGuide
{
#pragma warning disable CS0168 // The variable is declared but never used
    public class Account_Management
    {
        IClient client = null;

        public async Task CreateCloudDirectory()
        {
            #region code/csharp/account_management/create_cloud_dir_req.cs
            var captainsDirectory = await client.CreateDirectoryAsync(
                "Captains", 
                "Captains from a variety of stories",
                DirectoryStatus.Enabled);
            #endregion
        }

        public async Task SearchDirectoryGroupDescription1()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_directory_group_description1.cs
            var groupsInUS = await myDirectory
                .GetGroups()
                .Where(g => g.Description.Contains("/US"))
                .ToListAsync();
            #endregion
        }

        public async Task SearchDirectoryGroupDescription2()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_directory_group_description2.cs
            var groupsInUSEast = await myDirectory
                .GetGroups()
                .Where(g => g.Description.Contains("/US East"))
                .ToListAsync();
            #endregion
        }

        public async Task CreateDirectoryGroup()
        {
            IDirectory captainsDirectory = null;

            #region code/csharp/account_management/create_group_req.cs
            var officersGroup = await captainsDirectory.CreateGroupAsync(
                "Starfleet Officers",
                "Commissioned officers in Starfleet");
            #endregion
        }

        public async Task CreateDirectoryGroupDisabled()
        {
            IDirectory captainsDirectory = null;

            #region code/csharp/account_management/create_disabled_group_req.cs
            var officersGroup = client.Instantiate<IGroup>()
                .SetName("Starfleet Officers")
                .SetDescription("Commissioned officers in Starfleet")
                .SetStatus(GroupStatus.Disabled);
            await captainsDirectory.CreateGroupAsync(officersGroup);
            #endregion
        }

        public async Task CreateDirectoryAccount()
        {
            IDirectory captainsDirectory = null;

            #region code/csharp/account_management/create_account_in_dir_req.cs
            var picard = client.Instantiate<IAccount>()
                .SetUsername("jlpicard")
                .SetEmail("capt@enterprise.com")
                .SetGivenName("Jean-Luc")
                .SetSurname("Picard")
                .SetPassword("uGhd%a8Kl!");
            await captainsDirectory.CreateAccountAsync(picard);
            #endregion
        }

        public async Task AddAccountToGroup()
        {
            IGroup officersGroup = null;
            IAccount picard = null;

            #region code/csharp/account_management/add_account_to_group_req.cs
            await officersGroup.AddAccountAsync(picard);
            #endregion
        }

        public async Task CreateAccountWithDisabledWorkflow()
        {
            IDirectory myDirectory = null;
            IAccount acct = null;

            #region code/csharp/account_management/create_account_disable_reg_workflow.cs
            await myDirectory.CreateAccountAsync(acct, opt => opt.RegistrationWorkflowEnabled = false);
            #endregion
        }

        public async Task CreateAccountWithMcfHash()
        {
            IDirectory myDirectory = null;
            IAccount acct = null;

            #region code/csharp/account_management/create_account_mcf_hash.cs
            await myDirectory.CreateAccountAsync(acct, opt => opt.PasswordFormat = PasswordFormat.MCF);
            #endregion
        }

        public async Task AddCustomDataToAccount()
        {
            IAccount picard = null;

            #region code/csharp/account_management/add_cd_to_account_req.cs
            picard.CustomData["currentAssignment"] = "USS Enterprise (NCC-1701-E)";
            await picard.SaveAsync();
            #endregion
        }

        public async Task SearchApplicationAccountsForWord()
        {
            IApplication myApplication = null;

            #region code/csharp/account_management/search_app_accounts_for_word_req.cs
            var accountsContainingLuc = await myApplication
                .GetAccounts()
                .Filter("Luc")
                .ToListAsync();
            #endregion
        }

        public async Task SearchDirectoryAccountsForDisabled()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_dir_accounts_for_disabled_req.cs
            var disabledAccounts = await myDirectory
                .GetAccounts()
                .Where(acct => acct.Status == AccountStatus.Disabled)
                .ToListAsync();
            #endregion
        }

        public async Task SearchDirectoryAccountsByDatetimeWithin()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_dir_accounts_for_create_date_req.cs
            var accountsModifiedOnDec1 = await myDirectory
                .GetAccounts()
                .Where(acct => acct.ModifiedAt.Within(2015, 12, 01))
                .ToListAsync();
            #endregion
        }

        public async Task SearchDirectoryAccountsByDatetimeGreaterThan()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_dir_accounts_for_create_after_date_req.cs
            var accountsCreatedAfterMidnightJan5 = await myDirectory
                .GetAccounts()
                // Jan 5 2016, midnight GMT
                .Where(acct => acct.CreatedAt > new DateTimeOffset(2016, 1, 5, 00, 00, 00, TimeSpan.Zero))
                .ToListAsync();
            #endregion
        }

        public async Task UpdateAccountPassword()
        {
            IAccount picard = null;

            #region code/csharp/account_management/update_account_pwd.cs
            picard.SetPassword("some_New+Value1234");
            await picard.SaveAsync();
            #endregion
        }

        public async Task ResetPassword()
        {
            IApplication myApplication = null;

            #region code/csharp/account_management/reset1_trigger_req.cs
            var token = await myApplication.SendPasswordResetEmailAsync("phasma@empire.gov");
            #endregion
        }

        public async Task ResetPasswordSpecifyAccountStore()
        {
            IApplication myApplication = null;
            Stormpath.SDK.AccountStore.IAccountStore someAccountStore = null;

            #region code/csharp/account_management/reset1_trigger_req_accountstore.cs
            var token = await myApplication.SendPasswordResetEmailAsync(
                "phasma@empire.gov", someAccountStore);
            #endregion
        }

        public async Task VerifyPasswordResetToken()
        {
            IApplication myApplication = null;
            string tokenFromRequest = null;

            #region code/csharp/account_management/reset2_verify_token.cs
            try
            {
                var account = await myApplication.VerifyPasswordResetTokenAsync(tokenFromRequest);
            }
            catch (ResourceException rex)
            {
                // Token is not valid!
            }
            #endregion
        }

        public async Task ResetPasswordFinish()
        {
            IApplication myApplication = null;
            string tokenFromRequest = null;
            string newPassword = null;

            #region code/csharp/account_management/reset3_update.cs
            await myApplication.ResetPasswordAsync(tokenFromRequest, newPassword);
            #endregion
        }

        public async Task VerifyEmailFinish()
        {
            string tokenFromRequest = null;

            #region code/csharp/account_management/verify_email_req.cs
            try
            {
                var account = await client.VerifyAccountEmailAsync(tokenFromRequest);
            }
            catch (ResourceException rex)
            {
                // The token is invalid
            }
            #endregion
        }

        public async Task ResendVerificationEmail()
        {
            IApplication myApplication = null;

            #region code/csharp/account_management/resend_verification_email.cs
            await myApplication.SendVerificationEmailAsync("han@newrepublic.gov");
            #endregion
        }

        public async Task UpdatePasswordStrengthPolicy()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/update_dir_pwd_strength_req.cs
            // Get the Password Strength Policy from the Directory's Password Policy resource
            var passwordPolicy = await myDirectory.GetPasswordPolicyAsync();
            var strengthPolicy = await passwordPolicy.GetPasswordStrengthPolicyAsync();

            // Update and save
            strengthPolicy.SetMinimumLength(1)
                .SetMaximumLength(24)
                .SetMinimumSymbols(1);
            await strengthPolicy.SaveAsync();
            #endregion
        }

        public async Task EnablePasswordResetEmail()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/enable_pwd_reset_email.cs
            var passwordPolicy = await myDirectory.GetPasswordPolicyAsync();

            passwordPolicy.SetResetEmailStatus(EmailStatus.Enabled);
            await passwordPolicy.SaveAsync();
            #endregion
        }

        public async Task SearchPasswordModified()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/search_password_modified.cs
            var passwordsModifiedIn2015 = await myDirectory.GetAccounts()
                .Where(acct => acct.PasswordModifiedAt < new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .ToListAsync();
            #endregion
        }

        public async Task UpdatePreventReuse()
        {
            IDirectory myDirectory = null;

            #region code/csharp/account_management/update_prevent_reuse.cs
            var passwordPolicy = await myDirectory.GetPasswordPolicyAsync();
            var strengthPolicy = await passwordPolicy.GetPasswordStrengthPolicyAsync();

            strengthPolicy.SetPreventReuse(10);
            await strengthPolicy.SaveAsync();
            #endregion
        }
    }
#pragma warning restore CS0168 // The variable is declared but never used
}