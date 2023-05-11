using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Badges : DefaultFields
    {
        [Key]
        public int BadgeId { get; set; }

        public string BadgeName { get; set; }
        public string ImageUrl { get; set; }
        public int BadgeType { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}