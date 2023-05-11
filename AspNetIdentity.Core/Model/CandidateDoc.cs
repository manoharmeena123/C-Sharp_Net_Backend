using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class CandidateDoc : DefaultFields
    {
        [Key]
        public int DocumentId { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public bool PanCard { get; set; }
        public bool AadharCard { get; set; }
        public bool Passport { get; set; }
        public bool BankPassbook { get; set; }
        public bool Marksheet10Th { get; set; }
        public bool Marksheet11Th { get; set; }
        public bool Marksheet12Th { get; set; }
        public bool UgLastSemMarksheet { get; set; }
        public bool PgLastSemMarksheet { get; set; }
        public bool UgDegree { get; set; }
        public bool PgDegree { get; set; }
        public bool ExperienceLetter { get; set; }
        public bool PaySlips3months { get; set; }
        public bool Resignation { get; set; }
        public bool Certificate { get; set; }
        public string Url { get; set; }
        public bool IsDocumentSubmited { get; set; }
    }
}