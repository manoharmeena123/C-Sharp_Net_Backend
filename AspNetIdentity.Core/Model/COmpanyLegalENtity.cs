using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Updated By Harshit Mitra on 30-05-2022
    /// </summary>
    public class CompanyLegalEntity : BaseModelClass
    {
        [Key]
        public Guid LegalEntityId { get; set; } = Guid.NewGuid();

        public string EntityName { get; set; } = String.Empty;
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
        public string Logo { get; set; } = String.Empty;
    }
}