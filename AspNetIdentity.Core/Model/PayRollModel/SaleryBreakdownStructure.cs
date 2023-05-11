using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    public class SaleryBreakdownStructure : BaseModelClass
    {
        [Key]
        public Guid SaleryBreakdownStructureId { get; set; } = Guid.NewGuid();
        public string SaleryBreakdownStructureName { get; set; } = string.Empty;
        public int EmployeeId { get; set; } = 0;
        public double GrossSalary { get; set; } = 0;
        public double BaseSalary { get; set; } = 0;
        public double HRA { get; set; } = 0;
        public double NetSalary { get; set; } = 0;
        public string Allowance { get; set; } = string.Empty;
        public string TotalTaxes { get; set; } = string.Empty;
        public string TotalDeduction { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; } = 0;
        public bool IsCurrent { get; set; } = false;
        public int RevisedCount { get; set; } = 0;
    }
}