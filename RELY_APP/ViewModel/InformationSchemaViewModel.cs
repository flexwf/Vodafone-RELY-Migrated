using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class InformationSchemaViewModel
    {
        public string DATA_TYPE { get; set; }
        public int CHARACTER_MAXIMUM_LENGTH { get; set; }
        
        public string IS_NULLABLE { get; set; }
        public string  COLUMN_DEFAULT { get; set; }
    }
}