using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class ReviewQuestion : BaseModelClass
    {
        [Key]
        public Guid ReviewQuestionId { get; set; } = Guid.NewGuid();
        public Guid ReviewGroupId { get; set; } = Guid.Empty;
        public Guid ReviewEmployeeId { get; set; } = Guid.Empty;
        public string ReviewQuestionName { get; set; }
        public OptionTypeConstants OptionType { get; set; }
        public string OptionTypeName { get; set; }
        public ReviewsTypeConstants Reviews { get; set; }
        public string ReviewsName { get; set; }

    }
}