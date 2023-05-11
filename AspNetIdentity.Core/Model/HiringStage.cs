using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra On 30-09-2022
    /// </summary>
    public class HiringStage : DefaultFields
    {
        [Key]
        public Guid StageId { get; set; }
        public virtual JobPost Job { get; set; }
        public string StageName { get; set; }
        public StageFlowType StageType { get; set; }
        public int StageOrder { get; set; }
        public bool SechduleRequired { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? SechduleDateTime { get; set; }
    }
}