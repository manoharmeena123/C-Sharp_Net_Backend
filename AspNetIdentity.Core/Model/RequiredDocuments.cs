using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 10-02-2022
    /// </summary>
    public class RequiredDocuments
    {
        [Key]
        public int DocRecId { get; set; }

        public int ReqDocMasterId { get; set; }
        public int DocTypeId { get; set; }
        public int CandidateId { get; set; }
        public int CandidateDocId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentStatus { get; set; }
        public int DocType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        //public string Reason { get; set; }
    }
}