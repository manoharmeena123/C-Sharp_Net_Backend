using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeFiles
    {
        [Key]
        public int FileId { get; set; }

        public int EmployeeId { get; set; }
        public int FolderId { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public int UploadedBy { get; set; }
        public String DownloadFileName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}