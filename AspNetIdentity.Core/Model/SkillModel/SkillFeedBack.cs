using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.SkillModel
{
    public class SkillFeedBack : DefaultFields
    {
        [Key]
        public int EmpFeedbackId { get; set; }

        public int FeedbackGivenTo { get; set; }
        public int FeedBackGivenBy { get; set; }
        public string FeedBackProviderName { get; set; }
        public string FeedBackProviderToName { get; set; }
        public int TotalMarks { get; set; }
        public int TotalQuestion { get; set; }
        public float AverageMarks { get; set; }
        public string GivenComment { get; set; }
    }
}