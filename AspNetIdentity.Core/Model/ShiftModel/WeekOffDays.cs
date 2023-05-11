using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    /// <summary>
    /// Created By Harshit Mitra On 07-10-2022
    /// </summary>
    public class WeekOffDaysGroup : BaseModelClass
    {
        [Key]
        public Guid WeekOffId { get; set; } = Guid.NewGuid();
        public string WeekOffName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
    }
}