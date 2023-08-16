using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class GenericGridColumnsViewModel
    {
        public int WStepId { get; set; }
        public string DataType { get; set; }
        public string BaseTableName { get; set; }
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public bool ShouldBeVisible { get; set; }
        public Nullable<int> OrderByOrdinal { get; set; }
        public string AscDesc { get; set; }
        public Nullable<int> Ordinal { get; set; }
        public string FunctionName { get; set; }
        public string JoinTable { get; set; }
        public string JoinTableColumn { get; set; }
        public string BaseTableJoinColumn { get; set; }
        //Extra column to resolve the joining of tables having same column issue
        public string CompleteColumnName { get; set; }
    }

    public class OtherAPIData
    {
        public string TransactionID { get; set; }
    }
}