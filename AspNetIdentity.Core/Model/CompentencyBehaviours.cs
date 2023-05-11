using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CompentencyBehaviours : DefaultFields
    {
        [Key]
        public int BehavioursId { get; set; }

        public int CompetencyId { get; set; }
        public string BehavioursName { get; set; }
        public bool UseInRating { get; set; }
    }
}