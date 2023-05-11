using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class StageMaster : DefaultFields
    {
        [Key]
        public int StageId { get; set; }

        public int HiringFlowId { get; set; }
        public string StageName { get; set; }
        public StageFlowType StageType { get; set; }
        public bool SechduleRequired { get; set; }
        public DateTime? SechduleDateTime { get; set; }
        // public DateTime CreateDate { get; set; }
    }
}