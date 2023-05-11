using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class JobFuntion : BaseModelClass
    {
        [Key]
        public Guid JobFuntionId { get; set; } = Guid.NewGuid();
        public string JobFuntionName { get; set; }
        public int EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        public JobLevelsConstants JobLevelsEnumId { get; set; }
        public CompetencyTypeConstants CompetencyTypeId { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public int? DepartmentId { get; set; }
        public string Description { get; set; }
        public Guid CompetenciesId { get; set; }
    }
}