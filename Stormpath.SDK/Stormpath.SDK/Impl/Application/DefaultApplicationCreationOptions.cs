// <copyright file="DefaultApplicationCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationCreationOptions : IApplicationCreationOptions
    {
        private readonly bool createDirectory;
        private readonly string directoryName;

        public DefaultApplicationCreationOptions(bool createDirectory, string directoryName)
        {
            this.createDirectory = createDirectory;
            this.directoryName = directoryName;
        }

        bool IApplicationCreationOptions.CreateDirectory => this.createDirectory;

        string IApplicationCreationOptions.DirectoryName => this.directoryName;

        private bool IsDirectoryNameSpecified => !string.IsNullOrEmpty(this.directoryName);

        public string GetQueryString()
        {
            var argument = string.Empty;

            if (this.createDirectory)
            {
                argument = "createDirectory=" +
                    (this.IsDirectoryNameSpecified ? this.directoryName : "true");
            }

            return argument;
        }
    }
}
