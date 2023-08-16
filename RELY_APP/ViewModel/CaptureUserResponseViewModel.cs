using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class CaptureUserResponseViewModel
    {
        //Section to send data to partial view
        public int SurveyId { get; set; }
        public string ChapterCode { get; set; }
        public string SectionCode { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
    }

    public partial class UserResponseForGridViewModel
    {
        //Section to Get Data to Grid
        public int Id { get; set; }
        public int Ordinal { get; set; }
        public string ItemText { get; set; }
        public string Name { get; set; }
        public bool IsQuestion { get; set; }
    }
}