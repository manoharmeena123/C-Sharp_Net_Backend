using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class QuickLinks : DefaultFields
    {
        [Key]
        public int LinkId { get; set; }
        public string LinkName { get; set; }
        public string URL { get; set; }
    }
}