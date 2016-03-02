// <copyright file="CustomDataParsingHelper.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stormpath.SDK.CustomData;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal static class CustomDataParsingHelper
    {
        private static Lazy<MethodInfo> CachedCustomDataProxyIndexer = new Lazy<MethodInfo>(() =>
        {
            var proxyTypeInfo = typeof(ICustomDataProxy).GetTypeInfo();

            var indexer = (proxyTypeInfo
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == proxyTypeInfo
                    .CustomAttributes.ElementAtOrDefault(0)?.ConstructorArguments[0].Value.ToString()
                ) as PropertyInfo)?.GetMethod;

            return indexer;
        });

        private static MethodInfo GetCustomDataProxyIndexer()
            => CachedCustomDataProxyIndexer.Value;

        public static string GetFieldName(Expression node)
        {
            MethodCallExpression methodCall = null;

            // Handle casting: (string)CustomData["foo"] or (CustomData["foo"] as string)
            bool isCast = node.NodeType == ExpressionType.Convert
                || node.NodeType == ExpressionType.TypeAs;
            if (isCast)
            {
                // We just unwrap the Convert() expression and ignore the cast
                methodCall = (node as UnaryExpression)?.Operand as MethodCallExpression;
            }
            else
            {
                // Handle straight member access: CustomData["foo"]
                methodCall = node as MethodCallExpression;
            }

            if (methodCall == null)
            {
                return null; // Wasn't able to parse expression. Fail fast
            }

            var asMemberAccess = methodCall.Object as MemberExpression;
            bool isAccessingCustomDataProxy = asMemberAccess.Type == typeof(ICustomDataProxy);
            bool isAccessingIndexer = methodCall.Method == GetCustomDataProxyIndexer();

            string argument = (methodCall.Arguments[0] as ConstantExpression)?.Value?.ToString();
            bool isArgumentPresent = !string.IsNullOrEmpty(argument);

            if (!isAccessingCustomDataProxy
                || !isAccessingIndexer
                || !isArgumentPresent)
            {
                return null; // fail fast
            }

            var fieldName = $"customData.{argument}";

            return fieldName;
        }
    }
}
