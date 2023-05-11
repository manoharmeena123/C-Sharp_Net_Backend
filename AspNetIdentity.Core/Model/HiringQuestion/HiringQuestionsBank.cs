using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.HiringQuestion
{
    public class HiringQuestionsBank : BaseModelClass
    {
        [Key]
        public Guid QuesionsId { get; set; } = Guid.NewGuid();
        public int JobId { get; set; } = 0;
        public string QuesionTitle { get; set; } = string.Empty;
        public bool ShowQuestionOnPortal { get; set; } = false;
    }
}