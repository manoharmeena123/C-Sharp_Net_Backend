using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 10-02-2022
    /// </summary>
    public class DocTypeMaster
    {
        [Key]
        public int DocTypeId { get; set; }

        public string DocName { get; set; }
        public int DocType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}