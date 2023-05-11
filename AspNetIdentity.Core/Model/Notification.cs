using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra On 19-08-2022
    /// </summary>
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }

        [NotMapped]
        public bool ForPC { get; set; }

        //public string Url { get; set; }
    }

    /// <summary>
    /// Created By Harshit Mitra On 19-08-2022
    /// </summary>
    public class FireBase
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string IMEI { get; set; }
        public string AndroidVersion { get; set; }
        public string SIMOperator { get; set; }
        public string SIMNumber { get; set; }
        public string SIMState { get; set; }
        public string SIMCountry { get; set; }
        public string FCMToken { get; set; }
        public string Brand { get; set; }
        public string PhoneModel { get; set; }
        public string Manufacture { get; set; }
        public string PCFCMToken { get; set; }
    }
}