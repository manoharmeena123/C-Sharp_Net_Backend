using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class GoalsPermission : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int EmployeeId { get; set; } = 0;
        public string Permission { get; set; } = JsonConvert.SerializeObject(new ModuleClass());
    }
    public class ModuleClass
    {
        public PermissionBody Individual_Goal { get; set; } = new PermissionBody();
        public PermissionBody Departmental_Goal { get; set; } = new PermissionBody();
        public PermissionBody Company_Goal { get; set; } = new PermissionBody();
    }
    public class PermissionBody
    {
        public bool Create { get; set; } = false;
        public bool View { get; set; } = false;
        public bool Dashboard { get; set; } = false;
    }
}