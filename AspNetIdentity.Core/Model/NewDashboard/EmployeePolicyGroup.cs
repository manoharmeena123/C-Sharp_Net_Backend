using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class EmployeePolicyGroup : DefaultFields
    {
        [Key]
        public int EmpGroupId { get; set; }
        public int PolicyGroupId { get; set; }
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }

    }
}