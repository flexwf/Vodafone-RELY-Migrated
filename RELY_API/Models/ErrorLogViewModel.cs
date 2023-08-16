using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class ErrorLogViewModel
    {
        public System.DateTime ErrorDateTime { get; set; }
        public string SourceProject { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string StackTrace { get; set; }
        public string UserName { get; set; }
        public string ErrorType { get; set; }
        public string ErrorDescription { get; set; }
        public string Resolution { get; set; }
        public string ErrorOwner { get; set; }
        public string FieldName { get; set; }
        public Nullable<int> BatchNumber { get; set; }
        public string Status { get; set; }
        public int Exceptions { get; set; }
    }
}