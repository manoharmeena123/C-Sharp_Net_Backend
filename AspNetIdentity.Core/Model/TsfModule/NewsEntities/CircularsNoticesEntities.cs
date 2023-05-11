using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule.CircularsNotices
{
    public class CircularsNoticesEntities : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
        public CircularsNoticesType Type { get; set; } 
    }
    public enum CircularsNoticesType
    {
        Circulars = 1,
        Notices = 2,
    }
}
