using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.QueryModel
{
    internal sealed class DatetimeShorthandModel
    {
        public DatetimeShorthandModel(
            string field,
            int year,
            int? month = null,
            int? day = null,
            int? hour = null,
            int? minute = null,
            int? second = null)
        {
            this.FieldName = field;

            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
        }

        public string FieldName { get; private set; }

        public int Year { get; private set; }

        public int? Month { get; private set; }

        public int? Day { get; private set; }

        public int? Hour { get; private set; }

        public int? Minute { get; private set; }

        public int? Second { get; private set; }
    }
}
