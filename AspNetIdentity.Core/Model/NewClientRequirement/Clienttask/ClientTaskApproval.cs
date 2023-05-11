using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.Clienttask
{
    public class ClientTaskApproval : BaseModelClass
    {
        [Key]
        public Guid ClientTaskApprovalId { get; set; } = Guid.NewGuid();
        public Guid ClientTaskId { get; set; } = Guid.Empty;
        public Guid ClientId { get; set; } = Guid.Empty;
        public string ClientCode { get; set; }
        //public DateTimeOffset? Taskdate { get; set; }
        //public DateTimeOffset? StartTime { get; set; }
        //public DateTimeOffset? EndTime { get; set; }
        public Guid WorkTypeId { get; set; }
        public string ReEvaluteDiscription { get; set; }
        public string TaskName { get; set; }
        public bool IsSFA { get; set; }
        public ClientTaskRequestConstants TaskRequest { get; set; } = ClientTaskRequestConstants.New;
        public bool IsApproved { get; set; }
        public bool IsRe_Evaluate { get; set; }
        public int ProjectManagerId { get; set; } = 0;
        //   public string TotalWorkingTime { get; set; }
    }
    public enum ClientTaskRequestConstants
    {
        New = 1,
        Pending = 2,
        Approved = 3,
        Revaluation = 4,
    }
    public enum OrderbyRequestConstants
    {
        A_to_Z = 1,
        Z_to_A = 2,
        Latest = 3,
        Oldest = 4,
    }
    public enum FilterSortConstants
    {
        CreatedOn = 0,
        Name = 1,
        Date = 2,
    }
}