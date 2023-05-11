using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model.Teams
{
    public class TeamMembers : DefaultFields
    {
        [Key]
        public int TeamMemberId { get; set; }

        public int TeamMasterId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int DesignationId { get; set; }
        public string EmployeeDesignation { get; set; }

        [NotMapped]
        public bool IsLead { get; set; }
    }
}