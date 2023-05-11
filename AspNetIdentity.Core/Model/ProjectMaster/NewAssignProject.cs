using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ProjectMaster
{
    public class NewAssignProject : BaseModelClass
    {
        [Key]
        public Guid AssignProjectId { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Status { get; set; }
    }
}