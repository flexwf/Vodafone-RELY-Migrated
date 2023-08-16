using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class ReadAndValidateReferenceDataViewModel
    {
        public bool HideButton { get; set; }
        public string ValidData { get; set; }
        public string ErrorData { get; set; }
        public string ErrorMessage { get; set; }
        public string PopUpErrorMessage { get; set; }
    }



    public partial class ReadAndValidateViewModel
    {
        public bool HideButton { get; set; }
        public DataTable ValidData { get; set; }
        public DataTable ErrorData { get; set; }
        public string ErrorMessage { get; set; }
        public string PopUpErrorMessage { get; set; }
    }

    public partial class BulkDataValidationViewModel
    {
        public string PopUpErrorMessage { get; set; }
        public string PopUpSuccessMessage { get; set; }
        public bool HideSaveButton { get; set; }
        //public List<dynamic> ErrorData { get; set; }
        public int ErrorRowsCount { get; set; }
    }

}