using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ProjectMaster
{
    public class NewProjectMaster : BaseModelClass
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; }
        public Guid ProjectManagerId { get; set; } = Guid.NewGuid();
        public string CampanyName { get; set; }
        public Guid TechnologyId { get; set; } = Guid.NewGuid();
        public string TechnologyName { get; set; }
        public string PaymentType { get; set; }
        public string ProjectDiscription { get; set; }
        public Guid LeadTypeId { get; set; } = Guid.NewGuid();
        public string LeadTypeName { get; set; }
        public string Others { get; set; }
        public int ClientBillableAmount { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double ClientAmount { get; set; }
        public double ClientConvertedAmt { get; set; }
        public DateTime ExchangeDate { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}