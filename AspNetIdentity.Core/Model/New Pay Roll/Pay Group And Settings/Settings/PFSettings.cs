using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    public class PFSettings : BaseModelClass
    {
        [Key]
        public Guid PFSettingId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public bool IsPFRequired { get; set; } = false;
        public double MinimumPFAmount { get; set; } = 0.0;
        public PfCalculationType PfCalculationType { get; set; } = PfCalculationType.Not_Selected;
        public bool IsAllowOverridingOfPf { get; set; } = false;
        public bool IsPayEmployeePFOutsideGross { get; set; } = false;
        public bool IsLimitEmpPFMaxAmount { get; set; } = false;
        public double LimitEmpPFMaxAmountMonthly { get; set; } = 0.0;
        public bool IsHidePFEmpInPaySlip { get; set; } = false;
        public bool IsPayOtherChargesOutsideGross { get; set; } = false;
        public bool IsHideOtherChargesPFPaySlip { get; set; } = false;
        public bool IsEmpContributeVPF { get; set; } = false;
        public bool Is1poin16PerShareOfPension { get; set; } = false;
        public bool IsAdminToOveridePF { get; set; } = false;
    }
}