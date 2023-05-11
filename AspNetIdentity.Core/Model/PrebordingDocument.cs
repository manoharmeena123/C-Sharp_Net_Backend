using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class PrebordingDocument : DefaultFields
    {
        [Key]
        public int PrebordDocId { get; set; }
        public int DocumentId { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public string PanCard { get; set; }
        public string AadharCard { get; set; }
        public string Passport { get; set; }
        public string BankPassbook { get; set; }
        public string Marksheet10Th { get; set; }
        public string Marksheet11Th { get; set; }
        public string Marksheet12Th { get; set; }
        public string UgLastSemMarksheet { get; set; }
        public string PgLastSemMarksheet { get; set; }
        public string UgDegree { get; set; }
        public string PgDegree { get; set; }
        public string ExperienceLetter { get; set; }
        public string PaySlips3months { get; set; }
        public string Resignation { get; set; }
        public string Certificate { get; set; }


    }
}