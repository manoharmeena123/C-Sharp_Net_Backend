using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Education : DefaultFields
    {
        [Key]
        public int EducationId { get; set; }

        public int CandidateId { get; set; }
        public string Degree { get; set; }
        public string BranchSpecialization { get; set; }
        public DateTime? DateOfJoining { get; set; } = null;
        public DateTime? DateOfCompletion { get; set; } = null;
        public string UniversityCollage { get; set; }
        public string Location { get; set; }
    }
}