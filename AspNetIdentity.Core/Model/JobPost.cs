using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 01-02-2022
    /// </summary>
    public class JobPost : DefaultFields
    {
        [Key]
        public int JobPostId { get; set; }

        public string JobTitle { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public bool IsPriority { get; set; }
        public string JobDescription { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int Offer { get; set; }
        public int Openings { get; set; }
        public DateTimeOffset? TargetHireDate { get; set; }
        public decimal SaleryStRange { get; set; }
        public decimal SaleryEdRange { get; set; }
        public string SaleryRange { get; set; } = string.Empty;
        public string SaleryEndRange { get; set; } = string.Empty;
        public string JobType { get; set; }
        public int HiringFlow { get; set; }
        public decimal Experience { get; set; }
        public decimal ExperienceMax { get; set; }
        public string MinExperience { get; set; } = string.Empty;
        public string MaxExperience { get; set; } = string.Empty;
        public bool IsExtended { get; set; }
        public int ExtendedDays { get; set; }
        public bool PublishToCareers { get; set; }
        public bool PublishToPortal { get; set; }
        public JobCategory JobCategory { get; set; }
        public JobPriorityHelperConstants PriorityId { get; set; }
        public string PriorityName { get; set; }

        // Not Maped Fields That Not Save in Data Base//
        [NotMapped]
        public bool IsPublished { get; set; }

        public string OrgName { get; set; }
        public bool ConfidentialSalary { get; set; } = false;
        public bool CompetativeSalary { get; set; } = false;
    }

}