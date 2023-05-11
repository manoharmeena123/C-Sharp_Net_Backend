using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class GoalComment : BaseModelClass
    {
        [Key]
        public Guid CommentId { get; set; } = Guid.NewGuid();
        public Guid GoalId { get; set; } = Guid.NewGuid();
        public string Message { get; set; } = string.Empty;
        public string CommentBy { get; set; } = string.Empty;
        public string CommentOn { get; set; } = string.Empty;
    }
}