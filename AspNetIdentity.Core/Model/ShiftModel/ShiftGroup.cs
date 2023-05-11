using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    /// <summary>
    /// Created By Harshit Mitra On 29-09-2022
    /// </summary>
    public class ShiftGroup : BaseModelClass
    {
        [Key]
        public Guid ShiftGoupId { get; set; } = Guid.NewGuid();
        public string ShiftName { get; set; } = String.Empty;
        public string ShiftCode { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsFlexible { get; set; } = false;
        public bool IsTimingDifferent { get; set; } = false;
        public bool IsDurationDifferent { get; set; } = false;
        public bool IsDefaultShiftGroup { get; set; } = false;

    }
}