﻿using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewDashboard
{
    public class CompanyPolicyHistory : DefaultFields
    {
        [Key]

        public int CompanyPolicyHistoryId { get; set; }
        public int PolicyId { get; set; }
        public int PolicyGroupId { get; set; }
        public string PolicyName { get; set; }
        public string Link { get; set; }
        public string PolicyDiscriyption { get; set; }
    }
}