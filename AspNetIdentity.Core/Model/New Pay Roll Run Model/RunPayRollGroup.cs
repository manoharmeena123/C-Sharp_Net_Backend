using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll_Run_Model
{
    /// <summary>
    /// Created By Harshit Mitra On 23/12/2022
    /// </summary>
    public class RunPayRollGroup : BaseModelClass
    {
        [Key]
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public Guid PayGroupId { get; set; } = Guid.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalAmountPay { get; set; } = 0.0;
        public long EmployeeCount { get; set; } = 0;
        public string RunCount { get; set; } = String.Empty;
        public PayRollRunTypeConstants RunType { get; set; }
    }
}