using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class NewTimeSheet
    {
        [Key]
        public int NewTimeSheetId { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string Note { get; set; }
        public DateTime Time { get; set; }
        public DateTime? Date { get; set; }
        public bool IsTimeStart { get; set; }
        public string TotalTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string day { get; set; }
    }
}