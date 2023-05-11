using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace AspNetIdentity.Core.Model.TsfModule
{
    public class BlogCategories  :BaseModelClass
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
