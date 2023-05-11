using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CredentilData : DefaultFields
    {
        [Key]
        public int CredentilId { get; set; }

        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CredentialMessage { get; set; }
    }
}