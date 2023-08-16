using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LASLifecycleEventViewModel
    {

        public int Id { get; set; }
        public int AccountingScenarioId { get; set; }
        public int Ordinal { get; set; }
        public int NatureId { get; set; }
        public string NatureValue { get; set; }
        public int EventId { get; set; }
        public string EventValue { get; set; }
        public string Notes { get; set; }

    }
}