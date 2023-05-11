using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class WeightAge : BaseModelClass
    {
        [Key]
        public int WeightAgeId { get; set; }
        public int WeightAgee { get; set; }
        public int Weightage1 { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeId { get; set; }
        public Guid PrimaryJobFunction { get; set; } = Guid.Empty;
        public Guid SecondaryJobFunction { get; set; } = Guid.Empty;
    }
}