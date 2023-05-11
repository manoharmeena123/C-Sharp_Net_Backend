using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Feedback
{
    public class FeedbackQuestions : DefaultFields
    {
        [Key]
        public int QuestionId { get; set; }

        public int FBCategoryId { get; set; }
        public string Questions { get; set; }
    }
}