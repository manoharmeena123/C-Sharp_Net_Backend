using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.SkillModel
{
    public class SkillRequest : DefaultFields
    {
        [Key]
        public int RequestId { get; set; }

        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public int RequestById { get; set; }
        public string RequestedId { get; set; }
        public string ApprovedBy { get; set; }
        public int ApprovedById { get; set; }
        public DateTime RequestOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string Status { get; set; }
        public int ApprovedSkillId { get; set; }
    }
}