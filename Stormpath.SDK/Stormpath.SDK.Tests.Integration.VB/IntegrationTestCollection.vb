' <copyright file="IntegrationTestCollection.vb" company="Stormpath, Inc.">
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

Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB
    <CollectionDefinition(NameOf(IntegrationTestCollection))>
    Public Class IntegrationTestCollection
        Implements ICollectionFixture(Of TestFixture)

        ' Intentionally left blank. This class only serves as an anchor for CollectionDefinition.
        ' The test fixture Is shared in Common between multiple integration test projects,
        ' but due to limitations of xUnit, this CollectionDefinition must be in the local assembly.
    End Class
End Namespace