using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Header
{
    public class HomeHeader : BaseModelClass
    {
        [Key]
        public int NavigaionId { get; set; }
        public string NavigationName { get; set; }
        public NavigationConstants NavigationNameId { get; set; }
    }
}