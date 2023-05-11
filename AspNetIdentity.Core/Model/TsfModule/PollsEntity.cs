using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule
{
    public class PollsEntity : BaseModelClass
    {
        [Key]
        public Guid PollsId { get; set; } = Guid.NewGuid();
        public string PollsDescription { get; set; } = string.Empty;
    }
}
