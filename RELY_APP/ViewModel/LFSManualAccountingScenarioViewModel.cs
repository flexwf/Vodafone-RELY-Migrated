using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class LFSManualAccountingScenarioViewModel
    {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public string EntityType { get; set; }
            public string Reference { get; set; }
            public string Situation { get; set; }
            public string ObjectType { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
        //column for grid
        public string Product { get; set; }
    }
}