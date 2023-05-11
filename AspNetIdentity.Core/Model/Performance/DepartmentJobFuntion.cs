using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class DepartmentJobFuntion : BaseModelClass
    {
        [Key]
        public Guid DepartmentJobFuntionId { get; set; } = Guid.NewGuid();
        public Guid JobFuntionId { get; set; } = Guid.Empty;
        public int DepartmentId { get; set; }
        public CompetencyTypeConstants CompetencyTypeId { get; set; }
        public string DepartmentJobFuntionName { get; set; }
        public Guid? CompentenciesId { get; set; }

    }
}