using System;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.Payment
{
    public class ProjectFeedback : BaseModelClass
    {
        public Guid ProjectFeedbackId { get; set; } = new Guid();
        public string FeedBack { get; set; }
        public int ProjectId { get; set; }
        public string CustomerName { get; set; }
        public ProjectConstants Project { get; set; }
        public string Comments { get; set; }
        public string ReviewType { get; set; }
        public string Rating { get; set; }

    }
}