using AspNetIdentity.WebApi.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TicketsModel
{
    /// <summary>
    /// Created By Avani Shrivastava on 31-03-2023
    /// </summary>
    public class TicketCategoryEntity : BaseModelClass
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
