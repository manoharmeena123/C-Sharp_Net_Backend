using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class AddUserRatingSkill : DefaultFields
    {
        [Key]
        public int RatingSkillId { get; set; }

        public int UserId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Rating { get; set; }
    }
}