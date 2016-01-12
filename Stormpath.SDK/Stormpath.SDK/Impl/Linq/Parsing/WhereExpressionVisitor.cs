// <copyright file="WhereExpressionVisitor.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class WhereExpressionVisitor : ExpressionVisitor
    {
        public static List<ParsedExpression> GetParsedExpressions(Expression predicate)
        {
            var visitor = new WhereExpressionVisitor();
            visitor.Visit(predicate);
            return visitor.parsedExpressions;
        }

        private WhereExpressionVisitor()
        {
            this.parsedExpressions = new List<ParsedExpression>();
        }

        private List<ParsedExpression> parsedExpressions;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // .Where(x => true) doesn't mean anything to us.
            if (node.Body.NodeType == ExpressionType.Constant)
            {
                throw new NotSupportedException("This query type is not supported.");
            }

            return base.VisitLambda<T>(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var comparison = this.GetBinaryComparisonType(node.NodeType);

            if (!comparison.HasValue)
            {
                throw new NotSupportedException($"The comparison operator {node.NodeType} is not supported.");
            }

            if (comparison.Value == WhereComparison.AndAlso)
            {
                this.parsedExpressions.AddRange(GetParsedExpressions(node.Right));
                this.parsedExpressions.AddRange(GetParsedExpressions(node.Left));

                return node;
            }

            // .Where(x => x.Foo == 5)
            if (node.Left.NodeType == ExpressionType.MemberAccess &&
                node.Right.NodeType == ExpressionType.Constant)
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Left as MemberExpression).Member.Name,
                        (node.Right as ConstantExpression).Value,
                        comparison.Value));

                return node; // done
            }

            // .Where(x => 5 == x.Foo)
            if (node.Left.NodeType == ExpressionType.Constant &&
                node.Right.NodeType == ExpressionType.MemberAccess)
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Right as MemberExpression).Member.Name,
                        (node.Left as ConstantExpression).Value,
                        comparison.Value));

                return node; // done
            }

            throw new NotSupportedException("A Where expression must contain a method call, or member access and constant expressions.");
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Within")
            {
                return this.HandleWithinExtensionMethod(node);
            }

            bool correctOverload =
                node.Arguments.Count == 1 &&
                node.Arguments[0].NodeType == ExpressionType.Constant;
            if (!correctOverload)
            {
                throw new NotSupportedException($"The {node.Method.Name} with these overloads is not supported.");
            }

            if (node.Method.Name == "Equals")
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Object as MemberExpression).Member.Name,
                        (string)(node.Arguments[0] as ConstantExpression).Value,
                        WhereComparison.Equal));

                return node; // done
            }

            if (node.Method.Name == "StartsWith")
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Object as MemberExpression).Member.Name,
                        (string)(node.Arguments[0] as ConstantExpression).Value,
                        WhereComparison.StartsWith));

                return node; // done
            }

            if (node.Method.Name == "EndsWith")
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Object as MemberExpression).Member.Name,
                        (string)(node.Arguments[0] as ConstantExpression).Value,
                        WhereComparison.EndsWith));

                return node; // done
            }

            if (node.Method.Name == "Contains")
            {
                this.parsedExpressions.Add(
                    new WhereMemberExpression(
                        (node.Object as MemberExpression).Member.Name,
                        (string)(node.Arguments[0] as ConstantExpression).Value,
                        WhereComparison.Contains));

                return node; // done
            }

            throw new NotSupportedException($"The method {node.Method.Name} is not supported.");
        }

        private Expression HandleWithinExtensionMethod(MethodCallExpression node)
        {
            var methodCallMember = node.Arguments[0] as MemberExpression;
            var fieldName = methodCallMember.Member.Name;

            bool isDateTimeField = methodCallMember?.Type == typeof(DateTimeOffset);
            if (!isDateTimeField)
            {
                throw new NotSupportedException("Within must be used on a supported datetime field.");
            }

            var numberOfConstantArgs = node.Arguments.Count - 1;
            var yearArg = (int)(node.Arguments[1] as ConstantExpression).Value;
            int? monthArg = null,
                dayArg = null,
                hourArg = null,
                minuteArg = null,
                secondArg = null;

            if (node.Arguments.Count >= 3)
            {
                monthArg = (node.Arguments?[2] as ConstantExpression)?.Value as int?;
            }

            if (node.Arguments.Count >= 4)
            {
                dayArg = (node.Arguments?[3] as ConstantExpression)?.Value as int?;
            }

            if (node.Arguments.Count >= 5)
            {
                hourArg = (node.Arguments?[4] as ConstantExpression)?.Value as int?;
            }

            if (node.Arguments.Count >= 6)
            {
                minuteArg = (node.Arguments?[5] as ConstantExpression)?.Value as int?;
            }

            if (node.Arguments.Count == 7)
            {
                secondArg = (node.Arguments?[6] as ConstantExpression)?.Value as int?;
            }

            var shorthandModel = new DatetimeShorthandModel(fieldName, yearArg, monthArg, dayArg, hourArg, minuteArg, secondArg);
            this.parsedExpressions.Add(new WhereMemberExpression(fieldName, shorthandModel, WhereComparison.Equal));

            return node;
        }

        private static Dictionary<ExpressionType, WhereComparison> comparisonLookup
            = new Dictionary<ExpressionType, WhereComparison>()
            {
                [ExpressionType.Equal] = WhereComparison.Equal,
                [ExpressionType.AndAlso] = WhereComparison.AndAlso,
                [ExpressionType.GreaterThan] = WhereComparison.GreaterThan,
                [ExpressionType.GreaterThanOrEqual] = WhereComparison.GreaterThanOrEqual,
                [ExpressionType.LessThan] = WhereComparison.LessThan,
                [ExpressionType.LessThanOrEqual] = WhereComparison.LessThanOrEqual
            };

        private WhereComparison? GetBinaryComparisonType(ExpressionType type)
        {
            WhereComparison found;
            if (!comparisonLookup.TryGetValue(type, out found))
            {
                return null;
            }

            return found;
        }
    }
}