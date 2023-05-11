using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class CoreCompetency : BaseModelClass
    {
        [Key]
        public Guid CoreCompetencyId { get; set; } = Guid.NewGuid();
        public string CoreCompetencyName { get; set; }
        public string Description { get; set; }
        public CompetencyTypeConstants CompetencyTypeId { get; set; }

        [NotMapped]
        public List<Behaviour> Behaviours { get; set; }


    }
}