using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class AssignProject : BaseModelClass
    {
        [Key]
        public int AssignProjectId { get; set; }

        public int ProjectId { get; set; }
        public Guid EmployeeRoleInProjectId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; }
        public string OccupyPercent { get; set; }
        public int ManagerId { get; set; }
        public string Status { get; set; }
        public bool IsProjectManager { get; set; }

    }
}