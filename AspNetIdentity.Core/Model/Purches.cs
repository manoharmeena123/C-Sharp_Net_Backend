using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class Purches : DefaultFields
    {
        [Key]
        public int OrderId { get; set; }

        public string OrderByName { get; set; }
        public int ItemId { get; set; }
        public PurchesItem CategoryID { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public string From { get; set; }
        public string PurchesDocUrl { get; set; }
        public int Units { get; set; }
        public DateTime? OrderDate { get; set; }
        public PaidByName PaidByEnum { get; set; }
        public string PaidBy { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
    }
}