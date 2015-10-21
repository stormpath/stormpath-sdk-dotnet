using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal class OrderByTerm
    {
        public string FieldName { get; set; }

        public OrderByDirection Direction { get; set; }
    }
}
