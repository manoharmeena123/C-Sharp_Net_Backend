using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class Reaction : DefaultFields
    {
        [Key]
        public int ReactionId { get; set; }
        public int PostId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsReaction { get; set; }
        public PostTypeConstants Type { get; set; }
        public bool IsLike { get; set; }
    }
}