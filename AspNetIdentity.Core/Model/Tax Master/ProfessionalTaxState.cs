using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Tax_Master
{
    public class ProfessionalTaxState
    {
        [Key]
        public int PTStateId { get; set; }

        public int PTGroupId { get; set; }
        public int StateId { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        public GoalTypeEnum_PTStateDuration Duration { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}