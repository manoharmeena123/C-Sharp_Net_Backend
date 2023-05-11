using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Ctreted By Harshit Mitra on 01-02-2022
    /// </summary>
    public class JobPostTemplate : DefaultFields
    {
        [Key]
        public int TemplateId { get; set; }

        public string TemplateTitle { get; set; }
        public string TemplateDescription { get; set; }
    }
}