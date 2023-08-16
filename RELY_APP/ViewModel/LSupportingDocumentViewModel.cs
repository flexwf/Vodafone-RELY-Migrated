using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class LSupportingDocumentViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string FileName { get; set; }
        [RestrictSpecialChar]
        public string OriginalFileName { get; set; }
        [RestrictSpecialChar]
        public string Description { get; set; }
        public string FilePath { get; set; }
        [RestrictSpecialChar]
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public int CreatedById { get; set; }
        public int CreatedByRoleId { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        //SS added extra columns to display in grid
        public string LoginEmail { get; set; }
        public string Label { get; set; }
    }
}