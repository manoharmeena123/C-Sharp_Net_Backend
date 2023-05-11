using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Tax_Master
{
    public class ProfessionalTaxRange
    {
        [Key]
        public int PTRangeId { get; set; }

        public int PTGroupId { get; set; }
        public int PTStateId { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public double TaxAmount { get; set; }
        public bool IsLastMonthDiffrent { get; set; }
        public int LastMonthId { get; set; }
        public string LastMonthName { get; set; }
        public double LastMonthTax { get; set; }
        public PTRangeConstants RangeStatus { get; set; }
        public bool IsDiffrentForFemale { get; set; }
        public double GenderAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}