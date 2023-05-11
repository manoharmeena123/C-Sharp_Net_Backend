using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class Benefits : DefaultFields
    {
        [Key]

        public int BenefitsId { get; set; }
        public string BenifitsServicesDescription { get; set; }
        public bool IsDraft { get; set; }
        public AboutUsStatusConstants Status { get; set; }
    }
}