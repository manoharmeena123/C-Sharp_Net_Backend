using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class Behaviour : BaseModelClass
    {
        [Key]
        public Guid BehaviourId { get; set; } = Guid.NewGuid();
        public Guid? CoreCompetencyId { get; set; } = Guid.Empty;
        public Guid? CommonSuccessCompetencyId { get; set; } = Guid.Empty;
        public Guid? JobSpecificCompetencyId { get; set; } = Guid.Empty;
        public Guid? ReviewCoreValueId { get; set; } = Guid.Empty;
        public string Behaveour { get; set; }
        public bool UseInRating { get; set; }
        public bool Action { get; set; }
    }
}