using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Performence;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Performence_Controller
{
    [Authorize]
    [RoutePrefix("api/Funtion")]
    public class JobFuntionController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region This Api Use Job Funtion

        #region This Api Use Add Job Funtion
        /// <summary>
        /// API >> Post >>api/Funtion/addjobfunction
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addjobfunction")]
        public async Task<IHttpActionResult> AddJobFunction(RequestBodyForAddJob model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            JobFunctionCompencies Obj = new JobFunctionCompencies();
            try
            {
                JobFuntion obj = new JobFuntion();
                obj.JobFuntionName = model.JobFuntionName;
                obj.JobLevelsEnumId = model.JobLevelsEnumId;
                obj.Title = model.Title;
                obj.Description = model.Description;
                obj.CompanyId = tokenData.companyId;
                obj.OrgId = tokenData.orgId;
                obj.CreatedBy = tokenData.employeeId;
                _db.JobFuntions.Add(obj);
                await _db.SaveChangesAsync();

                if (model.Competencies == null)
                {
                    foreach (var item in model.Competencies)
                    {
                        JobFunctionCompencies obj3 = new JobFunctionCompencies();
                        obj3.JobFuntionId = obj.JobFuntionId;
                        obj3.CompentenciesId = item.CompetenciesId;
                        obj3.Weight = item.Weight;
                        obj3.CoreName = item.CoreName;
                        obj3.IsActive = true;
                        obj3.IsDeleted = false;
                        obj3.CreatedBy = tokenData.employeeId;
                        obj3.CreatedOn = DateTime.Now;
                        obj3.CompanyId = tokenData.companyId;
                        obj3.OrgId = tokenData.orgId;
                        _db.JobFunctionCompencies.Add(obj3);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    JobFunctionCompencies obj3 = new JobFunctionCompencies();
                    obj3.JobFuntionId = obj.JobFuntionId;
                    obj3.IsActive = true;
                    obj3.IsDeleted = false;
                    obj3.CreatedBy = tokenData.employeeId;
                    obj3.CreatedOn = DateTime.Now;
                    obj3.CompanyId = tokenData.companyId;
                    obj3.OrgId = tokenData.orgId;
                    _db.JobFunctionCompencies.Add(obj3);
                    await _db.SaveChangesAsync();
                }
                foreach (var item2 in model.Department)
                {
                    DepartmentJobFuntion obj2 = new DepartmentJobFuntion();
                    obj2.DepartmentJobFuntionName = _db.Department.Where(x => x.DepartmentId == item2.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault();
                    obj2.JobFuntionId = obj.JobFuntionId;
                    obj2.DepartmentId = item2.DepartmentId;
                    obj2.CreatedBy = tokenData.employeeId;
                    obj2.IsActive = true;
                    obj2.IsDeleted = false;
                    obj2.CreatedOn = DateTime.Now;
                    obj2.CompanyId = tokenData.companyId;
                    obj2.OrgId = tokenData.orgId;
                    _db.DepartmentJobFuntions.Add(obj2);
                    await _db.SaveChangesAsync();
                }
                if (model != null)
                {
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = model;

                }
                else
                {
                    res.Message = "Not Add Job Funtion ";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/addjobfunction", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RequestBodyForAddJob
        {
            public string JobFuntionName { get; set; }
            public JobLevelsConstants JobLevelsEnumId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public List<CompetenciesHelper> Competencies { get; set; }
            public List<DepartmentIdResponse> Department { get; set; }
        }


        public class CompetenciesHelper
        {
            public Guid CompetenciesId { get; set; }
            public int Weight { get; set; }
            public string CoreName { get; set; }
        }
        public class DepartmentIdResponse
        {
            public int DepartmentId { get; set; }
        }
        #endregion

        #region This Api Use Get All Job Funtion Data Get By Id
        /// <summary>
        /// API >> Get >>api/Funtion/getalldatajobfuntion
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldatajobfuntion")]
        public async Task<IHttpActionResult> GetAllDataJobFuntion(int depatmentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var jobFuntiondata = (from a in _db.JobFuntions
                                      join d in _db.DepartmentJobFuntions on a.JobFuntionId equals d.JobFuntionId
                                      where (a.IsActive && !a.IsDeleted && a.JobFuntionId == d.JobFuntionId && d.DepartmentId == depatmentId)
                                      select new JobFunctionNameResponse
                                      {
                                          JobFuntionId = a.JobFuntionId,
                                          JobFuntionName = a.JobFuntionName,
                                          DepartmentId = d.DepartmentId,
                                          DepartmentJobFuntionName = d.DepartmentJobFuntionName
                                      }).ToList();

                if (jobFuntiondata != null)
                {
                    res.Message = "Get Job Funtion Successfully !";
                    res.Status = true;
                    res.Data = jobFuntiondata;
                }
                else
                {
                    res.Message = "Job Funtion Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/getalldatajobfuntion", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetJobData
        {
            public string JobFuntionName { get; set; }
            public List<DepartmentIdResponse> Department { get; set; }
            public string DepartmentJobFuntionName { get; set; }
        }
        public class JobFunctionNameResponse
        {
            public Guid JobFuntionId { get; set; }
            public string JobFuntionName { get; set; }
            public int? DepartmentId { get; set; }
            public int Count { get; set; }
            public string DepartmentJobFuntionName { get; set; }
        }
        #endregion

        #region This Api Use Get Job Funtion By Id
        /// <summary>
        /// API >> Get >>api/Funtion/getalljobfuntionbyid
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalljobfuntionbyid")]
        public async Task<IHttpActionResult> GetAllJobFuntionById(Guid JobFuntionId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var jobFuntiondata = _db.JobFuntions.Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new GetJobFuntionData
                {
                    JobFuntionId = x.JobFuntionId,
                    Title = x.Title,
                    JobFuntionName = x.JobFuntionName,
                    Description = x.Description,
                    Weight = _db.JobFunctionCompencies.Where(j => j.JobFuntionId == x.JobFuntionId).Select(j => j.Weight).FirstOrDefault(),
                    CoreName = _db.JobFunctionCompencies.Where(j => j.JobFuntionId == x.JobFuntionId).Select(j => j.CoreName).FirstOrDefault(),
                    DepartmentJobFuntionName = _db.DepartmentJobFuntions.Where(d => d.JobFuntionId == x.JobFuntionId).Select(d => d.DepartmentJobFuntionName).FirstOrDefault()
                }).ToList();

                if (jobFuntiondata != null)
                {
                    res.Message = "Get Job Funtion SuccessFully";
                    res.Status = true;
                    res.Data = jobFuntiondata;
                }
                else
                {
                    res.Message = "Job Funtion Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/getalljobfuntionbyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetJobFuntionData
        {
            public Guid JobFuntionId { get; set; }
            public string JobFuntionName { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string CompetenciesName { get; set; }
            public int Weight { get; set; }
            public string CoreName { get; set; }
            public string DepartmentJobFuntionName { get; set; }
        }
        #endregion

        #region This Api Use Update Job Funtion
        /// <summary>
        /// API >> Put >>api/Funtion/updatejobfuntion
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        ///
        [HttpPut]
        [Route("updatejobfuntion")]
        public async Task<IHttpActionResult> UpdateJobFuntion(JobFuntion model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                JobFuntion Update = new JobFuntion();
                Update.JobFuntionName = model.JobFuntionName;
                Update.DepartmentId = model.DepartmentId;
                Update.JobLevelsEnumId = model.JobLevelsEnumId;
                Update.Title = model.Title;
                Update.Description = model.Description;


                _db.JobFuntions.Add(Update);
                await _db.SaveChangesAsync();

                foreach (var item in model.JobFuntionName)
                {
                    JobFunctionCompencies obj = new JobFunctionCompencies();
                    obj.JobFuntionId = obj.JobFuntionId;
                    obj.CompentenciesId = obj.CompentenciesId;
                    Update.DepartmentId = model.DepartmentId;

                    _db.JobFunctionCompencies.Add(obj);
                    await _db.SaveChangesAsync();
                }
                if (model != null)
                {
                    res.Message = "Updated Job Funtion Successfully";
                    res.Status = true;
                    res.Data = model;
                }
                else
                {
                    res.Message = "Job Funtion Not Updated";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/updatejobfuntion", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class RequestBodyAddJobFuntion
        {
            public Guid JobFuntionId { get; set; }
            public string JobFuntionName { get; set; }
            public string DepartmentName { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid? JobLevelsEnumId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public List<JobCompetenciesHelperModel> CompetenciesIdlist { get; set; }
        }
        public class JobCompetenciesHelperModel
        {
            public Guid JobFunctionCompenciesId { get; set; }
            public Guid? DepartmentId { get; set; }
            public Guid? CompentenciesId { get; set; }
            public Guid? JobFuntionId { get; set; }
            public string CompentenciesName { get; set; }
        }
        #endregion

        #region This Api Use Deleted Job Function
        /// <summary>
        /// API >> Delete >>api/Reviews/jobfuntiondelete
        ///  Created by Mayank Prajapati On 3/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("jobfuntiondelete")]
        public async Task<IHttpActionResult> JobFuntionDelete(Guid JobFuntionId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobfuntion = await _db.JobFuntions.FirstOrDefaultAsync(x =>
                    x.JobFuntionId == JobFuntionId && x.IsActive && !x.IsDeleted);
                if (jobfuntion != null)
                {
                    jobfuntion.IsDeleted = true;
                    jobfuntion.IsActive = false;
                    jobfuntion.DeletedBy = tokenData.employeeId;
                    jobfuntion.DeletedOn = DateTime.Now;

                    _db.Entry(jobfuntion).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Job Funtion Deleted Successfully!";
                    res.Data = jobfuntion;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Reviews/jobfuntiondelete", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use for Add Weightage
        /// <summary>
        /// created by Mayank Prajapati on 22/11/2022
        /// Api >> Post >>api/Funtion/addweightage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addweightage")]
        public async Task<IHttpActionResult> AddWeightAge(WeightAge model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weightagedata = _db.WeightAges.Where(x => x.EmployeeId == model.EmployeeId
                && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (weightagedata == null)
                {
                    WeightAge obj = new WeightAge();
                    {
                        obj.EmployeeId = model.EmployeeId;
                        obj.WeightAgeId = model.WeightAgeId;
                        obj.WeightAgee = model.WeightAgee;
                        obj.Weightage1 = model.Weightage1;
                        obj.DepartmentId = model.DepartmentId;
                        obj.DepartmentName = _db.DepartmentJobFuntions.Where(x => x.DepartmentId == obj.DepartmentId).Select(x => x.DepartmentJobFuntionName).FirstOrDefault();
                        obj.PrimaryJobFunction = model.PrimaryJobFunction;
                        obj.SecondaryJobFunction = model.SecondaryJobFunction;
                        obj.CreatedBy = tokenData.employeeId;
                        obj.CompanyId = tokenData.companyId;
                        obj.OrgId = tokenData.orgId;
                        obj.CreatedOn = DateTime.Now;
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                    }
                    _db.WeightAges.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "Created Successfully  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;

                }
                else
                {
                    res.Message = "Job Function Already Assigned";
                    res.Status = true;
                    res.Data = weightagedata;
                }

            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/addweightage", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Get Job Funtion Departmen Data
        /// <summary>
        /// API >> Get >>api/Funtion/getjobfuntiondepartment
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobfuntiondepartment")]
        public async Task<IHttpActionResult> GetJobFuntionDepartment()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobFunctionInDepartment = await (from d in _db.DepartmentJobFuntions
                                                     join de in _db.Department on d.DepartmentId equals de.DepartmentId
                                                     join jf in _db.JobFuntions on d.JobFuntionId equals jf.JobFuntionId
                                                     where d.IsActive && !d.IsDeleted && d.CompanyId == tokenData.companyId
                                                     select new
                                                     {
                                                         de.DepartmentId,
                                                         de.DepartmentName,
                                                         jf.JobFuntionId,
                                                         jf.JobFuntionName,

                                                     }).ToListAsync();
                var departmentList = jobFunctionInDepartment.Select(x => x.DepartmentId).Distinct().ToList();
                var wetightage = await _db.WeightAges
                    .Where(x => departmentList.Contains(x.DepartmentId) && x.IsActive && !x.IsDeleted)
                    .ToListAsync();
                var response = jobFunctionInDepartment
                    .Select(x => new
                    {
                        x.DepartmentId,
                        x.DepartmentName,
                    })
                    .OrderBy(z => z.DepartmentName)
                    .Distinct()
                    .Select(x => new
                    {
                        x.DepartmentId,
                        x.DepartmentName,
                        JobFunctions = jobFunctionInDepartment
                            .Where(z => z.DepartmentId == x.DepartmentId)
                            .Select(z => new
                            {
                                z.JobFuntionId,
                                z.JobFuntionName,
                                EmployeeCount = wetightage
                                    .Count(y => (y.PrimaryJobFunction == z.JobFuntionId ||
                                    y.SecondaryJobFunction == z.JobFuntionId) && y.DepartmentId == x.DepartmentId),
                            })
                            .OrderBy(z => z.JobFuntionName)
                            .ToList(),
                    })
                    .ToList();


                if (response != null)
                {
                    res.Status = true;
                    res.Message = "Job list Get Successfully";
                    res.Data = response;
                }
                else
                {
                    res.Message = "Job list Not Found";
                    res.Status = false;
                    res.Data = response;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/sprint/getsprint", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class JobFuntionModelHelper
        {
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public int EmployeeCount { get; set; }
            public List<JobFunctionDepartmentHelpermodeul> JobFeedBack { get; set; }
        }
        public class JobFunctionDepartmentHelpermodeul
        {
            public int EmployeeId { get; set; }
            public Guid JobFuntionId { get; set; }
            public string JobFuntionName { get; set; }
            public string EmployeeName { get; set; }
        }
        #endregion

        #region This Api Use Get Weight Age By Id
        /// <summary>
        /// API >> Get >>api/Funtion/getweightagebyid
        ///  Created by  Mayank Prajapati On 01/12/2022
        /// <summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getweightagebyid")]
        public async Task<IHttpActionResult> GetWeightAgeById(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var weightage = await _db.WeightAges.Where(x => x.IsActive && !x.IsDeleted
                      && x.EmployeeId == employeeId && x.CompanyId == tokenData.companyId)
                    .Select(x => new WeightHelperModel
                    {
                        WeightAgeId = x.WeightAgeId,
                        WeightAgee = x.WeightAgee,
                        DepartmentName = _db.DepartmentJobFuntions.Where(y => y.DepartmentId == x.DepartmentId).Select(y => y.DepartmentJobFuntionName).FirstOrDefault(),
                        EmployeeId = x.EmployeeId,
                        EmployeeName = _db.Employee.Where(z => z.EmployeeId == x.EmployeeId).Select(z => z.DisplayName).FirstOrDefault(),
                        Weightage1 = x.Weightage1,
                        JobFuntionName = _db.JobFuntions.Where(j => j.JobFuntionId == x.PrimaryJobFunction).Select(j => j.JobFuntionName).FirstOrDefault(),
                        SecondJobFuntionName = _db.JobFuntions.Where(j => j.JobFuntionId == x.SecondaryJobFunction).Select(j => j.JobFuntionName).FirstOrDefault(),

                    }).FirstOrDefaultAsync();
                if (weightage != null)
                {
                    res.Message = "Weight Age Get SuccessFully";
                    res.Status = true;
                    res.Data = weightage;
                }
                else
                {
                    res.Message = "Weight Age not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/getweightagebyid", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class WeightHelperModel
        {
            public int WeightAgeId { get; set; }
            public int WeightAgee { get; set; }
            public int Weightage1 { get; set; }
            public string DepartmentName { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string JobFuntionName { get; set; }
            public string SecondJobFuntionName { get; set; }
        }
        #endregion Get all Group Detail

        #region This Api Use Get All Job Assing Data
        /// <summary>
        /// API >> Get >>api/Funtion/getalldatajobassing
        ///  Created by  Mayank Prajapati On 17/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldatajobassing")]
        public async Task<IHttpActionResult> GetAllJobAssing(int departmentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await (from e in _db.Employee
                                      join u in _db.User on e.EmployeeId equals u.EmployeeId
                                      where (e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId && e.DepartmentId == departmentId)
                                      select new JobHelpermodeul
                                      {
                                          EmployeeId = e.EmployeeId,
                                          EmployeeName = _db.Employee.Where(x => x.EmployeeId == e.EmployeeId).Select(x => x.DisplayName).FirstOrDefault(),
                                          DesignationName = _db.Designation.Where(x => x.DesignationId == e.DesignationId).Select(x => x.DesignationName).FirstOrDefault(),

                                      }).ToListAsync();
                if (employee.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Employee list Found";
                    res.Data = employee;
                }
                else
                {
                    res.Message = "No Employee list Found";
                    res.Status = false;
                    res.Data = employee;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/sprint/getsprint", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class JobHelpermodeul
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string DesignationName { get; set; }
        }
        #endregion

        #region Helper For Dashboard
        public class DashboarsResponse
        {
            public int EmployeeCount { get; set; }
            public int JobFunctionCount { get; set; }
            public int JobFunctionEmployeeCount { get; set; }
            public int JobFunctionWithoutCom { get; set; }
            public int JobfuntionWithCom { get; set; }
        }
        #endregion

        #region Api for Dashboard 
        /// <summary>
        /// API >> Get >>api/Funtion/getemployeewithjobfuntioncount
        ///  Created by  Mayank Prajapati On 22/11/2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeewithjobfuntioncount")]
        public async Task<IHttpActionResult> GetEmployeeWithJobFuntioncount()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DashboarsResponse response = new DashboarsResponse();

            try
            {
                var employeeCount = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee).CountAsync();
                response.EmployeeCount = employeeCount;

                var TotaljobFunction = await _db.JobFuntions.Where(x => x.IsActive && !x.IsDeleted).CountAsync();
                response.JobFunctionCount = TotaljobFunction;

                var employeeJobFunction = _db.WeightAges.Where(x => x.EmployeeId != 0 && x.IsActive && !x.IsDeleted).Count();
                response.JobFunctionEmployeeCount = employeeJobFunction;

                var Competencie = _db.JobFunctionCompencies.Where(x => x.CompentenciesId == null && x.IsActive && !x.IsDeleted).ToList().Count();
                response.JobFunctionWithoutCom = Competencie;

                res.Message = "data found";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                logger.Error("api/Funtion/getemployeewithjobfuntioncount", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        #endregion

        public class JobFunctionReponse
        {
            public string JobFuntionName { get; set; }
            public string CompetencyName { get; set; }
            public string Descriptions { get; set; }
            public string Type { get; set; }
            public int Weightage { get; set; }
            public int Count { get; set; }
        }
        #endregion
    }
}
