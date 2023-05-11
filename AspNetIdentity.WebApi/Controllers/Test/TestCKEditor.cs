using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Controllers.Test
{
    public class TestCKEditor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StringHTML { get; set; } = String.Empty;
    }
}