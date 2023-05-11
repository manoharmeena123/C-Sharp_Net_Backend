using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.LeaveMasterModel
{
    /// <summary>
    /// Created By Harshit Mitra On 22-07-2022
    /// </summary>
    public class LeaveRequest : DefaultFields
    {
        [Key]
        public int LeaveRequestId { get; set; }

        public int LeaveTypeId { get; set; }
        public LeaveStatusConstants Status { get; set; }
        public string Details { get; set; }
        public string Documents { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double RequestDay { get; set; }
        public double LeaveDay { get; set; }
        public bool LeaveOnSameDay { get; set; }
        public LeaveCase? CaseA { get; set; }
        public LeaveCase? CaseB { get; set; }
        public int RequestedBy { get; set; }
        public int ReportingManagerId { get; set; }
        public string RejectReason { get; set; }
    }
}