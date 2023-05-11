using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.PayRollModel.Payroll_Tax_Master
{
    public class ProfessionalTaxes
    {
        [Key]
        public Guid PTaxId { get; set; } = Guid.NewGuid();
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public double TaxAmount { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsLastMonthDiffrent { get; set; }
        public int LastMonthId { get; set; }
        public double LastMonthTax { get; set; }
        public PTRangeConstants RangeStatus { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}