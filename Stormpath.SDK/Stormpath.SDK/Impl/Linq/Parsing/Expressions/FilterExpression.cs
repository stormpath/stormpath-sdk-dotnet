using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions
{
    internal class FilterExpression : ParsedExpression
    {
        public FilterExpression(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        protected internal override Expression Accept(CompilingExpressionVisitor visitor)
        {
            return visitor.VisitFilter(this);
        }
    }
}
