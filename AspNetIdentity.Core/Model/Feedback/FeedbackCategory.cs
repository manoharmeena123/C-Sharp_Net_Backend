using AspNetIdentity.WebApi.Model.Feedback;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 04-04-2022
    /// </summary>
    public class FeedbackCategory : DefaultFields
    {
        [Key]
        public int FBCategoryId { get; set; }

        public string CategoryName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }

        [NotMapped]
        public List<FeedbackQuestions> Questions { get; set; }
    }
}