// <copyright file="Api.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ApiApprover;
using ApprovalTests.Reporters;
using Shouldly;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Xunit;

namespace Stormpath.SDK.Tests.Sanity
{
    public class Api
    {
        [DebugOnlyFact]
        [UseReporter(typeof(DiffReporter))]
        public void No_public_api_changes()
        {
            PublicApiApprover.ApprovePublicApi(typeof(Client.IClient).Assembly.Location);
        }

        [Fact]
        public void All_Impl_members_are_hidden()
        {
            var typesInNamespace = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .Where(x =>
                    x.Namespace != null &&
                    x.Namespace.StartsWith("Stormpath.SDK.Impl", StringComparison.InvariantCultureIgnoreCase))
                .Where(x => x.IsPublic)
                .ToList();

            typesInNamespace.Count.ShouldBe(
                expected: 0,
                customMessage: $"These types are visible: {string.Join(", ", typesInNamespace)}");
        }

        [Fact]
        public void All_async_methods_have_CancellationToken_parameters()
        {
            var methodsInAssembly = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .Where(x => !Helpers.IsCompilerGenerated(x))
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));

            var asyncMethods = methodsInAssembly
                .Where(method =>
                    method.ReturnType == typeof(Task) ||
                    (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));

            var asyncMethodsWithoutCancellationToken = asyncMethods
                .Where(method => !method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)));

            asyncMethodsWithoutCancellationToken
                .Any()
                .ShouldBe(
                    expected: false,
                    customMessage: "These methods do not have a CancellationToken parameter:" + Helpers.NL + Helpers.PrettyMethodOutput(asyncMethodsWithoutCancellationToken));
        }

        [Fact]
        public void All_Impl_async_methods_have_required_CancellationToken_parameters()
        {
            var methodsInAssembly = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));

            var asyncMethods = methodsInAssembly
                .Where(method =>
                    method.ReturnType == typeof(Task) ||
                    (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));

            var asyncMethodsWithOptionalCT = asyncMethods
                .Where(method => method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken) && p.IsOptional))
                .Where(method => method.DeclaringType.Namespace.StartsWith("Stormpath.SDK.Impl"));

            var violatingMethods = asyncMethodsWithOptionalCT
                .Select(m => Helpers.PrettyPrintMethod($"{m.DeclaringType.Name}.{m.Name}", m.GetParameters()));

            // No optional/default values here!
            violatingMethods
                .Any()
                .ShouldBe(
                    expected: false,
                    customMessage: "These methods should not have an optional CancellationToken parameter:" + Helpers.NL + string.Join(Helpers.NL, violatingMethods));
        }

        [Fact]
        public void All_SDK_async_methods_have_optional_CancellationToken_parameters()
        {
            // Whitelist some methods that legitimately have nonoptional CancellationToken parameters
            var whitelistedMethods = new List<string>()
            {
                "IIdSiteAsyncResultListener.OnRegisteredAsync(IAccountResult, CancellationToken)",
                "IIdSiteAsyncResultListener.OnAuthenticatedAsync(IAccountResult, CancellationToken)",
                "IIdSiteAsyncResultListener.OnLogoutAsync(IAccountResult, CancellationToken)",
            };

            var methodsInAssembly = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));

            var asyncMethods = methodsInAssembly
                .Where(method =>
                    method.ReturnType == typeof(Task) ||
                    (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));

            var asyncMethodsWithRequiredCT = asyncMethods
                .Where(method => method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken) && !p.IsOptional))
                .Where(method => !method.DeclaringType.Namespace.StartsWith("Stormpath.SDK.Impl"));

            var violatingMethods = asyncMethodsWithRequiredCT
                .Select(m => Helpers.PrettyPrintMethod($"{m.DeclaringType.Name}.{m.Name}", m.GetParameters()))
                .Except(whitelistedMethods);

            // Must be all optional
            violatingMethods
                .Any()
                .ShouldBe(
                    expected: false,
                    customMessage: "These methods must have an optional CancellationToken parameter:" + Helpers.NL + string.Join(Helpers.NL, violatingMethods));
        }

        [Fact]
        public void Everything_sync_can_do_async_can_do_better()
        {
            // Whitelist some methods that legitimately are asymmetrical
            var whitelistedAsyncMethods = new List<string>()
            {
                "IAsyncQueryable`1.MoveNext()",
                "IAsynchronousHttpClient.Execute(IHttpRequest)",
                "IAsynchronousCache.Get(String)",
                "IAsynchronousCache.Put(String, IDictionary`2)",
                "IAsynchronousCache.Remove(String)",
                "IAsynchronousCacheProvider.GetCache(String)",
                "IAsynchronousNonceStore.ContainsNonce(String)",
                "IAsynchronousNonceStore.PutNonce(String)",
                "IIdSiteAsyncCallbackHandler.GetAccountResult()",
                "IIdSiteAsyncResultListener.OnRegistered(IAccountResult)",
                "IIdSiteAsyncResultListener.OnAuthenticated(IAccountResult)",
                "IIdSiteAsyncResultListener.OnLogout(IAccountResult)",
            };
            var whitelistedSyncMethods = new List<string>()
            {
                "IQueryable`1.Filter(String)",
                "IQueryable`1.Expand(Expression`1)",
                "IAsyncQueryable`1.Synchronously()",
                "IApplication.NewIdSiteSyncCallbackHandler(IHttpRequest)"
            };

            // Get normal async API from interfaces
            var asyncMethods = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .Where(t => t.IsPublic && t.IsInterface)
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(m => m.ReturnType == typeof(Task) ||
                            (m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
                .ToList();

            var asyncMethodsByName = asyncMethods
                .Select(m =>
                {
                    var nameWithoutAsync = m.Name.Replace("Async", string.Empty);

                    var argList = m
                        .GetParameters()
                        .Where(p => p.ParameterType != typeof(CancellationToken));

                    return Helpers.PrettyPrintMethod($"{m.DeclaringType.Name}.{nameWithoutAsync}", argList);
                })
                .ToList();

            // Get extension methods in Stormpath.SDK.Sync
            var syncMethods = Assembly
                .GetAssembly(typeof(Client.IClient))
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace == "Stormpath.SDK.Sync" &&
                            t.IsSealed && !t.IsGenericType && !t.IsNested)
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .ToList();

            var syncMethodsByName = syncMethods
                .Select(m =>
                {
                    var argList = m
                        .GetParameters()
                        .Skip(1)
                        .Select(p => p.ParameterType.Name);

                    return $"{m.GetParameters()[0].ParameterType.Name}.{m.Name}" +
                           $"({string.Join(", ", argList)})";
                })
                .ToList();

            var asyncButNotSync = asyncMethodsByName
                .Except(whitelistedAsyncMethods)
                .Except(syncMethodsByName)
                .ToList();

            var syncButNotAsync = syncMethodsByName
                .Except(whitelistedSyncMethods)
                .Except(asyncMethodsByName)
                .ToList();

            asyncButNotSync.Count.ShouldBe(
                0,
                $"These async methods do not have a corresponding sync method:{Helpers.NL}{string.Join(Helpers.NL, asyncButNotSync)}");

            syncButNotAsync.Count.ShouldBe(
                0,
                $"These sync methods do not have a corresponding async method:{Helpers.NL}{string.Join(Helpers.NL, syncButNotAsync)}");
        }

        [Fact]
        public void Expand_extension_methods_are_consistent_across_namespaces()
        {
            var getMethodInfoFunc = new Func<MethodInfo, string>(m =>
            {
                var parameters = m.GetParameters();
                var nestedType = parameters[0].ParameterType.GenericTypeArguments[0];
                var expandablesType = parameters[1].ParameterType
                    .GenericTypeArguments[0]
                    .GenericTypeArguments[0];

                return $"Expand<{nestedType.Name}>({expandablesType.Name})";
            });

            var asyncExpandMembers = typeof(AsyncQueryableExpandExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Select(getMethodInfoFunc)
                .ToList();

            var syncExpandMembers = typeof(SyncExpandExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Select(getMethodInfoFunc)
                .ToList();

            var retrievalExpandMembers = typeof(RetrievalOptionsExpandExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Select(getMethodInfoFunc)
                .ToList();

            asyncExpandMembers
                .SequenceEqual(syncExpandMembers)
                .ShouldBeTrue();

            asyncExpandMembers
                .SequenceEqual(retrievalExpandMembers)
                .ShouldBeTrue();
        }
    }
}
