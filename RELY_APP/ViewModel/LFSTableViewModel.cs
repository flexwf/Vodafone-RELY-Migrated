using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSTableViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string TableCode { get; set; }
        public string TableTitle { get; set; }
        public int NoOfCols { get; set; }
        public int NoOfRows { get; set; }
    }
}