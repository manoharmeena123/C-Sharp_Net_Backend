using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SkillsInSkillsGroup : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public int SkillGroupId { get; set; }
        public int SkillId { get; set; }
        public string SkillGroupName { get; set; }
    }
}