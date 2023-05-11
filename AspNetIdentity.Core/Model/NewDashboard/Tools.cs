using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class Tools : DefaultFields
    {
        [Key]

        public int ToolId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }
}