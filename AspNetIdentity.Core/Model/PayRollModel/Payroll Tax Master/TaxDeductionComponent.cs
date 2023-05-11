using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.PayRollModel.Payroll_Tax_Master
{
    public class TaxDeductionComponent : BaseModelClass
    {
        [Key]
        public Guid TaxDeductionId { get; set; } = Guid.NewGuid();
        public string DeductionName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int Country { get; set; } = 0;
        public int State { get; set; } = 0;
        public int Month { get; set; } = 0;
        public string ComponentFormula { get; set; } = String.Empty;
        public bool IsPercentage { get; set; } = false;
        public double Value { get; set; } = 0.0;
        public double Limit { get; set; } = 0.0;
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 0.0;
        public DeductionForConstants DeductionFor { get; set; } = DeductionForConstants.Undefined;
    }
}