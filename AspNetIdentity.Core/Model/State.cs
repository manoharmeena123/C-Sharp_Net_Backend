﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class State
    {
        [Key]
        public int StateId { get; set; }

        public int CountryId { get; set; }
        public string StateName { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}