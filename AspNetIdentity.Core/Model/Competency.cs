using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Competency : DefaultFields
    {
        [Key]
        public int CompetencyId { get; set; }

        public int CompetencyTypeId { get; set; }
        public string CompetencyName { get; set; }
        public string Description { get; set; }
    }
}