using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class WfhNofifyByEmployee : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public int WFHId { get; set; }
        public int EmployeeId { get; set; }
    }
}