// <copyright file="VbCallTransformingVisitor.cs" company="Stormpath, Inc.">
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

using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class VbCallTransformingVisitor : ExpressionVisitor
    {
        private static readonly string VbStringCompareCallTypeName = "Microsoft.VisualBasic.CompilerServices.Operators";
        private static readonly string VbStringCompareCallName = "CompareString";

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var compareStringCall = GetStringCompareMethodCall(node);

            if (compareStringCall == null)
                return node;

            var arg1 = compareStringCall.Arguments[0];
            var arg2 = compareStringCall.Arguments[1];

            switch (node.NodeType)
            {
                case ExpressionType.LessThan:
                    return Expression.LessThan(arg1, arg2);
                case ExpressionType.LessThanOrEqual:
                    return Expression.GreaterThan(arg1, arg2);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(arg1, arg2);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(arg1, arg2);
                default:
                    return Expression.Equal(arg1, arg2);
            }
        }

        private static MethodCallExpression GetStringCompareMethodCall(BinaryExpression node)
        {
            var leftAsMethodCall = node.Left as MethodCallExpression;
            var rightAsMethodCall = node.Right as MethodCallExpression;

            bool isLeftCall =
                leftAsMethodCall != null
                    && node.Right.NodeType == ExpressionType.Constant
                    && leftAsMethodCall.Method.DeclaringType.FullName == VbStringCompareCallTypeName
                    && leftAsMethodCall.Method.Name == VbStringCompareCallName;

            bool isRightCall =
                rightAsMethodCall != null
                    && node.Left.NodeType == ExpressionType.Constant
                    && rightAsMethodCall.Method.DeclaringType.FullName == VbStringCompareCallTypeName
                    && rightAsMethodCall.Method.Name == VbStringCompareCallName;

            if (isLeftCall)
                return leftAsMethodCall;

            if (isRightCall)
                return rightAsMethodCall;

            return null;
        }
    }
}
