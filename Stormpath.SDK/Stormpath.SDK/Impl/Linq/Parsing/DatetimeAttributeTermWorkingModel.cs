using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class DatetimeAttributeTermWorkingModel
    {
        public string FieldName { get; set; }

        public DateTimeOffset? Start { get; set; }

        public bool? StartInclusive { get; set; }

        public DateTimeOffset? End { get; set; }

        public bool? EndInclusive { get; set; }
    }
}
