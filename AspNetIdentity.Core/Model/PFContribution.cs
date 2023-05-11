using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class PFContribution : DefaultFields
    {
        [Key]
        public int PFCId { get; set; }

        public float PFRate { get; set; }
        public string EffectivePeriodFrom { get; set; }
        public string EffectivePeriodTo { get; set; }
        public string Description { get; set; }
    }
}