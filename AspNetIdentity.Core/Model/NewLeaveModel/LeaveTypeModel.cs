using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewLeaveModel
{
    /// <summary>
    /// Created By Harshit Mitra On 14-02-2023
    /// </summary>
    public class LeaveTypeModel : BaseModelClass
    {
        [Key]
        public Guid LeaveTypeId { get; set; } = Guid.NewGuid();
        public int OldLeaveTypeId { get; set; } = 0;
        public string LeaveTypeName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsPaidLeave { get; set; } = false;
        public bool RestrictToG { get; set; } = false;
        public string Gender { get; set; } = String.Empty;
        public bool RestrictToS { get; set; } = false;
        public string Status { get; set; } = String.Empty;
        public bool IsReasonRequired { get; set; } = false;
        public bool IsDelatable { get; set; } = true;
    }
}