using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeFolder
    {
        [Key]
        public int FolderId { get; set; }

        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string FolderName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool IsFolder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}