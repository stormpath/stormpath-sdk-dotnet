// <copyright file="TypeLoader.cs" company="Stormpath, Inc.">
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
using System.IO;
using System.Reflection;

namespace Stormpath.SDK.Impl.Utility
{
    internal abstract class TypeLoader<T> : ITypeLoader<T>
        where T : class
    {
        private readonly string fileName;
        private readonly string fullyQualifiedTypeName;

        public TypeLoader(string fileName, string fullyQualifiedTypeName)
        {
            this.fileName = fileName;
            this.fullyQualifiedTypeName = fullyQualifiedTypeName;
        }

        public bool TryLoad(out T instance, object[] constructorArguments = null)
        {
            // Try to load via AppDomain.CurrentDomain
            var appDomainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", this.fileName);
            bool foundViaAppDomain = File.Exists(appDomainPath);
            
            // Try to load via local working directory
            var naivePath = Path.GetFullPath(this.fileName);
            bool foundViaNaivePath = File.Exists(naivePath);
            
            if (!foundViaAppDomain && !foundViaNaivePath)
            {
                instance = null;
                return false;
            }
            
            var absolutePath = foundViaAppDomain
                ? appDomainPath
                : naivePath;

            Assembly assembly = Assembly.LoadFile(absolutePath);
            var type = assembly.GetType(this.fullyQualifiedTypeName);
            instance = Activator.CreateInstance(type, constructorArguments) as T;

            return instance != null;
        }
    }
}
