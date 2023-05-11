using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ESICContribution : DefaultFields
    {
        [Key]
        public int ESICId { get; set; }

        public float ESICRate { get; set; }
        public string EffectivePeriodFrom { get; set; }
        public string EffectivePeriodTo { get; set; }
        public string Description { get; set; }
    }
}