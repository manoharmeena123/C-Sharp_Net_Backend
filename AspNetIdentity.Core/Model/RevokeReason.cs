using AspNetIdentity.WebApi.Model;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi
{
    public class RevokeReason : DefaultFields
    {
        [Key]
        public int RevokeReasonId { get; set; }
        public int CandidateId { get; set; }
        public string RevokReasons { get; set; }
    }
}