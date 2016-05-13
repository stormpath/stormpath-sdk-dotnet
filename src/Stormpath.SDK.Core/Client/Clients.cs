// <copyright file="Clients.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Client;

#if !NET45
using System.Runtime.CompilerServices;
using Microsoft.Extensions.PlatformAbstractions;
#endif

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Static entry point for working with <see cref="IClient">Client</see> objects.
    /// </summary>
    public sealed class Clients
    {
        /// <summary>
        /// Gets a new <see cref="IClientBuilder"/> instance, used to fluently construct <see cref="IClient">Client</see> instances.
        /// </summary>
        /// <returns>A new <see cref="IClientBuilder"/> instance.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
        /// </example>
#if NET45
        public static IClientBuilder Builder()
        {
            var language = string.Empty;

            var callingAssembly = System.Reflection.Assembly.GetCallingAssembly();

            if (callingAssembly != null)
            {
                var analyzer = new Polyglot.AssemblyAnalyzer(callingAssembly, failSilently: true);
                language = analyzer.DetectedLanguage?.ToString();
            }

            var userAgentBuilder = new Net451UserAgentBuilder(
                Impl.Introspection.Platform.Analyze(),
                Impl.Introspection.Sdk.Analyze(),
                language);

            return new DefaultClientBuilder(userAgentBuilder);
        }
#else
        public static IClientBuilder Builder([CallerFilePath] string callerFilePath = null)
        {
            var language = string.Empty;

            language = new DnxCallerFileLanguageDetector(callerFilePath).Language;

            var userAgentBuilder = new DnxUserAgentBuilder(
                runtimeEnvironment: PlatformServices.Default.Runtime,
                appEnvironment: PlatformServices.Default.Application,
                language: language);

            return new DefaultClientBuilder(userAgentBuilder);
        }
#endif
        }
    }
