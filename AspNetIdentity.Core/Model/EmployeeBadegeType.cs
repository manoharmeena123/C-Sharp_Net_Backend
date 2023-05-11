using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeBadegeType : DefaultFields
    {
        [Key]
        public int EmployeeId { get; set; }

        public int BadgeId { get; set; }
    }
}