using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class EmployeeTaskReminderHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int EmployeeId { get; set; } = 0;
        public int ReminderSendBy { get; set; } = 0;
        public DateTime LastDate { get; set; } = DateTime.UtcNow;
    }
}