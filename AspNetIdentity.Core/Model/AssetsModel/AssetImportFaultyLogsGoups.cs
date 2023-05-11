using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AssetImportFaultyLogsGoups : DefaultFields
    {
        [Key]
        public Guid GroupId { get; set; }
        public long TotalImported { get; set; }
        public long SuccessFullImported { get; set; }
        public long UnSuccessFullImported { get; set; }
    }
}