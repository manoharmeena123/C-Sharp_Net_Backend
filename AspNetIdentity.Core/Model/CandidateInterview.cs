using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 02-03-2022
    /// </summary>
    public class CandidateInterview : DefaultFields
    {
        [Key]
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public int InterviewType { get; set; }
        public int EmployeeId { get; set; }
        public string InterviewTypeName { get; set; }
        public string CandidateName { get; set; }
        public Guid StageId { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public string ReviewURL { get; set; }
        public string MeetingURL { get; set; }
        public bool IsReviewSubmited { get; set; } = false;
        public bool IsReschedule { get; set; }
        public int RescheduleCount { get; set; }
        public InterviewStatus InterviewStatusvalue { get; set; }
        public string CancelReason { get; set; }
        public string MettingId { get; set; }
    }
}