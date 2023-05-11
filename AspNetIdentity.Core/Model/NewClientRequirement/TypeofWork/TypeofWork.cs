using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork
{
    public class TypeofWork : BaseModelClass
    {
        [Key]
        public Guid WorktypeId { get; set; } = Guid.NewGuid();
        public string WorktypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //public string WorkTypeCode { get; set; } = string.Empty;

    }
}