using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Performence
{
    public class RequestFeedback : BaseModelClass
    {
        [Key]
        public int RequestFeedbackId { get; set; }
        public int RequestFeedbackFor { get; set; }
        public string RequestMessage { get; set; }
    }
}