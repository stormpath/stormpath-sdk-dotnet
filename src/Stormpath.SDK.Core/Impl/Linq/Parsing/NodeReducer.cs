using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal static class NodeReducer
    {
        /// <summary>
        /// Removes any Convert() calls wrapping a node.
        /// </summary>
        /// <remarks>VB.NET has a tendency to add tons of Convert() calls everywhere.</remarks>
        /// <param name="node">The node.</param>
        /// <returns>The unwrapped node.</returns>
        public static Expression Reduce(Expression node)
        {
            while (node.NodeType == ExpressionType.Convert)
            {
                node = (node as UnaryExpression).Operand;
            }

            return node;
        }
    }
}
