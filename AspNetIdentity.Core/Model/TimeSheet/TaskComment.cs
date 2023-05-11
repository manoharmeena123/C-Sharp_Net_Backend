using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.TimeSheet
{
    public class TaskComment : BaseModelClass
    {
        [Key]
        public Guid TaskCommentId { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; } = Guid.Empty;
        public int ProjectId { get; set; } = 0;
        public string Img1 { get; set; }
        public string Img2 { get; set; }
        public string Img3 { get; set; }
        public string Img4 { get; set; }
        public string Img5 { get; set; }
        public string Comments { get; set; }
    }
}