// <copyright file="TypeLoader.cs" company="Stormpath, Inc.">
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
using System.IO;
using System.Linq;
using System.Reflection;

#if !NET45 && !NET451
using Microsoft.Extensions.PlatformAbstractions;
#endif

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed class TypeLoader
    {
        private readonly string libraryName;
        private readonly string fileName;
        private readonly string fullyQualifiedTypeName;

        public TypeLoader(string libraryName, string fileName, string fullyQualifiedTypeName)
        {
            this.libraryName = libraryName;
            this.fileName = fileName;
            this.fullyQualifiedTypeName = fullyQualifiedTypeName;
        }

        public Type Load()
        {
#if NET45 || NET451
            var path = this.GetPath();

            if (!string.IsNullOrEmpty(path))
            {
                var assembly = Assembly.LoadFile(path);
                return assembly.GetType(this.fullyQualifiedTypeName);
            }
#else
            var referencedLibrary = PlatformServices.Default.LibraryManager.GetLibrary(libraryName);
            if (referencedLibrary != null)
            {
                var assemblyName = referencedLibrary.Assemblies.Single();
                var assembly = PlatformServices.Default.AssemblyLoadContextAccessor.Default.Load(assemblyName);
                var type = assembly.ExportedTypes.Where(t => t.FullName == this.fullyQualifiedTypeName).SingleOrDefault();

                if (type != null)
                {
                    return type;
                }
            }
#endif

            throw new Exception($"Could not find plugin '{libraryName}'.");
        }

#if NET45 || NET451
        private string GetPath()
        {
            // Try to load via AppDomain.CurrentDomain
            var appDomainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", this.fileName);
            bool foundViaAppDomain = File.Exists(appDomainPath);

            // Try to load via local working directory
            var naivePath = Path.GetFullPath(this.fileName);
            bool foundViaNaivePath = File.Exists(naivePath);

            if (!foundViaAppDomain && !foundViaNaivePath)
            {
                return null;
            }

            return foundViaAppDomain
                ? appDomainPath
                : naivePath;
        }
#endif
    }
}