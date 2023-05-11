using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class Project : DefaultFields
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<int> TechnologyId { get; set; }
        public int ProjectTypeId { get; set; }
        public int BillTypeId { get; set; }
        public decimal? EstimatedDays { get; set; }
        public int ClientId { get; set; }
        public int StatusId { get; set; }
        public string ProjectOwner { get; set; }
        public string Customer { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public double ProjectPrice { get; set; }
    }
}