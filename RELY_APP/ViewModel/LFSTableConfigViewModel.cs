using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSTableConfigViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string TableCode { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        //public bool IsTotal { get; set; }
        public int? ItemTypeId { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionText { get; set; }
        public string QuestionName { get; set; }
        public string ItemTypeName { get; set; }
        public string ShowResponseFromQuestionCode { get; set; }
    }
}