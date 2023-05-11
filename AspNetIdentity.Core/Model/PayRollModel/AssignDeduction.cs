using System;

namespace AspNetIdentity.WebApi.Model.PayRollModel
{
    public class AssignDeduction : BaseModelClass
    {
        public Guid AssignDeductionId { get; set; } = Guid.NewGuid();
        public int DeductionId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
    }
}