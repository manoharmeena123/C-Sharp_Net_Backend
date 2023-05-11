using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class ReviewersInGoal : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GoalId { get; set; } = Guid.NewGuid();
        public int ReviewerId { get; set; } = 0;
    }
}