using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class RWorkflowViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string Name { get; set; }
        [RestrictSpecialChar]
        public string UILabel { get; set; }
        [RestrictSpecialChar]
        public string BaseTableName { get; set; }
        public bool CRAllowed { get; set; }
        [RestrictSpecialChar]
        public string CRWFName { get; set; }
        public string WFType { get; set; }

        public string Months { get; set; }
        public int Request { get; set; }
        public string CompanyCode { get; set; }

        public int LocalPobs { get; set; }
        public int Users { get; set; }
        public int RequestsPPM { get; set; }
        public int Products { get; set; }
        public int References { get; set; }

        public int AccountingScenario { get; set; }
    }
}