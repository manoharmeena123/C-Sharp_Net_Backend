using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class FeedBackResponse : BaseModelClass
    {
        [Key]
        public int FeedbackResponseId { get; set; }
        public int RequestFeedbackId { get; set; }
        public int RequestFeedbackFrom { get; set; }
        public bool IsFeedbackGiven { get; set; } = false;
        public string FeebBackMessage { get; set; }
    }
}