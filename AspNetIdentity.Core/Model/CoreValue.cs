using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CoreValue : DefaultFields
    {
        [Key]
        public int CoreValueId { get; set; }

        public string CoreValueName { get; set; }
        public string Description { get; set; }
    }
}