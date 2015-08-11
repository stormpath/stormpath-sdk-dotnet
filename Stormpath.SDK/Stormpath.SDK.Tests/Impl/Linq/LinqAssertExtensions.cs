// <copyright file="LinqAssertExtensions.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public static class LinqAssertExtensions
    {
        public static void GeneratedArgumentsWere<T>(this IQueryable<T> queryable, string url, string resource, string arguments)
        {
            var resourceQueryable = queryable as ICollectionResourceQueryable<T>;
            if (resourceQueryable == null)
                Assert.Fail("This queryable is not an ICollectionResourceQueryable.");

            resourceQueryable.CurrentHref.ShouldBe($"{url}/{resource}?{arguments}");
        }
    }
}
