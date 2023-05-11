using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskMentionEmployee : BaseModelClass
    {
        [Key]
        public Guid Mentionid { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public int EmployeeId { get; set; }
        public int Mentionby { get; set; }
        public string Comments { get; set; }
    }
}