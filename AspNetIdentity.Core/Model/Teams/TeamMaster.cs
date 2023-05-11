using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model.Teams
{
    public class TeamMaster : DefaultFields
    {
        [Key]
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        //public int DepartmentId { get; set; }
        public int TeamLeadId { get; set; }

        public string TeamLeadName { get; set; }

        [NotMapped]
        public List<TeamMembers> TeamMemberList { get; set; }
    }
}