using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra On 19/12/2022
    /// </summary>
    public class SalaryStructureConfig : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StructureId { get; set; } = Guid.Empty;
        public Guid ComponentId { get; set; } = Guid.Empty;
        public ComponentTypeInPGConstants ComponentType { get; set; }
        public bool CalculationType { get; set; } // true for auto calculated // true for perctange 
        public string CalculatingValue { get; set; } = String.Empty;
        public string TaxSettings { get; set; } = String.Empty;
        public bool IsCalculationDone { get; set; } = false;
    }
}