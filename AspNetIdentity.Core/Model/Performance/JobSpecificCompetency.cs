using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class JobSpecificCompetency : BaseModelClass
    {
        [Key]
        public Guid JobSpecificCompetencyId { get; set; } = Guid.NewGuid();
        public string JobSpecificCompetencyName { get; set; }
        //public int BehaviourId { get; set; }
        public string Description { get; set; }
        public CompetencyTypeConstants CompetencyTypeId { get; set; }
        public string CompetencyTypeName { get; set; }



        [NotMapped]
        public List<Behaviour> Behaviours { get; set; }
    }
}