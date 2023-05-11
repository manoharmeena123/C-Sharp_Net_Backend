using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ProjectExpenseMaster : DefaultFields
    {
        [Key]
        public int ProjectExpId { get; set; }

        public int ProjectId { get; set; }
        public string ExpenseName { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ExpenseInvoice { get; set; }
    }
}