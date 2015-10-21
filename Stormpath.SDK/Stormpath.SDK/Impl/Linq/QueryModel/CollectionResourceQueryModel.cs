using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal sealed class CollectionResourceQueryModel
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public string FilterTerm { get; set; }

        public List<WhereTerm> WhereTerms { get; set; }
            = new List<WhereTerm>();

        public List<OrderByTerm> OrderByTerms { get; set; }
            = new List<OrderByTerm>();

        public ExecutionPlanModel ExecutionPlan { get; set; }
            = new ExecutionPlanModel();

        public ResultOperator? ResultOperator { get; set; }

        public bool ResultDefaultIfEmpty { get; set; } = false;

        public static CollectionResourceQueryModel Default = new CollectionResourceQueryModel()
        {
            Offset = null,
            Limit = null,
            FilterTerm = null,
            ExecutionPlan = new ExecutionPlanModel()
            {
                MaxItems = null
            },
            ResultOperator = null,
            ResultDefaultIfEmpty = false
        };
    }
}
