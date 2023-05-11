using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SkillMaster : DefaultFields
    {
        [Key]
        public int SkillId { get; set; }

        public string SkillName { get; set; }
        public string Description { get; set; }
        public string ApprovedId { get; set; }
        public int SkillRequestId { get; set; }
    }
}