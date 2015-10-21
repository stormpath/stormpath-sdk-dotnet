using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    public enum WhereComparison
    {
        Equal,
        StartsWith,
        EndsWith,
        Contains,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        AndAlso
    }
}
