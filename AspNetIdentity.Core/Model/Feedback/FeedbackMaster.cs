using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class FeedbackMaster
    {
        [Key]
        public int FeedbackId { get; set; }

        public int ReceiverEmployeeId { get; set; }

        public int AverageScore { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RatedByEmpId { get; set; }
        public string YourFeedback { get; set; }
        public int RoleId { get; set; }
        public int CategoryTypeId { get; set; }
        public int TeamLeadId { get; set; }

        [NotMapped]
        public FeedbackScore[] FBScore { get; set; }
    }
}