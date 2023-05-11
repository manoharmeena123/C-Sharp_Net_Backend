using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.UserAccesPermission
{
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public class CompaniesModuleAccess
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int CompanyId { get; set; } = 0;
        public string ModuleName { get; set; } = String.Empty;
        public string ModuleCode { get; set; } = String.Empty;
        public string ModulePathURL { get; set; } = String.Empty;
    }
}
