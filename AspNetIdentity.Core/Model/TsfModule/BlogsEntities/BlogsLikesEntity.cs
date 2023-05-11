using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Core.Model.TsfModule
{
    /// <summary>
    /// Created By Harshit Mitra On 12-04-2023
    /// </summary>
    public class BlogsLikesEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BlogId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; } = 0;
    }
}
