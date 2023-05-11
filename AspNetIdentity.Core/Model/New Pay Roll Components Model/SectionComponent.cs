using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll_Components_Model
{
    public class SectionComponent
    {
        [Key]
        public Guid SectionId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string SectionName { get; set; } = String.Empty;

        public Guid ExemptionDeclarationId { get; set; } = Guid.Empty;

        [StringLength(250)]
        public string Description { get; set; } = String.Empty;

        public int Country { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedOn { get; set; } = null;
        public DateTimeOffset? DeletedOn { get; set; } = null;
    }
}