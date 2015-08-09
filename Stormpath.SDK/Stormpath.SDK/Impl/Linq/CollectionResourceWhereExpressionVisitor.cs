// <copyright file="CollectionResourceWhereExpressionVisitor.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Impl.Linq.StaticNameTranslators;

namespace Stormpath.SDK.Impl.Linq
{
    internal class CollectionResourceWhereExpressionVisitor : ThrowingExpressionTreeVisitor
    {
        private WhereAttributeTermInProgressModel inProgress = new WhereAttributeTermInProgressModel();

        public List<AbstractAttributeTermModel> GeneratedModels { get; } = new List<AbstractAttributeTermModel>();

        public static List<AbstractAttributeTermModel> GenerateModels(Expression expression)
        {
            var visitor = new CollectionResourceWhereExpressionVisitor();
            visitor.VisitExpression(expression);
            return visitor.GeneratedModels;
        }

        public override Expression VisitExpression(Expression expression)
        {
            var visited = base.VisitExpression(expression);

            if (inProgress.IsStringTermComplete())
            {
                this.GeneratedModels.Add(new StringAttributeTermModel(inProgress.Field, inProgress.StringValue, inProgress.StringMatchType.Value));
            }

            if (inProgress.IsDateTermComplete())
            {
                DatetimeAttributeTermModel model = null;
                if (inProgress.DateGreaterThan.Value)
                {
                    model = new DatetimeAttributeTermModel(
                        inProgress.Field,
                        start: inProgress.DateValue.Value,
                        startInclusive: inProgress.DateOrEqual.Value);
                }
                else
                {
                    model = new DatetimeAttributeTermModel(
                        inProgress.Field,
                        end: inProgress.DateValue.Value,
                        endInclusive: inProgress.DateOrEqual.Value);
                }

                this.GeneratedModels.Add(model);
            }

            return visited;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            if (!IsSupportedBinaryComparisonOperator(expression.NodeType))
                throw new NotSupportedException("One or more comparison operators are currently unsupported.");

            if (expression.NodeType == ExpressionType.AndAlso)
            {
                this.GeneratedModels.AddRange(GenerateModels(expression.Left));
                this.GeneratedModels.AddRange(GenerateModels(expression.Right));
                return expression; // done
            }

            bool isInequalityComparison =
                expression.NodeType == ExpressionType.GreaterThan ||
                expression.NodeType == ExpressionType.GreaterThanOrEqual ||
                expression.NodeType == ExpressionType.LessThan ||
                expression.NodeType == ExpressionType.LessThanOrEqual;
            if (isInequalityComparison)
            {
                // Only supported on dates right now
                inProgress.DateGreaterThan = expression.NodeType == ExpressionType.GreaterThan || expression.NodeType == ExpressionType.GreaterThanOrEqual;
                inProgress.DateOrEqual = expression.NodeType == ExpressionType.GreaterThanOrEqual || expression.NodeType == ExpressionType.LessThanOrEqual;
            }

            VisitExpression(expression.Left);
            VisitExpression(expression.Right);

            return expression;
        }

        private bool IsSupportedBinaryComparisonOperator(ExpressionType type)
        {
            return
                type == ExpressionType.And ||
                type == ExpressionType.AndAlso ||
                type == ExpressionType.GreaterThan ||
                type == ExpressionType.GreaterThanOrEqual ||
                type == ExpressionType.LessThan ||
                type == ExpressionType.LessThanOrEqual ||
                type == ExpressionType.Equal;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            StringAttributeMatchingType matchingType;
            bool isAChainedHelperMethodCall = StringHelperMethodNameTranslator.TryGetValue(expression.Method.Name, out matchingType);
            if (isAChainedHelperMethodCall)
            {
                if (expression.Object.NodeType != ExpressionType.MemberAccess)
                    throw new NotSupportedException("Chained method calls can only be used on attribute properties in the Where clause.");

                if (expression.Arguments[0].NodeType != ExpressionType.Constant)
                    throw new NotSupportedException("Only constant values may be used in a method call inside the Where clause.");

                if (expression.Arguments.Count > 1)
                    throw new NotSupportedException("Only simple overloads of helper methods may be used inside the Where clause.");

                inProgress.StringMatchType = matchingType;

                VisitMemberExpression((MemberExpression)expression.Object);
                VisitConstantExpression((ConstantExpression)expression.Arguments[0]);
                return expression;
            }

            return base.VisitMethodCallExpression(expression); // throws
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            var fieldName = string.Empty;

            if (DatetimeFieldNameTranslator.TryGetValue(expression.Member.Name, out fieldName))
            {
                inProgress.TargetType = typeof(DatetimeAttributeTermModel);
                inProgress.Field = fieldName;
                return expression; // done
            }

            if (FieldNameTranslator.TryGetValue(expression.Member.Name, out fieldName))
            {
                inProgress.TargetType = typeof(StringAttributeTermModel);
                inProgress.Field = fieldName;
                return expression; // done
            }

            throw new NotSupportedException($"The property {expression.Member.Name} is not currently supported in a Where clause.");
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Type == typeof(string))
            {
                inProgress.StringValue = expression.Value.ToString();
                return expression; // done
            }

            if (expression.Type == typeof(DateTimeOffset))
            {
                inProgress.DateValue = (DateTimeOffset)expression.Value;
                return expression; // done
            }

            throw new NotSupportedException($"The constant type {expression.Type.Name} is not currently supported in a Where clause.");
        }

        // TODO
        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private static string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}
