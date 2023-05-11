using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll_Components_Model
{
    /// <summary>
    /// Created By Harshit Mitra on 20-04-2022
    /// </summary>
    public class DeductionComponent : DefaultFields
    {
        [Key]
        public int DeductionId { get; set; } = 0;
        public Guid SectionId { get; set; } = Guid.Empty;
        public string DeductionName { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Month { get; set; }
        public DeductionForConstants Deductionfor { get; set; }
        public bool IsPercentage { get; set; }
        public double Value { get; set; }
        public ComponentTypeConstants Component { get; set; }
        public double Limit { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public DeclarationStatusConstants DeclarationStatus { get; set; }
        public double DeclarationAmount { get; set; }
        public string Documentproof { get; set; }
        public bool HasAffectOnGross { get; set; }
    }
}