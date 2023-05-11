using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra On 15/12/2022
    /// </summary>
    public class TaxDeductionComponent : BaseModelClass
    {
        [Key]
        public Guid TaxComponentId { get; set; } = Guid.NewGuid();
        public string DeductionName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int CountryId { get; set; } = 0;
        public int StateId { get; set; } = 0;
        public int Month { get; set; } = 13;
        public DeductionForConstants Deductionfor { get; set; } = DeductionForConstants.Undefined;
        public bool IsPercentage { get; set; } = false;
        public double Value { get; set; } = 0.0;
        public string Component { get; set; } = String.Empty;
        public double Limit { get; set; } = 0.0;
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 0.0;
    }
}