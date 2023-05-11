using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork
{
    public class AssignWorktype : BaseModelClass
    {
        [Key]
        public Guid AssignWorktypeid { get; set; } = Guid.NewGuid();
        public Guid clientId { get; set; } = Guid.Empty;
        public Guid worktypeid { get; set; } = Guid.NewGuid();
    }
}