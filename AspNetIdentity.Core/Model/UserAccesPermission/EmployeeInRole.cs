using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.UserAccesPermission
{
    public class EmployeeInRole : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
    }
}