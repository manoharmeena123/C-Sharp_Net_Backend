using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    public class ESISetting : BaseModelClass
    {
        [Key]
        public Guid ESISetingId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public bool IsESIStatus { get; set; } = false;
        public double EligibleAmountForESI { get; set; } = 0.0;
        public double MinESIEmpContributionOfGross { get; set; } = 0.0;
        public double MaxESIEmpContributionOfGross { get; set; } = 0.0;
        public bool IsAllowESIatSalary { get; set; } = false;
        public bool IsPayESIEmpOutsideGross { get; set; } = false;
        public bool IsHideESIEmpPaySlip { get; set; } = false;
        public bool IsExcludeEmpShareFromGrossESI { get; set; } = false;
        public bool IsExcludeEmpGratutyFromGrossESI { get; set; } = false;
        public bool IsRestrictESIGrossDuringContribution { get; set; } = false;
        public bool IsIncludeBonusandOneTimePaymentForESIEligibility { get; set; } = false;
        public bool IsIncludeBonusandOneTimePaymentForESIContribution { get; set; } = false;
    }
}