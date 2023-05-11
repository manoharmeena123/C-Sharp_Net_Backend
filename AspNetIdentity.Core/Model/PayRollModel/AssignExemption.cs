using System;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    public class AssignExemption : BaseModelClass
    {
        public Guid AssignExemptionId { get; set; } = Guid.NewGuid();
        public Guid ExemptionId { get; set; } = Guid.NewGuid();
        public int EmployeeId { get; set; } = 0;
    }
}