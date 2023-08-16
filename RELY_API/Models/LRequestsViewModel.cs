using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class LRequestsViewModel
    {
        public int Id { get; set; }
        public int Version { get; set; }

        public string Name { get; set; }
    }

    partial class GetRequestDataViewModel
    {
        public string Name { get; set; }
        public string AuthorizationNumber { get; set; }
        public string SystemName { get; set; }

    }
        
}