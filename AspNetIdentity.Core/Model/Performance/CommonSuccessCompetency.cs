using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    /// <summary>
    /// c
    /// </summary>
    public class CommonSuccessCompetency : BaseModelClass
    {
        [Key]
        public Guid CommonSuccessCompetencyId { get; set; } = Guid.NewGuid();
        public string CommonSuccessCompetencyName { get; set; }

        public string Description { get; set; }
        public CompetencyTypeConstants CompetencyTypeId { get; set; }
        //  public string CompetencyTypeName { get; set; }



        [NotMapped]
        public List<Behaviour> Behaviours { get; set; }

    }
}