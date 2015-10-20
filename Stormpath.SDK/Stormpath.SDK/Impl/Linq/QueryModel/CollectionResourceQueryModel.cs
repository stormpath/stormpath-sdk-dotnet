using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal sealed class CollectionResourceQueryModel
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public string FilterTerm { get; set; }

        public ExecutionPlanModel ExecutionPlan { get; private set; }

        public static CollectionResourceQueryModel Default = new CollectionResourceQueryModel()
        {
            Offset = null,
            Limit = null,
            FilterTerm = null,
            ExecutionPlan = new ExecutionPlanModel()
            {
                MaxItems = null
            }
        };
    }
}
