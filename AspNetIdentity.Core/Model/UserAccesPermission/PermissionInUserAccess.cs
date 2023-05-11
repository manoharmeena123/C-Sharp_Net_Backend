using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.UserAccesPermission
{
    public class PermissionInUserAccess
    {
        [Key]
        public Guid PermissionId { get; set; } = Guid.NewGuid();
        public Guid UserAccessRoleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public string SubModuleName { get; set; } = string.Empty;
        public string SubModuleCode { get; set; } = string.Empty;
        public bool IsAccess { get; set; } // View
        public bool Btn1 { get; set; } // Add 
        public bool Btn2 { get; set; } // Edit 
        public bool Btn3 { get; set; } // Delete
        public bool Btn4 { get; set; } // Import
        public bool Btn5 { get; set; } // Export
        public DateTimeOffset? UpdatedOn { get; set; } = null;
        public bool IsDeletd { get; set; }
    }
}