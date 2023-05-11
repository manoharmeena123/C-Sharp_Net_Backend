using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class InterviewRound
    {
        [Key]
        public int InterviewId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public Nullable<System.DateTime> InterviewDate { get; set; }
        public List<int> InterviewerId { get; set; }
        public string CandidateMail { get; set; }
        public int CandidateId { get; set; }
        public string InterviewRoundName { get; set; }
        public int HR { get; set; }
        public string InterviewLink { get; set; }
        public bool Qualified { get; set; }
        public string Status { get; set; }

        public string InterviewTitle { get; set; }

        public string Description { get; set; }

        public string InterviewerMail { get; set; }
        public string InterviewerName { get; set; }

        [NotMapped]
        public string Name { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}