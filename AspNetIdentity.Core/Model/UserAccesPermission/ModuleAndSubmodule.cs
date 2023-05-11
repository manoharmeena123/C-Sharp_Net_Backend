using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.UserAccesPermission
{
    public class ModuleAndSubmodule
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ModuleName { get; set; } = String.Empty;
        public string ModuleCode { get; set; } = String.Empty;
        public string ModulePathURL { get; set; } = String.Empty;
        public string SubModuleName { get; set; } = String.Empty;
        public string SubModuleCode { get; set; } = String.Empty;
        public string SubModulePathURL { get; set; } = String.Empty;
        public bool IsSuperAdmin { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}