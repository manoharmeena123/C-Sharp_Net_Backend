using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class StageStatus : DefaultFields
    {
        [Key]
        public int StageStatusId { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public Guid StageId { get; set; }
        public int CandidateId { get; set; }
        public string Reason { get; set; }
        public PreboardingStages PrebordingStageId { get; set; }
        public int StageOrder { get; set; }
        public bool IsReviewSubmited { get; set; }
        public InterviewCommentBy CommentBy { get; set; }
    }
}