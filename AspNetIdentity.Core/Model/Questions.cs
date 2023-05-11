using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class Questions
    {
        [Key]
        public int QuestionId { get; set; }

        public string Question { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        // public string Category_Type { get; set; }

        //public string CategoryName { get; set; }

        [NotMapped]
        public int ScoreRating { get; set; }

        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}