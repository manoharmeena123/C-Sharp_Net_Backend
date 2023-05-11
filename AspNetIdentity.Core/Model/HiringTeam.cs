using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class HiringTeam : DefaultFields
    {

        [Key]
        public Guid HiringTeamId { get; set; }
        public virtual JobPost Job { get; set; }
        public virtual Employee Employee { get; set; }
        public bool PreboardingHired { get; set; }
        public HiringTeamConstants Designation { get; set; }

    }
}