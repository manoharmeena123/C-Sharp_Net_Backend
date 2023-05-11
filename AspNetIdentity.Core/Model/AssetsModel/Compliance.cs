using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class Compliance : DefaultFields
    {
        [Key]
        public int ComplianceId { get; set; }

        public string ComplianceName { get; set; }
    }
}