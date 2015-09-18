// <copyright file="CreationOptions_tests.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Threading;
using NSubstitute;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class CreationOptions_tests
    {
        private static readonly string BaseHref = "http://api.foobar.com";

        private static void VerifyRequestContents(IRequestExecutor reqex, string queryString)
        {
            reqex.Received()
                .ExecuteAsync(
                    Arg.Is<IHttpRequest>(request =>
                        request.CanonicalUri.ToString().EndsWith(queryString)),
                    Arg.Any<CancellationToken>()).IgnoreAwait();
        }

        public class Application_options : IDisposable
        {
            private readonly IInternalDataStore dataStore;

            public Application_options()
            {
                this.dataStore = new StubDataStore(FakeJson.Application, BaseHref);
            }

            private void VerifyThat(ICreationOptions options, string resultsInQueryString)
            {
                var newApplication = new DefaultApplication(this.dataStore) as IApplication;
                this.dataStore.CreateAsync("/application", newApplication, options, CancellationToken.None);

                VerifyRequestContents(this.dataStore.RequestExecutor, resultsInQueryString);
            }

            [Fact]
            public void Create_without_directory()
            {
                var options = new ApplicationCreationOptionsBuilder()
                {
                    CreateDirectory = false
                }.Build();

                this.VerifyThat(options, resultsInQueryString: string.Empty);
            }

            [Fact]
            public void Create_application_request_with_default_directory()
            {
                var options = new ApplicationCreationOptionsBuilder()
                {
                    CreateDirectory = true
                }.Build();

                this.VerifyThat(options, resultsInQueryString: "?createDirectory=true");
            }

            [Fact]
            public void Create_application_request_with_named_directory()
            {
                var options = new ApplicationCreationOptionsBuilder()
                {
                    CreateDirectory = true,
                    DirectoryName = "Foobar Directory"
                }.Build();

                this.VerifyThat(options, resultsInQueryString: "?createDirectory=Foobar+Directory");
            }

            private bool isDisposed = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed)
                {
                    if (disposing)
                    {
                        this.dataStore.Dispose();
                    }

                    this.isDisposed = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                this.Dispose(true);
            }
        }

        public class Account_options : IDisposable
        {
            private readonly IInternalDataStore dataStore;

            public Account_options()
            {
                this.dataStore = new StubDataStore(FakeJson.Account, BaseHref);
            }

            private void VerifyThat(ICreationOptions options, string resultsInQueryString)
            {
                var newAccount = new DefaultAccount(this.dataStore) as IAccount;
                this.dataStore.CreateAsync("/account", newAccount, options, CancellationToken.None);

                VerifyRequestContents(this.dataStore.RequestExecutor, resultsInQueryString);
            }

            [Fact]
            public void Create_without_workflow_option()
            {
                var options = new AccountCreationOptionsBuilder()
                {
                }.Build();

                this.VerifyThat(options, resultsInQueryString: string.Empty);
            }

            [Fact]
            public void Create_with_workflow_override_enabled()
            {
                var options = new AccountCreationOptionsBuilder()
                {
                    RegistrationWorkflowEnabled = true
                }.Build();

                this.VerifyThat(options, resultsInQueryString: "?registrationWorkflowEnabled=true");
            }

            [Fact]
            public void Create_with_workflow_override_disabled()
            {
                var options = new AccountCreationOptionsBuilder()
                {
                    RegistrationWorkflowEnabled = false
                }.Build();

                this.VerifyThat(options, resultsInQueryString: "?registrationWorkflowEnabled=false");
            }

            private bool isDisposed = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed)
                {
                    if (disposing)
                    {
                        this.dataStore.Dispose();
                    }

                    this.isDisposed = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                this.Dispose(true);
            }
        }
    }
}
