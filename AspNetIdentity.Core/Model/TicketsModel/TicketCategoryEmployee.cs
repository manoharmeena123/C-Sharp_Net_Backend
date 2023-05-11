using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class TicketCategoryEmployee : DefaultFields
    {
        [Key]
        public int TicketCategoryEmployeeId { get; set; }

        public int TicketCategoryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}