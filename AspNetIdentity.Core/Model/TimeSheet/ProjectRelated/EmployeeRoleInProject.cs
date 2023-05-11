using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class EmployeeRoleInProject : BaseModelClass
    {
        [Key]

        public Guid EmployeeRoleInProjectId { get; set; } = Guid.NewGuid();
        public int ProjectId { get; set; } = 0;
        public string RoleName { get; set; } = string.Empty;
    }
}