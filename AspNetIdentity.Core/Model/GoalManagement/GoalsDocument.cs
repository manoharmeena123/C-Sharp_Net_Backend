using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GoalManagement
{
    public class GoalsDocument : BaseModelClass
    {
        [Key]
        public Guid DocId { get; set; } = Guid.NewGuid();
        public Guid GoalId { get; set; } = Guid.NewGuid();
        public string DocumentTitle { get; set; } = string.Empty;
        public string FileURL { get; set; } = string.Empty;
        public string ExtensionType { get; set; } = string.Empty;
        public int GoalPercentage { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public DateTimeOffset DocDate { get; set; } = DateTimeOffset.UtcNow;
    }
}