using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class ReviewCoreValue : BaseModelClass
    {
        [Key]
        public Guid ReviewCoreValueId { get; set; } = Guid.NewGuid();
        public string ReviewCoreValueName { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }


        [NotMapped]
        public List<Behaviour> Behaviours { get; set; }
    }
}