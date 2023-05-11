using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Postwall : DefaultFields
    {
        [Key]
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
    public enum PostTypeConstants
    {
        PostWall = 1,
        Announcement = 2,
        Poll = 3,
    }
}