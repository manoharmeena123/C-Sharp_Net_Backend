using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.EmployeeModel
{
    public class EmployeeReportingManager
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public bool IsTop { get; set; }
    }
}