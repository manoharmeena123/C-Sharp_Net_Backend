using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ProjectExpCostAmt : DefaultFields
    {
        [Key]
        public int ExpCostAmtId { get; set; }

        public int ExpenseId { get; set; }
        public int ProjectId { get; set; }
        public string CostName { get; set; }
        public double Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}