using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 28/11/2022
    /// </summary>
    public class SalaryBreakDown : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int EmployeeId { get; set; } = 0;
        public DateTimeOffset EffectiveFrom { get; set; }
        public double CostToCompany { get; set; } = 0.0;
        public double GrossYearly { get; set; } = 0.0;
        public double GrossMonthly { get; set; } = 0.0;
        public string Earnings { get; set; } = String.Empty;
        public string Deductions { get; set; } = String.Empty;
        public string Others { get; set; } = String.Empty;
        public string Bonus { get; set; } = String.Empty;
        public string StructureDesign { get; set; } = String.Empty;
        public bool IsCurrentlyUse { get; set; } = false;
        public long ChangesCount { get; set; } = 0;

    }
}