using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 20/12/2022
    /// </summary>
    public class StatutoryFlling : BaseModelClass
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public string PanNo { get; set; } = String.Empty;
        public string TanNo { get; set; } = String.Empty;
        public string TanCircleNo { get; set; } = String.Empty;
        public string CITLocation { get; set; } = String.Empty;
        public string Signatory { get; set; } = String.Empty;
        public string EsiName { get; set; } = String.Empty;
        public string PfName { get; set; } = String.Empty;
        public DateTimeOffset? PfRegistationDate { get; set; }
        public DateTimeOffset? EsiRegistationDate { get; set; }
        public DateTimeOffset? PtRegistationDate { get; set; }
        public string PFRegistationNo { get; set; } = String.Empty;
        public string SignatoryDesignation { get; set; } = String.Empty;
        public string SignatoryFatherName { get; set; } = String.Empty;
        public string ESIRegistationNo { get; set; } = String.Empty;
        public string EstablishmentId { get; set; } = String.Empty;
        public int StateId { get; set; } = 0;
    }
}