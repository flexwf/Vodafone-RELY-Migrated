using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LocalPobForProductViewModel
    {

        public int Id { get; set; }
        public int LocalPobTypeId { get; set; }
        public string LLocalPOBType { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public string WFStatus {get;set;}
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public int PobCatalogueId { get; set; }
    }
}