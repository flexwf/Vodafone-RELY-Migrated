using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class LProductRequestHistoryViewModel
    {
        public int Id { get; set; }
        public int SysCatId { get; set; }
        public int RequestId { get; set; }
        public string Name { get; set; }
        public string AuthorizationNumber { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public int Version { get; set; }

    }
}