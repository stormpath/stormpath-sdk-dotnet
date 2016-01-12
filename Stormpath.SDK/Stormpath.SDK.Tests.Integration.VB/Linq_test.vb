' <copyright file="Linq_test.vb" company="Stormpath, Inc.">
' Copyright (c) 2016 Stormpath, Inc.
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
