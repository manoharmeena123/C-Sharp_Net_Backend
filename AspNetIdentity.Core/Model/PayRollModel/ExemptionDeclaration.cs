using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    public class ExemptionDeclaration
    {
        [Key]
        public Guid ExemptionDeclarationId { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(50)]
        public string ExemptionDeclarationName { get; set; } = String.Empty;
        [StringLength(250)]
        public string Description { get; set; } = String.Empty;
        public int Country { get; set; } = 0;
        //public DeductionType Deductionfor { get; set; }
        //public bool IsPercentage { get; set; } = false;
        //public double Value { get; set; } = 0;
        //public ComponentType Component { get; set; }
        //public double Limit { get; set; }=0;
        //public double Min { get; set; }=0;
        //public double Max { get; set; } = 0;
        //public bool HasAffectOnGross { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedOn { get; set; } = null;
        //public ComponentType Component { get; set; } = ComponentType.Not_Select;
        //public string DocumentProof { get; set; }
    }
}