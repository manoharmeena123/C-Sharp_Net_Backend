using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 14/12/2022
    /// </summary>
    public class RecuringComponent : BaseModelClass
    {
        [Key]
        public Guid RecuringComponentId { get; set; } = Guid.NewGuid();
        public string ComponentName { get; set; } = String.Empty;
        public PayRollComponentTypeConstants ComponentType { get; set; }
        public bool IsAutoCalculated { get; set; } = false;
        public double MaxiumLimitPerYear { get; set; } = 0.0;
        public string Description { get; set; } = String.Empty;
        public bool IsTaxExempted { get; set; } = false;
        public string IncomeTaxSection { get; set; } = String.Empty;
        public double SectionMaxLimit { get; set; } = 0.0;
        public bool IsDocumentRequired { get; set; } = false;
        /// <summary>
        /// Include In Gross Calculations 
        /// </summary>
        public bool ShowOnPaySlip { get; set; } = true; // Include In Gross Calculations 
        public bool IsEditable { get; set; } = true;
    }
}