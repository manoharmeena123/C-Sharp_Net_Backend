using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.LeaveMasterModel
{
    /// <summary>
    /// Created By Harshit Mitra On 26-07-2022
    /// </summary>
    public class LeaveRequestHistory : DefaultFields
    {
        [Key]
        public int HistoryId { get; set; }

        public int LeaveRequestId { get; set; }
        public string Message { get; set; }
        public LeaveStatusConstants Status { get; set; }
    }
}