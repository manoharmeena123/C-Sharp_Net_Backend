using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SkillGroup : DefaultFields
    {
        [Key]
        public int SkillGroupId { get; set; }

        public string SkillGroupName { get; set; }
        public string Description { get; set; }
    }
}