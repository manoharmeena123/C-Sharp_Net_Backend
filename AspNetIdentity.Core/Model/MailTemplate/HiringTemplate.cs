using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.MailTemplate
{
    public class HiringTemplate : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public TemplateTypeConstants Templatetype { get; set; } = TemplateTypeConstants.InterviewSchedule;
        public bool IsCustmized { get; set; } = false;
        public string TemplateForInterviewer { get; set; } = string.Empty;
        public string TemplateForRecruiter { get; set; } = string.Empty;
        public string TemplateForCandidate { get; set; } = string.Empty;
    }
    /// <summary>
    /// Created By Ankit on 16-02-2023
    /// Its For Identify Tempalte Tyepe
    /// </summary>
    public enum TemplateTypeConstants
    {
        InterviewSchedule = 1,
        RescheduleInterview = 2,
        FaceToFaceInterview = 3,

    }
}