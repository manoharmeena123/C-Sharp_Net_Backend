using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Header
{
    public class ThirdHeader : DefaultFields
    {
        [Key]
        public int ThirdHeaderId { get; set; }
        public int NavigationId { get; set; }
        public int SecondHeaderId { get; set; }
        public string ContentName { get; set; }
        public string RichText { get; set; }
        public int? PolicyGroupId { get; set; }
        public string URL { get; set; }

    }
}