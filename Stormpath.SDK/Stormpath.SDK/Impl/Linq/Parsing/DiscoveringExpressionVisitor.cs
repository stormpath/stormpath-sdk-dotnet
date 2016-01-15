// <copyright file="DiscoveringExpressionVisitor.cs" company="Stormpath, Inc.">
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
// <copyright file="DiscoveringExpressionVisitor.cs" company="Stormpath, Inc.">
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class DiscoveringExpressionVisitor : ExpressionVisitor
    {
        private readonly List<Expression> expressions;
        private readonly Stack<Expression> orderByExpressions;
        private readonly Stack<Expression> whereExpressions;
        private readonly Stack<Expression> expandExpressions;

        public DiscoveringExpressionVisitor()
        {
            this.expressions = new List<Expression>();
            this.orderByExpressions = new Stack<Expression>();
            this.whereExpressions = new Stack<Expression>();
            this.expandExpressions = new Stack<Expression>();
        }

        public ReadOnlyCollection<Expression> Expressions
        {
            get
            {
                return new ReadOnlyCollection<Expression>(
                    this.expressions
                        .Concat(this.orderByExpressions)
                        .Concat(this.whereExpressions)
                        .Concat(this.expandExpressions)
                        .ToList());
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Query operators
            if (this.HandleWhereMethod(node) ||
                this.HandleOrderByMethod(node) ||
                this.HandleThenByMethod(node) ||
                this.HandleTakeMethod(node) ||
                this.HandleSkipMethod(node) ||
                this.HandleFilterExtensionMethod(node) ||
                this.HandleExpandExtensionMethod(node))
            {
                this.Visit(node.Arguments[0]);
                return node;
            }

            // Result operators
            if (this.HandleAnyResultOperator(node) ||
                this.HandleCountResultOperator(node) ||
                this.HandleLongCountResultOperator(node) ||
                this.HandleFirstResultOperator(node) ||
                this.HandleSingleResultOperator(node))
            {
                this.Visit(node.Arguments[0]);
                return node;
            }

            throw new NotSupportedException($"The '{node.Method.Name}' method is unsupported.");
        }

        private bool HandleWhereMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Where")
            {
                return false;
            }

            var whereExpressions = WhereExpressionVisitor.GetParsedExpressions(node.Arguments[1]);
            whereExpressions.ForEach(e => this.whereExpressions.Push(e));

            return true;
        }

        private bool HandleOrderByMethod(MethodCallExpression node)
        {
            OrderByDirection? direction = null;

            if (node.Method.Name == "OrderBy")
            {
                direction = OrderByDirection.Ascending;
            }
            else if (node.Method.Name == "OrderByDescending")
            {
                direction = OrderByDirection.Descending;
            }
            else
            {
                return false;
            }

            if (node.Arguments.Count > 2)
            {
                throw new NotSupportedException("This overload of OrderBy is not supported.");
            }

            var field = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
            if (field == null)
            {
                throw new NotSupportedException($"{node.Method.Name} must operate on a supported field.");
            }

            this.orderByExpressions.Push(new OrderByExpression(field.Member.Name, direction.Value));

            return true;
        }

        private bool HandleThenByMethod(MethodCallExpression node)
        {
            OrderByDirection? direction = null;

            if (node.Method.Name == "ThenBy")
            {
                direction = OrderByDirection.Ascending;
            }
            else if (node.Method.Name == "ThenByDescending")
            {
                direction = OrderByDirection.Descending;
            }
            else
            {
                return false;
            }

            if (node.Arguments.Count > 2)
            {
                throw new NotSupportedException("This overload of ThenBy is not supported.");
            }

            var field = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
            if (field == null)
            {
                throw new NotSupportedException($"{node.Method.Name} must operate on a supported field.");
            }

            this.orderByExpressions.Push(new OrderByExpression(field.Member.Name, direction.Value));

            return true;
        }

        private bool HandleTakeMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Take")
            {
                return false;
            }

            var value = node.Arguments[1] as ConstantExpression;
            if (value == null)
            {
                throw new ArgumentException("Value passed to Take operator is not supported.");
            }

            this.expressions.Add(new TakeExpression((int)value.Value));

            return true;
        }

        private bool HandleSkipMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Skip")
            {
                return false;
            }

            var value = node.Arguments[1] as ConstantExpression;
            if (value == null)
            {
                throw new ArgumentException("Value passed to Take operator is not supported.");
            }

            this.expressions.Add(new SkipExpression((int)value.Value));

            return true;
        }

        private bool HandleFilterExtensionMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Filter")
            {
                return false;
            }

            var value = (node.Arguments[1] as ConstantExpression)?.Value as string;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value passed to Filter operator is not supported.");
            }

            this.expressions.Add(new FilterExpression(value));

            return true;
        }

        private bool HandleExpandExtensionMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Expand")
            {
                return false;
            }

            var methodCallExpression =
                ((node.Arguments[1] as UnaryExpression)
                    ?.Operand as LambdaExpression)
                        ?.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException("Method selector passed to Expand operator could not be parsed.");
            }

            var targetMethod = methodCallExpression.Method;
            if (targetMethod == null)
            {
                throw new ArgumentException("Method selector passed to Expand operator is not supported.");
            }

            int? offset = null, limit = null;
            if (methodCallExpression.Arguments.Any())
            {
                offset = (methodCallExpression.Arguments[0] as ConstantExpression)?.Value as int?;
                limit = (methodCallExpression.Arguments[1] as ConstantExpression)?.Value as int?;
            }

            this.expandExpressions.Push(new ExpandExpression(targetMethod.Name, offset, limit));

            return true;
        }

        private bool HandleAnyResultOperator(MethodCallExpression node)
        {
            if (node.Method.Name != "Any")
            {
                return false;
            }

            this.expressions.Add(new AnyResultOperator());

            return true;
        }

        private bool HandleCountResultOperator(MethodCallExpression node)
        {
            if (node.Method.Name != "Count")
            {
                return false;
            }

            this.expressions.Add(new CountResultOperator());

            return true;
        }

        private bool HandleLongCountResultOperator(MethodCallExpression node)
        {
            if (node.Method.Name != "LongCount")
            {
                return false;
            }

            this.expressions.Add(new LongCountResultOperator());

            return true;
        }

        private bool HandleFirstResultOperator(MethodCallExpression node)
        {
            bool? defaultIfEmpty = null;

            if (node.Method.Name == "First")
            {
                defaultIfEmpty = false;
            }
            else if (node.Method.Name == "FirstOrDefault")
            {
                defaultIfEmpty = true;
            }
            else
            {
                return false;
            }

            this.expressions.Add(new FirstResultOperator(defaultIfEmpty.Value));

            return true;
        }

        private bool HandleSingleResultOperator(MethodCallExpression node)
        {
            bool? defaultIfEmpty = null;

            if (node.Method.Name == "Single")
            {
                defaultIfEmpty = false;
            }
            else if (node.Method.Name == "SingleOrDefault")
            {
                defaultIfEmpty = true;
            }
            else
            {
                return false;
            }

            this.expressions.Add(new SingleResultOperator(defaultIfEmpty.Value));

            return true;
        }
    }
}
