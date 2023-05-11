using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class Base
    {
        public string Message { get; set; }
        public bool StatusReason { get; set; }
        public int MyAvgScore { get; set; }
        public List<Project> projectData { get; set; }
        public List<User> UserData { get; set; }
        public List<BillType> billTypeData { get; set; }
        public List<ProjectType> projectTypeData { get; set; }
        public List<Status> statusData { get; set; }
        public List<ResourceCompany> resourceData { get; set; }
        public Project projectAssociation { get; set; }
        public User userAssociation { get; set; }
        public int totalProjects { get; set; }
        public int completedProjects { get; set; }
        public int activeProjects { get; set; }
        public int totalProjectManagers { get; set; }
        public int totalProjectCoordinator { get; set; }
    }
}