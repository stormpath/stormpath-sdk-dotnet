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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceWhereExpressionVisitor : ThrowingExpressionTreeVisitor
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

            if (this.inProgress.IsStringTermComplete())
            {
                this.GeneratedModels.Add(new StringAttributeTermModel(this.inProgress.Field, this.inProgress.StringValue, this.inProgress.StringMatchType.Value));
                this.inProgress.ResetStringTerm();
            }

            if (this.inProgress.IsDateTermComplete())
            {
                DatetimeAttributeTermModel model = null;
                if (this.inProgress.DateGreaterThan.Value)
                {
                    model = new DatetimeAttributeTermModel(
                        this.inProgress.Field,
                        start: this.inProgress.DateValue.Value,
                        startInclusive: this.inProgress.DateOrEqual.Value);
                }
                else
                {
                    model = new DatetimeAttributeTermModel(
                        this.inProgress.Field,
                        end: this.inProgress.DateValue.Value,
                        endInclusive: this.inProgress.DateOrEqual.Value);
                }

                this.GeneratedModels.Add(model);
                this.inProgress.ResetDateTerm();
            }

            if (this.inProgress.IsShorthandDateTermComplete())
            {
                this.GeneratedModels.Add(this.inProgress.ShorthandModel);
                this.inProgress.ResetShorthandDateTerm();
            }

            return visited;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            if (!this.IsSupportedBinaryComparisonOperator(expression.NodeType))
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
                this.inProgress.DateGreaterThan = expression.NodeType == ExpressionType.GreaterThan || expression.NodeType == ExpressionType.GreaterThanOrEqual;
                this.inProgress.DateOrEqual = expression.NodeType == ExpressionType.GreaterThanOrEqual || expression.NodeType == ExpressionType.LessThanOrEqual;
            }

            this.VisitExpression(expression.Left);
            this.VisitExpression(expression.Right);

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
            bool isWithinMethodCall = expression.Method.DeclaringType == typeof(WithinExpressionExtensions);
            if (isWithinMethodCall)
                return this.HandleWithinDateExtensionMethod(expression);

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

                this.inProgress.StringMatchType = matchingType;

                this.VisitMemberExpression((MemberExpression)expression.Object);
                this.VisitConstantExpression((ConstantExpression)expression.Arguments[0]);
                return expression;
            }

            return base.VisitMethodCallExpression(expression); // throws
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            var fieldName = string.Empty;

            if (DatetimeFieldNameTranslator.TryGetValue(expression.Member.Name, out fieldName))
            {
                this.inProgress.TargetType = typeof(DatetimeAttributeTermModel);
                this.inProgress.Field = fieldName;
                return expression; // done
            }

            if (FieldNameTranslator.TryGetValue(expression.Member.Name, out fieldName))
            {
                this.inProgress.TargetType = typeof(StringAttributeTermModel);
                this.inProgress.Field = fieldName;
                return expression; // done
            }

            throw new NotSupportedException($"The property {expression.Member.Name} is not currently supported in a Where clause.");
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Type == typeof(string))
            {
                this.inProgress.StringValue = expression.Value.ToString();
                return expression; // done
            }

            if (expression.Type == typeof(DateTimeOffset))
            {
                this.inProgress.DateValue = (DateTimeOffset)expression.Value;
                return expression; // done
            }

            if (expression.Type.IsSubclassOf(typeof(Enumeration)))
            {
                this.inProgress.StringValue = expression.Value.ToString();
                return expression; // done
            }

            throw new NotSupportedException($"The constant type {expression.Type.Name} is not currently supported in a Where clause.");
        }

        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private Expression HandleWithinDateExtensionMethod(MethodCallExpression methodCall)
        {
            var fieldName = string.Empty;
            var methodCallMember = methodCall.Arguments[0] as MemberExpression;
            bool validField = methodCallMember != null && DatetimeFieldNameTranslator.TryGetValue(methodCallMember.Member.Name, out fieldName);
            if (!validField)
                throw new NotSupportedException("Within must be used on a supported datetime field.");

            var numberOfConstantArgs = methodCall.Arguments.Count - 1;
            var yearArg = (int)(methodCall.Arguments[1] as ConstantExpression).Value;
            int? monthArg = null,
                dayArg = null,
                hourArg = null,
                minuteArg = null,
                secondArg = null;

            if (methodCall.Arguments.Count >= 3)
                monthArg = (methodCall.Arguments?[2] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 4)
                dayArg = (methodCall.Arguments?[3] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 5)
                hourArg = (methodCall.Arguments?[4] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 6)
                minuteArg = (methodCall.Arguments?[5] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count == 7)
                secondArg = (methodCall.Arguments?[6] as ConstantExpression)?.Value as int?;

            this.inProgress.ShorthandModel = new DatetimeShorthandAttributeTermModel(fieldName, yearArg, monthArg, dayArg, hourArg, minuteArg, secondArg);

            return methodCall;
        }

        private static string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}
