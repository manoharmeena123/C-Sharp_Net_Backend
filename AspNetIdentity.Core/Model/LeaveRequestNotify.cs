using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class LeaveRequestNotify : DefaultFields
    {
        [Key]
        public int LRNotifyId { get; set; }

        public int LeaveRequestId { get; set; }
        public int EmpNotifyTo { get; set; }
        public int NotifyBy { get; set; }
    }
}