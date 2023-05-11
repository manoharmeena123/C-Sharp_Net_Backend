using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 21-02-2022
    /// </summary>
    public class CompanyInformation : BaseModelClass
    {
        [Key]
        public Guid CompanyInfoId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; }
        public string SignatoryUrl { get; set; } = String.Empty;
        public string LegalNameOfCompany { get; set; } = String.Empty;
        public string CompanyIdentifyNumber { get; set; } = String.Empty;
        public DateTimeOffset DateOfIncorporation { get; set; } = DateTimeOffset.UtcNow;
        public int TypeOfBusinessId { get; set; } = 0;
        public int SectorId { get; set; } = 0;
        public int NatureOfBusinessId { get; set; } = 0;
        public string AddressLine1 { get; set; } = String.Empty;
        public string AddressLine2 { get; set; } = String.Empty;
        public int CityId { get; set; } = 0;
        public int StateId { get; set; } = 0;
        public string ZipCode { get; set; } = String.Empty;
        public int CountryId { get; set; } = 0;
    }
}