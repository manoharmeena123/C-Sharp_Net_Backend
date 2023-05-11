using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class CommentHistory : DefaultFields
    {
        [Key]
        public int CommentHistoryId { get; set; }
        public int CommentId { get; set; }
        public int EmployeeId { get; set; }
        public int PostId { get; set; }
        public bool IsComment { get; set; }
        public string Comments { get; set; }
        public PostTypeConstants CommentType { get; set; }
        public string MentionEmployee { get; set; }

    }
}