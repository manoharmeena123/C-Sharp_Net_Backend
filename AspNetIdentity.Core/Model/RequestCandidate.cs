using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class RequestCandidate : DefaultFields
    {
        [Key]
        public int RequestCandidateId { get; set; }
        public string profile { get; set; }
        public int? NoOfCandidate { get; set; }
        public string EmployeeName { get; set; }
        public RequestCandidatePriority Priority { get; set; }
        public string priorityName { get; set; }
        public string Description { get; set; }
        public double Experience { get; set; }
        public RequestCandidatStatus Status { get; set; }
        public string StatusName { get; set; }
    }
}