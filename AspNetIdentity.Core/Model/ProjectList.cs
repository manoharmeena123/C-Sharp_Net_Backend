using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    public class ProjectList : DefaultFields
    {
        [Key]
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public int ProjectManager { get; set; }
        public string CampanyName { get; set; }
        public int SubProjectId { get; set; } = 0;
        public string Technology { get; set; }
        public string PaymentType { get; set; }
        public string ProjectDiscription { get; set; }
        public string LeadType { get; set; }
        public string Others { get; set; }
        public int ClientBillableAmount { get; set; } //This is used as converted amount
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double ClientAmount { get; set; }
        public double ClientConvertedAmt { get; set; }
        public DateTimeOffset ExchangeDate { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsProjectManager { get; set; }
        public DateTimeOffset LastMailSendDate { get; set; }
        public ProjectStatusConstants ProjectStatus { get; set; }
        public string ProjectStatusDiscription { get; set; }
        public string ProjectAttachment { get; set; }
        public string LinkJson { get; set; }
    }
}