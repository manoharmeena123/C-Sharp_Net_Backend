using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class WorkingAt : DefaultFields
    {
        [Key]

        public int WorkingAtId { get; set; }
        public string WorkingAtDescription { get; set; }
        public bool IsDraft { get; set; }
        public AboutUsStatusConstants Status { get; set; }
    }
}