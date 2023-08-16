using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class LocalPobForProductViewModel
    {
        public int Id { get; set; }
        public int LocalPobTypeId { get; set; }
        public string LLocalPOBType { get; set; }
        public string Source { get; set; }
        public int Version { get; set; }
        public string Status { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        //public Nullable<int> LocalPobId { get; set; }
        //public Nullable<int> GlobalPobId1 { get; set; }
        //public Nullable<int> GlobalPobId2 { get; set; }
        //public Nullable<int> CopaId1 { get; set; }
        //public Nullable<int> CopaId2 { get; set; }
        //public Nullable<int> CopaId3 { get; set; }
        
        //public string POB1 { get; set; }
        //public string Type { get; set; }
        //public string Retention { get; set; }
        //public string Type1 { get; set; }
        //public string COPA2 { get; set; }
        //public string COPA5 { get; set; }
        //public string COPA22 { get; set; }
        //public string COPA52 { get; set; }
        public string WFStatus { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public int PobCatalogueId { get; set; }
        //public int LocalPobId { get; set; }
    }


    public class LocalPobForDropDown
    {
        public int Id { get; set; }
        public int Version { get; set; }

        public string Name { get; set; }
    }
}