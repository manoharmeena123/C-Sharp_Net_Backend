using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class MeetingScheduleInterview : DefaultFields
    {
        [Key]
        public int CalenderId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string MobileNumber { get; set; }
        public int JobId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string TimeZone { get; set; }
        public int InterviewType { get; set; }
        public string InterviewTypeName { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string CandidateEmail { get; set; }
        public string InterviwerEmail { get; set; }
        public string HrEmail { get; set; }
        public string RecruitersEmail { get; set; }
        public string MettingId { get; set; }
        public InterviewStatus InterviewStatusvalue { get; set; }
        public string MettingUrl { get; set; }
    }
}