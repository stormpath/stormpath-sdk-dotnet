using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions
{
    internal class WhereMemberExpression : ParsedExpression
    {
        public WhereMemberExpression(string field, object value, WhereComparison comparison)
        {
            this.FieldName = field;
            this.Value = value;
            this.Comparison = comparison;
        }

        public string FieldName { get; private set; }

        public object Value { get; private set; }

        public WhereComparison Comparison { get; private set; }

        protected internal override Expression Accept(CompilingExpressionVisitor visitor)
        {
            return visitor.VisitWhereMember(this);
        }
    }
}
