using AspNetIdentity.WebApi.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace AspNetIdentity.WebApi.Services
{
    public class EmployeeService
    {
        public ApplicationDbContext db;

        public EmployeeService()
        {
            db = new ApplicationDbContext();
        }

        public double GetTotalBillableCostByProjectId(int id)
        {
            double total = sumOfSalary(GetEmpListByProjectBill(id));
            return total;
        }

        public List<int> GetEmpListByProjectBill(int id)
        {
            List<int> empList = new List<int>();

            var emp = db.AssignProjects.Where(e => e.ProjectId == id && e.Status == "Billable").ToList();
            empList.AddRange(emp.Select(m => m.EmployeeId));
            return empList;
        }

        public double GetTotalNonBillableCostByProjectId(int id)
        {
            double total = sumOfSalary(GetEmpListByProjectNonBill(id));
            return total;
        }

        public List<int> GetEmpListByProjectNonBill(int id)
        {
            List<int> empList = new List<int>();

            var emp = db.AssignProjects.Where(e => e.ProjectId == id && e.Status == "Non-Billable").ToList();
            empList.AddRange(emp.Select(m => m.EmployeeId));
            return empList;
        }

        public int sumOfSalary(List<int> id)
        {
            double total = 0;
            foreach (var x in id)
            {
                var emp = db.Employee.Where(e => e.EmployeeId == x).FirstOrDefault();
                if (emp.GrossSalery != 0)
                {
                    total += emp.GrossSalery;
                }
                else
                {
                    total += 0;
                }
            }
            return (int)total;
        }
    }
}