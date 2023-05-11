using System;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll_Run_Model
{
    public class RunPayRoll
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
        public double EarningsAmount { get; set; }
        public double DeductionsAmount { get; set; }
        public double OtherAmount { get; set; }
        public double BonusAmount { get; set; }
        public double MonthlyTotal { get; set; }
        public string ComponentCheck { get; set; } = String.Empty;
    }
}