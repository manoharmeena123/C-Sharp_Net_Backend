using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class AddMultipleEmployee : BaseModelClass
    {
        [Key]
        public Guid MyProperty { get; set; } = Guid.NewGuid();
        public Guid ReviewGroupId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}