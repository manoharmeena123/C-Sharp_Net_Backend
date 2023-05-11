using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CoreValueBehaviour : DefaultFields
    {
        [Key]
        public int BehavioursId { get; set; }

        public int CoreValueId { get; set; }
        public string BehavioursName { get; set; }
        public bool UseInRating { get; set; }
    }
}