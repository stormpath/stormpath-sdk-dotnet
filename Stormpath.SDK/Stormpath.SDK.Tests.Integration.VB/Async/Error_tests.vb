' <copyright file="Error_tests.vb" company="Stormpath, Inc.">
' Copyright (c) 2015 Stormpath, Inc.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'      http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' </copyright>

Option Strict On
Option Explicit On
Option Infer On
Imports Shouldly
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.Async
    Public Class Error_tests
        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function When_resource_does_not_exist(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Try
                Dim bad = Await client.GetResourceAsync(Of IAccount)(tenant.Href + "/foobar")
            Catch rex As ResourceException
                rex.Code.ShouldBe(404)
                rex.DeveloperMessage.ShouldBe("The requested resource does not exist.")
                rex.Message.ShouldNotBe(Nothing)
                rex.MoreInfo.ShouldNotBe(Nothing)
                rex.HttpStatus.ShouldBe(404)
            End Try
        End Function
    End Class
End Namespace
