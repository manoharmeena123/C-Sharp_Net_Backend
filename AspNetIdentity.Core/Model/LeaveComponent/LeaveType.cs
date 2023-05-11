namespace AspNetIdentity.WebApi.Model.Leave
{
    /// <summary>
    /// Created BY Harshit Mitra On 23-07-2022
    /// </summary>
    public class LeaveType : DefaultFields
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public string Description { get; set; }
        public bool IsPaidLeave { get; set; }
        public bool RestrictToG { get; set; }
        public string Gender { get; set; }
        public bool RestrictToS { get; set; }
        public string Status { get; set; }
        public bool IsReasonRequired { get; set; }
        public bool IsDelatable { get; set; }
    }
}