using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.AssetsModel
{
    public class AsstesAssignHistory : DefaultFields
    {
        [Key]
        public int AssignHistroyId { get; set; }

        public int ItemId { get; set; }
        public int AssignToId { get; set; }
        public int? RecoverBy { get; set; }
        public string AssetCondition { get; set; }
        public string AssignImage1 { get; set; }
        public string AssignImage2 { get; set; }
        public string AssignImage3 { get; set; }
        public string AssignImage4 { get; set; }
        public string AssignImage5 { get; set; }
        public string RecoverImage1 { get; set; }    //add multiple image while recover
        public string RecoverImage2 { get; set; }    //add multiple image while recover
        public string RecoverImage3 { get; set; }    //add multiple image while recover
        public string RecoverImage4 { get; set; }    //add multiple image while recover
        public string RecoverImage5 { get; set; }    //add multiple image while recover
    }
}