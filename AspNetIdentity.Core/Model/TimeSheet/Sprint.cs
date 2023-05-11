using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class Sprint : BaseModelClass
    {
        [Key]

        public Guid SprintId { get; set; } = Guid.NewGuid();
        public string SprintName { get; set; }
        public int ProjectId { get; set; }
        public string SprintDescription { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public SprintStatusConstant SprintStatus { get; set; }
    }
    public enum SprintStatusConstant
    {
        Active = 1,
        Closed = 2,
        Draft = 3,
    }
}