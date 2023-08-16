using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class UploadReferenceViewModel
    {
        public int Id { get; set; }
        public int ReferenceTypeId { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }
        public string Description { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public string WFComments { get; set; }
        public Nullable<int> WFRequesterId { get; set; }
        public Nullable<int> WFAnalystId { get; set; }
        public Nullable<int> WFManagerId { get; set; }
        public Nullable<int> WFCurrentOwnerId { get; set; }
        public Nullable<int> WFRequesterRoleId { get; set; }
        public int CreatedById { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public string Status { get; set; }

        public Nullable<DateTime> WFStatusDateTime { get; set; }
        public Nullable<DateTime> WFIsReadyDateTime { get; set; }
        public string ExtractFileName { get; set; }
        public int Version { get; set; }
        
        //Below variables are for ReferenceData, these columns are not in db but added for manipulating ref data for manual entry
        public string GridArray { get; set; }
        public int collength { get; set; }//cannot make it nullable as its being used as loop counter in a for-loop in api
    }
}