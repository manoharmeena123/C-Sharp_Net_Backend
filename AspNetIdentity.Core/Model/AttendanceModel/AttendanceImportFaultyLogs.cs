using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AttendanceModel
{
    public class AttendanceImportFaultyLogs
    {
        [Key]
        public Guid AttendanceFaultyId { get; set; }
        public virtual AttendanceImportFaultyLogsGroup AttendanceGroups { get; set; }
        public int AttendencereportId { get; set; }
        public string One { get; set; }
        public string Two { get; set; }
        public string Three { get; set; }
        public string Four { get; set; }
        public string Five { get; set; }
        public string Six { get; set; }
        public string Seven { get; set; }
        public string Eight { get; set; }
        public string Nine { get; set; }
        public string Ten { get; set; }
        public string Eleven { get; set; }
        public string Twelve { get; set; }
        public string Thirteen { get; set; }
        public string Fourteen { get; set; }
        public string Fifteen { get; set; }
        public string Sixteen { get; set; }
        public string Seventeen { get; set; }
        public string Eighteen { get; set; }
        public string Nineteen { get; set; }
        public string Twenty { get; set; }
        public string Twentyone { get; set; }
        public string Twentytwo { get; set; }
        public string Twentythree { get; set; }
        public string Twentyfour { get; set; }
        public string Twentyfive { get; set; }
        public string Twentysix { get; set; }
        public string Twentyseven { get; set; }
        public string Twentyeight { get; set; }
        public string Twentynine { get; set; }
        public string Thirty { get; set; }
        public string Thirtyone { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int EmployeeId { get; set; }
        public string OfficeEmail { get; set; }
        public string FailReason { get; set; }
    }
}