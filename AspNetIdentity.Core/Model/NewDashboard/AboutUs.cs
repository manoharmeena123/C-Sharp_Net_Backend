using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class AboutUs : DefaultFields
    {
        [Key]
        public int AboutId { get; set; }
        public string AboutDescription { get; set; }
        public bool IsDraft { get; set; }
        public AboutUsStatusConstants Status { get; set; }

    }
}