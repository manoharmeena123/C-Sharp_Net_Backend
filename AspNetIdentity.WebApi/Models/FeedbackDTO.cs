using AspNetIdentity.WebApi.Model;

namespace AspNetIdentity.WebApi.Models
{
    public class FeedbackDTO
    {
        public FeedbackMaster feedback { get; set; }
        public CategoryType categoryType { get; set; }
    }
}