using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class UserComment : DefaultFields
    {
        [Key]
        public int CommentId { get; set; }

        public int PostId { get; set; }
        public int EmpId { get; set; }
        public string Comment { get; set; }
    }
}