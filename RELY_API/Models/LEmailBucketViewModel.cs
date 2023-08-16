using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{

    //created by RS to show Email Bucket detail tab columns on L2Admin Page
    public class LEmailBucketViewModel
    {
        public string CompanyCode { get; set; }
        public string Body { get; set; }
        public string RecipientList { get; set; }
        public string Subject { get; set; }
        public string EmailType { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int CreatedById { get; set; }
        public string Status { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public int EmailSent { get; set; }
    }
}