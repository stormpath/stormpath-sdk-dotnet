using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;

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
                throw new NotSupportedException("This query type is not supported.");

            return base.VisitLambda<T>(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var comparison = this.GetBinaryComparisonType(node.NodeType);

            if (!comparison.HasValue)
                throw new NotSupportedException($"The comparison operator {node.NodeType} is not supported.");

            if (comparison.Value == WhereComparison.AndAlso)
            {
                this.parsedExpressions.AddRange(GetParsedExpressions(node.Right));
                this.parsedExpressions.AddRange(GetParsedExpressions(node.Left));
                return node;
            }

            //if (node.Left.NodeType == ExpressionType.MemberAccess &&
            //    node.Right.NodeType == ExpressionType.MemberAccess)
            //{
            //    if ((node.Right as MemberExpression).Expression.NodeType == ExpressionType.Constant)
            //    {
            //        LambdaExpression lambda = Expression.Lambda(node.Right);
            //        Delegate fn = lambda.Compile();
            //        var result = Expression.Constant(fn.DynamicInvoke(null), node.Right.Type);
            //    }
            //}

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
            bool correctOverload =
                node.Arguments.Count == 1 &&
                node.Arguments[0].NodeType == ExpressionType.Constant;
            if (!correctOverload)
                throw new NotSupportedException($"The method {node.Method.Name} is not supported.");

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
                return null;

            return found;
        }
    }
}