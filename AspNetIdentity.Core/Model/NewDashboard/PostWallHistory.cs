using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class PostWallHistory : DefaultFields
    {
        [Key]

        public int PostWallHistoryId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public bool IsType { get; set; }
        public string Announcement { get; set; }
        public PostTypeConstants Type { get; set; }
        public string AnnouncementTitle { get; set; }
        public string AboutPoll { get; set; }
        public string PollOption { get; set; }
        public string MentionEmployee { get; set; }
    }
}