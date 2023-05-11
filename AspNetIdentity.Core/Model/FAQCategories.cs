using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class FAQCategories
    {
        [Key]
        public int FAQCategoriesId { get; set; }
        public string CategoriesName { get; set; }
        public string CategoriesDescription { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdateByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}