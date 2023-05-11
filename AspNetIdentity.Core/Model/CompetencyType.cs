using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CompetencyType : DefaultFields
    {
        [Key]
        public int CompetencyTypeId { get; set; }

        public string CompetencyTypeName { get; set; }
        public string Description { get; set; }
    }
}