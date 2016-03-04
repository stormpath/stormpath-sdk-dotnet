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
using System.Reflection;
using System.Linq;
using Stormpath.SDK.CustomData;
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
            var comparison = GetBinaryComparisonType(node.NodeType);

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

            // Handle .Where(x => x.Foo == 5)
            //     or .Where(x => 5 == x.Foo)
            var asMemberAccess = GetBinaryAsConstantAnd<MemberExpression>(node);
            if (asMemberAccess != null)
            {
                this.parsedExpressions.Add(new WhereMemberExpression(
                    asMemberAccess.Item2.Member.Name,
                    asMemberAccess.Item1.Value,
                    comparison.Value));

                return node; // done
            }

            // Handle .Where(x => x.CustomData["foo"] == "bar")
            // or     .Where(x => "bar" == x.CustomData["foo"])
            var asCustomDataExpression = GetBinaryAsConstantAnd<Expression>(node);
            if (asCustomDataExpression != null)
            {
                var fieldName = CustomDataParsingHelper.GetFieldName(asCustomDataExpression.Item2);

                if (!string.IsNullOrEmpty(fieldName))
                {
                    this.parsedExpressions.Add(new WhereMemberExpression(
                        fieldName,
                        asCustomDataExpression.Item1.Value,
                        comparison.Value));

                    return node; // done
                }
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
                throw new NotSupportedException($"The '{node.Method.Name}' method with these overloads is not supported.");
            }

            WhereComparison? comparison = null;
            switch (node.Method.Name)
            {
                case "Equals":
                    comparison = WhereComparison.Equal;
                    break;
                case "StartsWith":
                    comparison = WhereComparison.StartsWith;
                    break;
                case "EndsWith":
                    comparison = WhereComparison.EndsWith;
                    break;
                case "Contains":
                    comparison = WhereComparison.Contains;
                    break;
            }

            if (comparison == null)
            {
                throw new NotSupportedException($"The method '{node.Method.Name}' is not supported.");
            }

            var value = (node.Arguments[0] as ConstantExpression).Value;
            var fieldName = string.Empty;

            var targetAsMember = node.Object as MemberExpression;
            if (targetAsMember != null)
            {
                fieldName = targetAsMember.Member.Name;
            }
            else
            {
                fieldName = CustomDataParsingHelper.GetFieldName(node.Object);
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new NotSupportedException($"The target of method '{node.Method.Name}' could not be parsed.");
            }

            this.parsedExpressions.Add(
                new WhereMemberExpression(fieldName, value, comparison.Value));

            return node; // done
        }

        private Expression HandleWithinExtensionMethod(MethodCallExpression node)
        {
            var fieldName = string.Empty;

            var targetAsMember = node.Arguments[0] as MemberExpression;
            if (targetAsMember != null)
            {
                bool isDateTimeField = targetAsMember.Type == typeof(DateTimeOffset);
                if (!isDateTimeField)
                {
                    throw new NotSupportedException("Within must be used on a supported datetime field.");
                }

                fieldName = targetAsMember.Member.Name;
            }
            else
            {
                fieldName = CustomDataParsingHelper.GetFieldName(node.Arguments[0]);
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new NotSupportedException($"The target of method '{node.Method.Name}' could not be parsed.");
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

        /// <summary>
        /// This method will get the operands of a binary expression (a Constant and another expression type)
        /// regardless of order (expr == constant and constant == expr are both valid).
        /// </summary>
        /// <typeparam name="TOther">The other expression type.</typeparam>
        /// <param name="binaryNode">The binary node.</param>
        /// <returns>The constant expression and other expression.</returns>
        private static Tuple<ConstantExpression, TOther> GetBinaryAsConstantAnd<TOther>(BinaryExpression binaryNode)
            where TOther: Expression
        {
            ConstantExpression constant = null;
            TOther other = null;

            if (binaryNode.Left.NodeType == ExpressionType.Constant)
            {
                constant = binaryNode.Left as ConstantExpression;
                other = binaryNode.Right as TOther;
            }
            else if (binaryNode.Right.NodeType == ExpressionType.Constant)
            {
                constant = binaryNode.Right as ConstantExpression;
                other = binaryNode.Left as TOther;
            }

            return (constant == null || other == null)
                ? null
                : new Tuple<ConstantExpression, TOther>(constant, other);
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

        private static WhereComparison? GetBinaryComparisonType(ExpressionType type)
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