using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TicketsModel
{
    /// <summary>
    /// Created By Harshit Mitra on 23-03-2022
    /// </summary>
    public class TicketComment : DefaultFields
    {
        [Key]
        public int TicketCommentId { get; set; }

        public int TicketId { get; set; }
        public string Message { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        //public string ImagesUrls { get; set; }
        public string CommentBy { get; set; }
        public string CommentOn { get; set; }
    }
}