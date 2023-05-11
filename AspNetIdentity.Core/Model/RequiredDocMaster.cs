using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra On 10-02-2022
    /// </summary>
    public class RequiredDocMaster
    {
        [Key]
        public int ReqDocMasterId { get; set; }

        public int CandidateId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        //public int MyProperty { get; set; }

    }
}