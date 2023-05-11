using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra
    /// </summary>
    public class HiringFlowMaster : DefaultFields
    {
        [Key]
        public int HiringFlowId { get; set; }

        public string HiringFlowTitle { get; set; }
        public string OrderStagesId { get; set; }
    }
}