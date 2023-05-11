using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CompanyNews : BaseModelClass
    {
        [Key]
        public int NewsId { get; set; }
        public string News { get; set; }
        public string Image { get; set; }
        public string NewsHeading { get; set; }
        public bool LikeStatus { get; set; } = false;
    }
}