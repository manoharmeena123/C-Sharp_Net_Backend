using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class SalaryComponent : DefaultFields
    {
        [Key]
        public int ComponentId { get; set; }

        public string ComponentName { get; set; }
        public int ComponentTypeId { get; set; }
        public string ComponentType { get; set; }
        public string TaxExempt { get; set; }
        public string TaxExemptRemark { get; set; }
        public string MaxLimit { get; set; }
        public bool RequiredDocs { get; set; }
    }
}