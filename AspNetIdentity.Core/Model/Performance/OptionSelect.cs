using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class OptionSelect : BaseModelClass
    {
        [Key]
        public int OptionId { get; set; }
        public Guid ReviewQuestionId { get; set; } = Guid.Empty;
        public string Option { get; set; }
    }
}