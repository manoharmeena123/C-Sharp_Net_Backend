using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule
{
    public class NewsEntity : BaseModelClass
    {
        [Key]
        public Guid NewsId { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool IsFeatured { get; set; } = false;
        public DateTimeOffset FeaturedOn { get; set; } 
        public NewsEnumType NewsType { get; set; } = NewsEnumType.Drafted;
    }
}
