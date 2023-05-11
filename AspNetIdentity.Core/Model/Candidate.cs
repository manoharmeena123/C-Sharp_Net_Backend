using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class Candidate : DefaultFields
    {
        [Key]
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CurrentDesignation { get; set; }
        public string Experience { get; set; }
        public string RelevantExperience { get; set; }
        public string CurrentJobLocation { get; set; }
        public string Qualifications { get; set; }
        public string PositionAppliedFor { get; set; }
        public string UploadResume { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string HomeTown { get; set; }
        public string TechnologyKnown { get; set; }
        public string CurrentCTC { get; set; }
        public string ExpectedCTC { get; set; }
        public int Status { get; set; }
        public int RoleID { get; set; }
        public string CompanyName { get; set; }
        public DateTime? InterviewSechduleDate { get; set; }
        public string SecondaryContact { get; set; }
        public string NoticePeriod { get; set; }
        public string InterViewType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Source { get; set; }
        public int Availability { get; set; }
        public string Availabilitys { get; set; } = string.Empty;
        public string Gender { get; set; }
        public string CurrentLocation { get; set; }
        public int JobId { get; set; }
        public Guid? StageId { get; set; } = Guid.Empty;
        public StageFlowType StageType { get; set; }

        public PreboardingStages PrebordingStages { get; set; }
        public DateTime? PendingSince { get; set; }
        public int ReferredBy { get; set; }
        public string DocUrl10 { get; set; }
        public string DocUrl12 { get; set; }
        public string DocUrlUg { get; set; }
        public string DocUrlPg { get; set; }
        public string DocUrlAAdhar { get; set; }
        public string DocUrlPan { get; set; }
        public string DocUrlOther { get; set; }
        public string OfferLetter { get; set; }
        public string Reason { get; set; }
        public Guid? PreboardingArchiveStage { get; set; }
        public Guid? HiringArchiveStage { get; set; }
        public string FcmId { get; set; }
        public int PrebordingEmployeeId { get; set; }
        public bool IsCredentialProvided { get; set; }
        public string Description { get; set; }
        public string SalaryType { get; set; }
        public string PreferredLocation { get; set; }
        public DateTime? JoinedDate { get; set; }
        public bool IsPreboardingStarted { get; set; }
        public bool NewCandidate { get; set; }
        public int CurrentMeetingSecduleId { get; set; } = 0;
        public string CurrentOrganization { get; set; } = string.Empty;

    }
}