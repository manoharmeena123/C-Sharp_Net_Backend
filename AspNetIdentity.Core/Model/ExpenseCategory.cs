using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Update by Suraj Bundel on 23/05/2022
    /// use in ExpenseCategory controller
    /// </summary>
    public class ExpenseCategory : DefaultFields
    {
        [Key]
        public int CategoryId { get; set; }

        public string IconImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public ExpenseTypeConstants ExpenseCategoryType { get; set; }
        public string ExpenseTitle { get; set; }

        //public ExpenseCurrentStatus Status { get; set; }
        public string Status { get; set; }

        public DateTime ExpenseDate { get; set; }
        public int CurrencyId { get; set; }
        public string Amount { get; set; }
        public string BillNumber { get; set; }
        public string MerchantName { get; set; }
        public string Comment { get; set; }
    }
}