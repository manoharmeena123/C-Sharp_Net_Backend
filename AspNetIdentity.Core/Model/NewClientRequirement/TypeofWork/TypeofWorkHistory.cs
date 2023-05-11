using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork
{
    public class TypeofWorkHistory : BaseModelClass
    {
        [Key]
        public Guid WorktypeHisoryId { get; set; } = Guid.NewGuid();
        public Guid WorktypeId { get; set; } = Guid.Empty;
        public string WorktypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // public string WorkTypeCode { get; set; } = string.Empty;
    }
}