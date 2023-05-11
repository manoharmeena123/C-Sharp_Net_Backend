using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class RatingScore : BaseModelClass
    {
        [Key]
        public Guid RatingscalescoreId { get; set; } = Guid.NewGuid();
        public Guid? RatingScalesId { get; set; } = Guid.Empty;
        public int RatingScaleScore { get; set; }
        public string RatingLable { get; set; }
        public string Description { get; set; }
    }
}