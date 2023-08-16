using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class UpdateMappingAuthorizableObjectRoleViewModel
    {
        public int AuthorizableId { get; set; }

        public string ColumnName { get; set; }

        public bool NewResponse { get; set; }

        public bool OldResponse { get; set; }


    }

    public partial class UpdateMappingMenuRoleViewModel
    {
        public int MenuId { get; set; }

        public string ColumnName { get; set; }

        public bool NewResponse { get; set; }

        public bool OldResponse { get; set; }


    }

}