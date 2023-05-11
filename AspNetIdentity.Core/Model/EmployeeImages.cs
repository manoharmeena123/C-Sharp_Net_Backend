using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra On 30-01-2023
    /// </summary>
    public class EmployeeImages
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int EmployeeId { get; set; } = 0;
        public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;
        public string ImagesUrl { get; set; } = String.Empty;
    }
}