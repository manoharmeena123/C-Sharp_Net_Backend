using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.GiftsModel
{
    public class GiftAssign : DefaultFields
    {
        [Key]
        public int GiftAssignId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int GItemId { get; set; }
        public string GItem { get; set; }
        public int ItemCategory { get; set; }
    }
}