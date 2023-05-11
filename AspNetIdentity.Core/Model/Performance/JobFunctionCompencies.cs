using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class JobFunctionCompencies : BaseModelClass
    {
        [Key]

        public Guid JobFunctionCompenciesId { get; set; } = Guid.NewGuid();
        public int? DepartmentId { get; set; }
        public Guid? CompentenciesId { get; set; }
        public Guid? JobFuntionId { get; set; } = Guid.Empty;
        public CompetencyTypeConstants CompetencyTypeId { get; set; }
        public string CompentenciesName { get; set; } = string.Empty;
        public int Weight { get; set; }
        public string CoreName { get; set; }

    }
}