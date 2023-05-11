using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 15/12/2022
    /// </summary>
    public class CompanyInfoBank : BaseModelClass
    {
        [Key]
        public Guid InfoBanksId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public int BankId { get; set; } = 0;
    }
}