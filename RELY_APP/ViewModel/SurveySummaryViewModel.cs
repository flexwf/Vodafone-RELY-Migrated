using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class SurveySummaryViewModel
    {

        public string ChapterName { get; set; }
        public int ChapterId { get; set; }
        public int ChapterOrdinal { get; set; }
        public int SectionId { get; set; }
        public int SectionOrdinal { get; set; }
        public string SectionName { get; set; }
        public int DefaultCount { get; set; }
        public int MissingCount { get; set; }


    }
}