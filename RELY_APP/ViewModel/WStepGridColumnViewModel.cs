
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class WStepGridColumnViewModel
    {
        public int Id { get; set; }
        public int WStepId { get; set; }
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public bool ShouldBeVisible { get; set; }
        public Nullable<int> OrderByOrdinal { get; set; }
        public string AscDesc { get; set; }
        public int Ordinal { get; set; }
        public string FunctionName { get; set; }
        public string JoinTable { get; set; }
        public string JoinTableColumn { get; set; }
        public string BaseTableJoinColumn { get; set; }
    }
}