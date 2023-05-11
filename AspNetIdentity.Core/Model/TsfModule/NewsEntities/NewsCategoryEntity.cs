using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule.NewsEntities
{
    /// <summary>
    /// Created By Harshit Mitra On 21-04-2023
    /// </summary>
    public class NewsCategoryEntity : BaseModelClass
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool InTrash { get; set; } = false;
    }
}
