using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.HiringQuestion
{
    public class HiringQuesionsAndAnsBank : BaseModelClass
    {
        [Key]
        public Guid AnswerId { get; set; } = Guid.NewGuid();
        public Guid QuesionsId { get; set; } = Guid.NewGuid();
        public string Answer { get; set; } = string.Empty;
        public int CandidateId { get; set; } = 0;
        public int JobId { get; set; } = 0;
    }
}