using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class PolicyGroup : DefaultFields
    {
        [Key]

        public int PolicyGroupId { get; set; }
        public string PolicyGroupName { get; set; }


    }
}