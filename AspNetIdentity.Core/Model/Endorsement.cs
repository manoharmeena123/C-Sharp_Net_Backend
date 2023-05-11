using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Endorsement : DefaultFields
    {
        [Key]
        public int EndorsementId { get; set; }

        public int EmployeeId { get; set; }
        public int BadgeId { get; set; }
        public int EndorsementTypeId { get; set; }
        public string EndorsementsName { get; set; }
        public string DesignationName { get; set; }
        public int RoleId { get; set; }
        public string Comments { get; set; }
    }
}