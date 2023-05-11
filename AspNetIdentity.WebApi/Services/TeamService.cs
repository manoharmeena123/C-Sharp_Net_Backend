using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System.Collections.Generic;
using System.Linq;
using static AspNetIdentity.WebApi.Controllers.ProjectController;

namespace AspNetIdentity.WebApi.Services
{
    public class TeamService
    {
        public ApplicationDbContext db;

        public TeamService()
        {
            db = new ApplicationDbContext();
        }

        public List<Team> GetAllTeam()
        {
            return db.Team.ToList();
        }

        public List<Team> FilterTeamByEmp(int id)
        {
            return db.Team.Where(t => t.TeamMemberId == id).ToList();
        }

        public List<Employee> GetEmployees()
        {
            try
            {
                var dbm = new ApplicationDbContext();
                List<Employee> emplist = new List<Employee>();
                emplist = db.Employee.ToList();
                return emplist;
            }
            catch
            {
                throw;
            }
        }

        public List<TeamData> GetLeadsByEmp(int empId)
        {
            var empData = GetEmployees();
            var empList = empData.Where(ad => ad.IsDeleted == false && ad.EmployeeId == empId).ToList();
            List<TeamData> TeamData = new List<TeamData>();
            foreach (var t in empList)
            {
                var team = db.TeamType.Where(y => y.TeamTypeId == t.TeamTypeId).FirstOrDefault();
                if (team != null)
                {
                    var teamLeads = db.Employee.Where(e => e.EmployeeId == team.TeamLeadId).FirstOrDefault();
                    if (teamLeads != null)
                    {
                        var result = new TeamData();
                        result.EmployeeId = teamLeads.EmployeeId;
                        result.FullName = teamLeads.FirstName + ' ' + teamLeads.LastName;
                        TeamData.Add(result);
                    }
                }
            }
            return TeamData;
        }
    }
}