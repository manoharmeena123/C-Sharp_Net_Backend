using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class RatingScales : BaseModelClass
    {
        [Key]
        public Guid RatingScalesId { get; set; } = Guid.NewGuid();
        public string RatingScalesName { get; set; }

    }
}