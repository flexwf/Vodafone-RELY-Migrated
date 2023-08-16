using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class DropDownViewModel
    {
        public int Id { get; set; }
        public string SysCat { get; set; }

    }

    public partial class SurveyDropDownViewModel
    {
        public int Id { get; set; }
        public string SurveyName { get; set; }


    }
    public partial class GlobalPobDropDownViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string IFRS15Account { get; set; }
        public string Label { get; set; }


    }

    public partial class CopaDimentionDropDownViewModel
    {
        public int Id { get; set; }
        public string CopaValue { get; set; }
        public string Description { get; set; }
        public string Dimension { get; set; }

        public string Label { get; set; }

    }


}