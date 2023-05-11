using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.Clienttask
{
    public class clientTaskModel : BaseModelClass
    {
        [Key]
        public Guid ClientTaskId { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; }
        public string ClientCode { get; set; }
        public DateTimeOffset TaskDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid WorkTypeId { get; set; }
        public string Description { get; set; }
    }
}