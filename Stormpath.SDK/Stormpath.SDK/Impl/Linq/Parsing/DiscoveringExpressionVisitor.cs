using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class DiscoveringExpressionVisitor : ExpressionVisitor
    {
        private readonly List<Expression> expressions;

        public ReadOnlyCollection<Expression> Expressions
            => new ReadOnlyCollection<Expression>(this.expressions);

        public DiscoveringExpressionVisitor()
        {
            this.expressions = new List<Expression>();
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Query operators
            if (this.HandleWhereMethod(node) ||
                this.HandleOrderByMethod(node) ||
                this.HandleThenByMethod(node) ||
                this.HandleTakeMethod(node) ||
                this.HandleSkipMethod(node))
            {
                this.Visit(node.Arguments[0]);
                return node;
            }

            // Result operators
            if (this.HandleAnyResultOperator(node) ||
                this.HandleCountResultOperator(node) ||
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
                return false;

            // TODO
            return true;

            //var whereExpressions = WhereExpressionVisitor.GetParsedExpressions(node.Arguments[1]);
            //this.expressions.AddRange(whereExpressions);

            //return true;
        }

        private bool HandleOrderByMethod(MethodCallExpression node)
        {
            OrderByDirection? direction = null;

            if (node.Method.Name == "OrderBy")
                direction = OrderByDirection.Ascending;
            else if (node.Method.Name == "OrderByDescending")
                direction = OrderByDirection.Descending;
            else
                return false;

            var field = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
            if (field == null)
                throw new NotSupportedException($"{node.Method.Name} must operate on a supported field.");

            this.expressions.Add(new OrderByExpression(field.Member.Name, direction.Value));

            return true;
        }

        private bool HandleThenByMethod(MethodCallExpression node)
        {
            OrderByDirection? direction = null;

            if (node.Method.Name == "ThenBy")
                direction = OrderByDirection.Ascending;
            else if (node.Method.Name == "ThenByDescending")
                direction = OrderByDirection.Descending;
            else
                return false;

            var field = ((node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
            if (field == null)
                throw new NotSupportedException($"{node.Method.Name} must operate on a supported field.");

            this.expressions.Add(new OrderByExpression(field.Member.Name, direction.Value));

            return true;
        }

        private bool HandleTakeMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Take")
                return false;

            var value = node.Arguments[1] as ConstantExpression;
            if (value == null)
                throw new ArgumentException("Value passed to Take operator is not supported.");

            this.expressions.Add(new TakeExpression((int)value.Value));

            return true;
        }

        private bool HandleSkipMethod(MethodCallExpression node)
        {
            if (node.Method.Name != "Skip")
                return false;

            var value = node.Arguments[1] as ConstantExpression;
            if (value == null)
                throw new ArgumentException("Value passed to Take operator is not supported.");

            this.expressions.Add(new SkipExpression((int)value.Value));

            return true;
        }

        private bool HandleAnyResultOperator(MethodCallExpression node)
        {
            if (node.Method.Name != "Any")
                return false;

            this.expressions.Add(new AnyResultOperator());

            return true;
        }

        private bool HandleCountResultOperator(MethodCallExpression node)
        {
            if (node.Method.Name != "Count")
                return false;

            this.expressions.Add(new CountResultOperator());

            return true;
        }

        private bool HandleFirstResultOperator(MethodCallExpression node)
        {
            bool? defaultIfEmpty = null;

            if (node.Method.Name == "First")
                defaultIfEmpty = false;
            else if (node.Method.Name == "FirstOrDefault")
                defaultIfEmpty = true;
            else
                return false;

            this.expressions.Add(new FirstResultOperator(defaultIfEmpty.Value));

            return true;
        }

        private bool HandleSingleResultOperator(MethodCallExpression node)
        {
            bool? defaultIfEmpty = null;

            if (node.Method.Name == "Single")
                defaultIfEmpty = false;
            else if (node.Method.Name == "SingleOrDefault")
                defaultIfEmpty = true;
            else
                return false;

            this.expressions.Add(new SingleResultOperator(defaultIfEmpty.Value));

            return true;
        }
    }
}
