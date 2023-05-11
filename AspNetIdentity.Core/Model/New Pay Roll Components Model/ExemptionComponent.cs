using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll_Components_Model
{
    public class ExemptionComponent
    {
        [Key]
        public Guid ExemptionComponentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SectionId { get; set; } = Guid.Empty;

        [Required]
        [StringLength(50)]
        public string ExemptionName { get; set; } = String.Empty;

        [StringLength(250)]
        public string Description { get; set; } = String.Empty;

        //public int Country { get; set; } =0;
        //public int State { get; set; } = 0;
        //public int Month { get; set; } = 13;
        public bool IsPercentage { get; set; } = false;
        public double Value { get; set; } = 0;
        public ComponentTypeConstants Component { get; set; } = ComponentTypeConstants.Not_Select;
        public double Limit { get; set; } = 0;
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 0;
        public bool HasAffectOnGross { get; set; } = false;
        public DeclarationStatusConstants DeclarationStatus { get; set; } = DeclarationStatusConstants.Not_Sublimated;
        public double DeclarationAmount { get; set; }
        public string DocumentProof { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedOn { get; set; } = null;
    }
}