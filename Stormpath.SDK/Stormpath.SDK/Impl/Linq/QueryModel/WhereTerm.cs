using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal class WhereTerm
    {
        public string FieldName { get; set; }

        public object Value { get; set; }

        public Type Type { get; set; }

        public WhereComparison Comparison { get; set; }
    }
}
