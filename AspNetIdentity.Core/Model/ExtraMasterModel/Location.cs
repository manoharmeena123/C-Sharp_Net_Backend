using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ExtraMasterModel
{
    public class Location : DefaultFields
    {
        [Key]
        public int LocationId { get; set; }

        public string LocationName { get; set; }
        public string Address { get; set; }
    }
}