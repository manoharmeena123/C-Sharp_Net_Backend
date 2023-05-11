using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ExtraMasterModel
{
    public class NatureOfBusiness
    {
        [Key]
        public int NatureOfBusinessId { get; set; }
        [Required]
        public string NatureOfBusinessName { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

}