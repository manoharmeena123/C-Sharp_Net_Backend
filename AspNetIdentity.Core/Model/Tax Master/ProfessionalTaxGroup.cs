using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Tax_Master
{
    /// <summary>
    /// Created By Harshit Mitra On 05-09-2022
    /// </summary>
    public class ProfessionalTaxGroup
    {
        [Key]
        public int PTGroupId { get; set; }

        public string GroupTitle { get; set; }
        public string Discription { get; set; }
        public int CountryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}