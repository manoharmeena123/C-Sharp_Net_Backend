using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Update By Suraj Bundel On 30/05/2022
    /// Client tabel use in ClientController
    /// </summary>
    public class Client : DefaultFields
    {
        [Key]
        public int ClientId { get; set; } = 0;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string DisplayName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Gender { get; set; } = String.Empty;
        public string CompanyName { get; set; } = String.Empty;
        public string Website { get; set; } = String.Empty;
        public string AboutYourCompany { get; set; } = String.Empty;
        public string ClientCompanyLinkedInPage { get; set; } = String.Empty;
        public string ClientPersonalLinkedInPage { get; set; } = String.Empty;
        public int ExactNoResource { get; set; } = 0;
        public int NoOfResourceinEachTechnology { get; set; } = 0;
        public string BusinessPhone { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string MobilePhone { get; set; } = String.Empty;
        public string PostalCode { get; set; } = String.Empty;
        public int CountryName { get; set; } = 0;
        public int StateName { get; set; } = 0;
        public int CityName { get; set; } = 0;
        public string ImageUrl { get; set; } = String.Empty;
        public string IconImageUrl { get; set; } = String.Empty;
        public double DealPrice { get; set; }
        public bool AbleToClientLogin { get; set; } = false;
        public bool IsISAClient { get; set; } = false;
        public string Password { get; set; } = String.Empty;
        public bool IsClientLock { get; set; } = false;
        public string CompanyLogoURL { get; set; } = String.Empty;
        public int TurnOver { get; set; } = 0;
    }
    public enum ClientTechnologyConstants
    {
        Artificial_Intelligence_And_Machine_Learning = 1,
        Block_Chain_Internet_of_Things = 2,
        Cyber_Security = 3,
        Full_Stack_Development = 4,
        Cloud_Computing = 5,
        Data_Science = 6,
        AR_And_VR = 7,
        Dev_Ops = 8,
        Others = 9,
    }
}