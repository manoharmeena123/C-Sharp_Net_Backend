using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class FeedbackScore
    {
        [Key]
        public int FeedbackScoreId { get; set; }

        public int FeedbackId { get; set; }
        public int CategoryId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionScore { get; set; }
    }

    [NotMapped]
    public class QuestionScore
    {
        public string QuestionName { get; set; }
        public int QuestionId { get; set; }
        public int Score { get; set; }

        //public float QuestionScoreTotal { get; set; }
        public int CompanyId { get; set; }

        public int OrgId { get; set; }
    }
}