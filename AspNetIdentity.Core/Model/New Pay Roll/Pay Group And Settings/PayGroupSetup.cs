using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 21-02-2022
    /// </summary>
    public class PayGroupSetup
    {
        [Key]
        public Guid SetupId { get; set; } = Guid.NewGuid();
        [Required]
        public Guid PayGroupId { get; set; } = Guid.Empty;
        public PayRollSetupConstants StepsInSettings { get; set; }
        public bool IsSetupComplete { get; set; } = false;
    }
}