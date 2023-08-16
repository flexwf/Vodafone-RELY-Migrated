//using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class LEmailTemplateViewModel
    {
       
        public int Id { get; set; }


       
        
        public string TemplateName { get; set; }


     
        
        public string EmailSubject { get; set; }



       
        public string EmailBody { get; set; }


       
        public string Signature { get; set; }


      
        public string CompanyCode { get; set; }




    }

    public partial class GMenuViewModel
    {
         public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string MenuName { get; set; }

       public string MenuURL { get; set; }

       public int OrdinalPosition { get; set; }


    }
}
