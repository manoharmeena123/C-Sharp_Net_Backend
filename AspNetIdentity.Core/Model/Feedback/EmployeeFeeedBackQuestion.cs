using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Feedback
{
    public class EmployeeFeeedBackQuestion : DefaultFields
    {
        [Key]
        public int FBQuestionId { get; set; }

        public int EmpFeedbackId { get; set; }
        public int CategoryId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int ProvidedMarks { get; set; }
    }
}