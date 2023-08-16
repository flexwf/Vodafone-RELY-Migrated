using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSTableResponseViewModel
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public string TableCode { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public string Response { get; set; }
    }
}