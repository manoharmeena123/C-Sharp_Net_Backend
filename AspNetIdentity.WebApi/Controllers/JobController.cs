using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.HiringQuestion;
using AspNetIdentity.WebApi.Model.MailTemplate;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.HiringFlowMasterController;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Helper.ClientHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;
using RestClient = RestSharp.RestClient;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 01-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/job")]
    public class JobController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api For Add Job

        /// <summary>
        /// API >> Post >> api/job/addjob
        /// Created By Harshit Mitra on 31-01-2022
        /// Modify By Ankit on 05/08/2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addjob")]
        public async Task<ResponseBodyModel> AddJob(JobPost model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var location = _db.Locations.FirstOrDefault
                              (x => x.LocationId == model.LocationId
                               && x.CompanyId == claims.companyId);
                    if (location == null)
                    {
                        res.Message = "Location Required";
                        res.Status = false;
                        return res;
                    }
                    var department = _db.Department.FirstOrDefault
                                  (x => x.DepartmentId == model.DepartmentId
                                   && x.CompanyId == claims.companyId);
                    if (department == null)
                    {
                        res.Message = "Department Required";
                        res.Status = false;
                        return res;
                    }
                    //if (model.MaxExperience < model.MinExperience)
                    //{
                    //    res.Message = "Check Experience Range";
                    //    res.Status = false;
                    //    return res;
                    //}
                    JobPost obj = new JobPost
                    {
                        JobTitle = model.JobTitle,
                        LocationId = location.LocationId,
                        Location = location.LocationName,
                        IsPriority = model.IsPriority,
                        JobDescription = model.JobDescription,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        Offer = 0,
                        DepartmentId = department.DepartmentId,
                        Department = department.DepartmentName,
                        Openings = model.Openings,
                        TargetHireDate = model.TargetHireDate,
                        //SaleryStRange = model.SaleryStRange,
                        //SaleryEdRange = model.SaleryEdRange,
                        SaleryRange = model.SaleryRange,
                        SaleryEndRange = model.SaleryEndRange,
                        JobType = Enum.GetName(typeof(JobType),
                        Convert.ToInt32(model.JobType)).Replace('_', ' '),
                        HiringFlow = 0,
                        MinExperience = model.MinExperience,
                        MaxExperience = model.MaxExperience,
                        IsExtended = model.IsExtended,
                        ExtendedDays = model.IsExtended == true ? model.ExtendedDays : 0,
                        PublishToCareers = model.PublishToCareers,
                        PublishToPortal = model.PublishToPortal,
                        CompetativeSalary = model.CompetativeSalary,
                        ConfidentialSalary = model.ConfidentialSalary,
                        OrgId = claims.orgId,
                        OrgName = _db.OrgMaster.Where(x => x.OrgId == claims.orgId)
                                    .Select(x => x.OrgName).FirstOrDefault(),
                        JobCategory = model.IsPublished == true ? JobCategory.Active_Job
                                   : JobCategory.Archived_job,
                        CompanyId = claims.companyId,
                        PriorityId = model.PriorityId,
                        CreatedBy = claims.employeeId,
                    };

                    _db.JobPosts.Add(obj);
                    await _db.SaveChangesAsync();
                    var defaultiringFlow = JobHelper.CreateDefaultHiringFlow(obj);
                    _db.HiringStages.AddRange(defaultiringFlow);
                    await _db.SaveChangesAsync();

                    res.Message = "Job Added";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "No Data";
                    res.Status = false;
                }
            }

            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Add Job

        #region Api For Add Job To Dasborad

        /// <summary>
        /// API >> Post >> api/job/addcandidate
        /// Created By Ankit on 08-09-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcandidate")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> AddCandidate(CandidateData model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Stage = _db.HiringStages.Include("Job").Where
                          (x => x.Job.JobPostId == model.JobId &&
                          x.StageType == StageFlowType.Sourced
                          && x.IsActive && !x.IsDeleted && x.CompanyId
                           == model.CompanyId).FirstOrDefault();
                if (model != null)
                {
                    Candidate Demo = new Candidate
                    {
                        CandidateName = model.CandidateName,
                        Email = model.Email,
                        JobId = model.JobId,
                        MobileNumber = model.MobileNumber,
                        UploadResume = model.UploadResume,
                        Experience = model.Experience,
                        Gender = Enum.GetName(typeof(GenderConstants), model.Gender),
                        Description = model.Description,
                        StageType = Stage.StageType,
                        StageId = Stage.StageId,
                        Source = (int)JobHiringSourceConstants.Website,
                        PrebordingStages = Stage.StageType == StageFlowType
                                     .Preboarding ? PreboardingStages.Start : 0,
                        CreatedOn = DateTime.Now,
                        CompanyId = model.CompanyId,
                        OrgId = Stage.OrgId,
                        IsActive = true,
                        IsDeleted = false,
                    };

                    _db.Candidates.Add(Demo);
                    await _db.SaveChangesAsync();

                    var jobQuesions = _db.HiringQuestionsBanks.Where
                                (x => x.JobId == model.JobId).ToList();
                    if (jobQuesions.Count > 0)
                    {
                        foreach (var job in model.JobAnsAndQueList)
                        {
                            HiringQuesionsAndAnsBank obj = new HiringQuesionsAndAnsBank
                            {
                                CandidateId = Demo.CandidateId,
                                QuesionsId = job.QuesionsId,
                                JobId = Demo.JobId,
                                Answer = job.Answer,
                                CreatedOn = DateTime.Now,
                                CompanyId = model.CompanyId,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _db.HiringQuesionsAndAnsBanks.Add(obj);
                            await _db.SaveChangesAsync();
                        }
                    }
                    res.Message = "Candidate Added";
                    res.Status = true;
                    res.Data = Demo;
                }
                else
                {
                    res.Message = "No Data";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Add Job

        public class CandidateData
        {
            public string CandidateName { get; set; }
            public int JobId { get; set; }
            public string Email { get; set; }
            public string MobileNumber { get; set; }
            public string UploadResume { get; set; }
            public string Experience { get; set; }
            public int Gender { get; set; }
            public string Description { get; set; }
            public int CompanyId { get; set; }
            public List<JobAnsAndQuesionHelper> JobAnsAndQueList { get; set; }

        }
        public class JobAnsAndQuesionHelper
        {
            public Guid QuesionsId { get; set; }
            public string Answer { get; set; }
        }
        #region Api For Getting Job By Id

        /// <summary>
        /// API >> Get >> api/job/getbyid
        /// Created By Ankit on 13-09-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyid")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetJobById(int jobId, int companyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            HelperForjob response = new HelperForjob();
            List<JobQuestions> questionList = new List<JobQuestions>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<GetJobBycompanyandjobid> Jobdata = new List<GetJobBycompanyandjobid>();
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.JobPostId == jobId
                && x.IsActive && !x.IsDeleted);
                GetJobBycompanyandjobid obj = new GetJobBycompanyandjobid
                {
                    JobPostId = job.JobPostId,
                    JobTitle = job.JobTitle,
                    LocationId = job.LocationId,
                    Location = job.Location,
                    IsPriority = job.IsPriority,
                    JobDescription = job.JobDescription,
                    DepartmentId = job.DepartmentId,
                    Department = job.Department,
                    Offer = job.Offer,
                    Openings = job.Openings,
                    TargetHireDate = job.TargetHireDate,
                    //SaleryStRange = job.SaleryStRange,
                    //SaleryEdRange = job.SaleryEdRange,
                    SaleryRange = job.SaleryRange,
                    SaleryEndRange = job.SaleryEndRange,
                    JobType = job.JobType.ToString(),
                    HiringFlow = job.HiringFlow,
                    MinExperience = job.MinExperience,
                    MaxExperience = job.MaxExperience,
                    IsExtended = job.IsExtended,
                    ExtendedDays = job.ExtendedDays,
                    TotalCandidate = _db.Candidates.Where(x => x.JobId == jobId).ToList().Count,
                    PublishToCareers = job.PublishToCareers,
                    PublishToPortal = job.PublishToPortal,
                    IsPublished = job.IsPublished,
                    OrgName = job.OrgName,
                    ConfidentialSalary = job.ConfidentialSalary,
                    CompetativeSalary = job.CompetativeSalary,
                    AllSalaryType = job.SaleryRange + " to " + job.SaleryEndRange,
                };
                Jobdata.Add(obj);
                response.jobDetails = Jobdata;

                List<HelperCompany> companydata = new List<HelperCompany>();
                var companyList = await _db.Company.Where(x =>
                     x.IsDeleted == false && x.IsActive == true && x.CompanyId == companyId).FirstOrDefaultAsync();

                HelperCompany demo = new HelperCompany
                {
                    CompanyId = companyList.CompanyId,
                    RegisterCompanyName = companyList.RegisterCompanyName,
                    RegisterAddress = companyList.RegisterAddress,
                    CompanyGst = companyList.CompanyGst,
                    CIN = companyList.CIN,
                    PhoneNumber = companyList.PhoneNumber,
                    RegisterEmail = companyList.RegisterEmail,
                    IncorporationCertificate = companyList.IncorporationCertificate,
                    IncorporationDate = companyList.IncorporationDate
                };
                companydata.Add(demo);
                response.CompanyDetails = companydata;

                var jobQuesions = _db.HiringQuestionsBanks.Where
                              (x => x.IsActive && !x.IsDeleted && x.JobId
                               == jobId && x.CompanyId == companyId &&
                               x.ShowQuestionOnPortal == true).ToList();
                foreach (var item in jobQuesions)
                {
                    JobQuestions Pi = new JobQuestions
                    {
                        QuesionsId = item.QuesionsId,
                        QuesionTitle = item.QuesionTitle,
                    };
                    questionList.Add(Pi);
                }
                response.JobQuesions = questionList;
                res.Message = "Job and Comany Found";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting Job By Id

        /// <summary>
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        public class GetJobBycompanyandjobid
        {
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
            public string JobType { get; set; }
            public int HiringFlow { get; set; }
            public string MinExperience { get; set; }
            public string MaxExperience { get; set; }
            public bool IsExtended { get; set; }
            public int ExtendedDays { get; set; }
            public bool PublishToCareers { get; set; }
            public bool PublishToPortal { get; set; }
            public bool IsPublished { get; set; }
            public string OrgName { get; set; }
            public int TotalCandidate { get; set; }
            public JobPriorityHelperConstants PriorityId { get; set; }
            public string SaleryRange { get; set; } = string.Empty;
            public string SaleryEndRange { get; set; } = string.Empty;
            public bool ConfidentialSalary { get; set; } = false;
            public bool CompetativeSalary { get; set; } = false;
            public string AllSalaryType { get; set; } = string.Empty;
        }

        #region Api For Getting Job By Id

        /// <summary>
        /// API >> Get >> api/job/getjobbyjobId
        /// Created By Ankit on 13-09-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobbyjobId")]

        public async Task<ResponseBodyModel> GetJobById(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetJobByIdClass obj = new GetJobByIdClass();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var job = await _db.JobPosts.Where(x => x.JobPostId == jobId && x.IsActive && !x.IsDeleted
                && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (job != null)
                {
                    obj.JobPostId = job.JobPostId;
                    obj.JobTitle = job.JobTitle;
                    obj.LocationId = job.LocationId;
                    obj.Location = job.Location;
                    obj.IsPriority = job.IsPriority;
                    obj.JobDescription = job.JobDescription;
                    obj.DepartmentId = job.DepartmentId;
                    obj.Department = job.Department;
                    obj.Offer = job.Offer;
                    obj.Openings = job.Openings;
                    obj.TargetHireDate = job.TargetHireDate;
                    //obj.SaleryStRange = job.SaleryStRange;
                    //obj.SaleryEdRange = job.SaleryEdRange;
                    obj.SaleryRange = job.SaleryRange;
                    obj.SaleryEndRange = job.SaleryEndRange;
                    obj.JobType = (int)Enum.Parse(typeof(JobType), job.JobType.Replace(' ', '_'));
                    obj.HiringFlow = job.HiringFlow;
                    obj.MinExperience = job.MinExperience;
                    obj.MaxExperience = job.MaxExperience;
                    obj.IsExtended = job.IsExtended;
                    obj.ExtendedDays = job.ExtendedDays;
                    obj.PublishToCareers = job.PublishToCareers;
                    obj.PublishToPortal = job.PublishToPortal;
                    obj.IsPublished = job.IsPublished;
                    obj.PriorityId = job.PriorityId;
                    obj.ConfidentialSalary = job.ConfidentialSalary;
                    obj.CompetativeSalary = job.CompetativeSalary;
                    obj.OrgName = job.OrgName;
                    res.Message = "Job Found";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Job List Empty";
                    res.Status = false;
                    res.Data = obj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting Job By Id

        #region Api For Getting All Job

        /// <summary>
        /// API >> Get >> api/job/getalljob
        /// Created By Ankit Jain on 08-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalljob")]
        public async Task<ResponseBodyModel> GetAllJob()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobList = await (from j in _db.JobPosts
                                     where j.IsActive == true && j.IsDeleted == false && j.CompanyId == claims.companyId
                                     select new GetJob
                                     {
                                         JobPostId = j.JobPostId,
                                         JobTitle = j.JobTitle,
                                         Location = j.Location,
                                         IsPriority = j.IsPriority,
                                         TotalCandidates = _db.Candidates.Where(x => x.JobId == j.JobPostId).ToList().Count,
                                         Openings = j.Openings,
                                         DueDate = j.TargetHireDate,
                                         OrgName = j.OrgName,
                                         PublishToPortal = j.PublishToPortal,
                                         PublishToCareers = j.PublishToCareers,
                                         ExtendedDays = j.ExtendedDays,
                                         IsExtended = j.IsExtended,
                                         MinExperience = j.MinExperience,
                                         HiringFlow = j.HiringFlow,
                                         JobType = j.JobType,
                                         //SaleryEdRange = j.SaleryEdRange,
                                         //SaleryStRange = j.SaleryStRange,
                                         SaleryRange = j.SaleryRange,
                                         SaleryEndRange = j.SaleryEndRange,
                                         TargetHireDate = j.TargetHireDate,
                                         Department = j.Department,
                                         DepartmentId = j.DepartmentId,
                                         JobDescription = j.JobDescription,
                                         LocationId = j.LocationId,
                                         PriorityName = j.PriorityName,
                                         CreatedOn = j.CreatedOn,
                                         JobCreatedBy = _db.Employee.Where(x => x.CreatedBy == j.CreatedBy && x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault(),
                                         Offer = _db.Candidates.Where(x => x.PrebordingStages == PreboardingStages.Hired && x.JobId == j.JobPostId).ToList().Count(),
                                     }).OrderByDescending(j => j.CreatedOn).ToListAsync();
                if (jobList.Count > 0)
                {
                    res.Message = "All Job List";
                    res.Status = true;
                    res.Data = jobList;
                }
                else
                {
                    res.Message = "Job List Empty";
                    res.Status = false;
                    res.Data = jobList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Job

        #region Api For Getting All Job By Without Authorize

        /// <summary>
        /// API >> Get >> api/job/getalljobbycompanyid
        /// Created By Ankit jain on 01-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalljobbycompanyid")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetAll(int companyId, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            HelperForCandidate JobData = new HelperForCandidate();
            try
            {
                if (search == null)
                {
                    var jobList = await (from j in _db.JobPosts
                                         where j.IsActive == true && j.IsDeleted == false && j.JobCategory == JobCategory.Active_Job
                                         && j.CompanyId == companyId && j.PublishToCareers == true
                                         select new GetJob
                                         {
                                             JobPostId = j.JobPostId,
                                             JobTitle = j.JobTitle,
                                             Location = j.Location,
                                             IsPriority = j.IsPriority,
                                             TotalCandidates = _db.Candidates.Where(x => x.JobId == j.JobPostId).ToList().Count,
                                             Offer = j.Offer,
                                             Openings = j.Openings,
                                             DueDate = j.TargetHireDate,
                                             OrgName = j.OrgName,
                                             PublishToPortal = j.PublishToPortal,
                                             PublishToCareers = j.PublishToCareers,
                                             ExtendedDays = j.ExtendedDays,
                                             IsExtended = j.IsExtended,
                                             MinExperience = j.MinExperience,
                                             HiringFlow = j.HiringFlow,
                                             JobType = j.JobType,
                                             SaleryRange = j.SaleryRange,
                                             SaleryEndRange = j.SaleryEndRange,
                                             TargetHireDate = j.TargetHireDate,
                                             Department = j.Department,
                                             DepartmentId = j.DepartmentId,
                                             JobDescription = j.JobDescription,
                                             LocationId = j.LocationId,
                                             MaxExperience = j.MaxExperience,
                                             CompetativeSalary = j.CompetativeSalary,
                                             ConfidentialSalary = j.ConfidentialSalary,
                                             AllSalaryType = j.SaleryRange + " to " + j.SaleryEndRange,
                                         }).ToListAsync();
                    JobData.Totaljob = jobList;

                    var companyList = await _db.Company.Where(x =>
                        x.IsDeleted == false && x.IsActive == true && x.CompanyId == companyId)
                       .Select(x => new
                       {
                           x.RegisterCompanyName,
                           x.RegisterAddress,
                           x.CompanyId,
                           x.CompanyGst,
                           x.CIN,
                           x.PhoneNumber,
                           x.RegisterEmail,
                           x.IncorporationCertificate,
                           x.IncorporationDate
                       }).ToListAsync();

                    var company = await _db.Company.Where(x =>
                          x.IsDeleted == false && x.IsActive == true && x.CompanyId == companyId)
                         .Select(x => new
                         {
                             x.RegisterCompanyName,
                             x.NavigationLogo,

                         }).ToListAsync();

                    JobData.CompanyDetails = companyList;
                    JobData.CompanyLogoName = company;

                    res.Message = "All Job List Found";
                    res.Status = true;
                    res.Data = JobData;
                }
                else
                {
                    var jobList = await (from j in _db.JobPosts
                                         where j.IsActive == true && j.IsDeleted == false && j.JobCategory == JobCategory.Active_Job
                                         && j.CompanyId == companyId && j.PublishToCareers == true && j.JobTitle.ToLower().Contains(search.ToLower())
                                         select new GetJob
                                         {
                                             JobPostId = j.JobPostId,
                                             JobTitle = j.JobTitle,
                                             Location = j.Location,
                                             IsPriority = j.IsPriority,
                                             TotalCandidates = _db.Candidates.Where(x => x.JobId == j.JobPostId).ToList().Count,
                                             Offer = j.Offer,
                                             Openings = j.Openings,
                                             DueDate = j.TargetHireDate,
                                             OrgName = j.OrgName,
                                             PublishToPortal = j.PublishToPortal,
                                             PublishToCareers = j.PublishToCareers,
                                             ExtendedDays = j.ExtendedDays,
                                             IsExtended = j.IsExtended,
                                             MinExperience = j.MinExperience,
                                             HiringFlow = j.HiringFlow,
                                             JobType = j.JobType,
                                             //SaleryEdRange = j.SaleryEdRange,
                                             //SaleryStRange = j.SaleryStRange,
                                             SaleryRange = j.SaleryRange,
                                             SaleryEndRange = j.SaleryEndRange,
                                             TargetHireDate = j.TargetHireDate,
                                             Department = j.Department,
                                             DepartmentId = j.DepartmentId,
                                             JobDescription = j.JobDescription,
                                             LocationId = j.LocationId,
                                             MaxExperience = j.MaxExperience,
                                         }).ToListAsync();
                    JobData.Totaljob = jobList;

                    var companyList = await _db.Company.Where(x =>
                          x.IsDeleted == false && x.IsActive == true && x.CompanyId == companyId)
                         .Select(x => new
                         {
                             x.RegisterCompanyName,
                             x.RegisterAddress,
                             x.CompanyId,
                             x.CompanyGst,
                             x.CIN,
                             x.PhoneNumber,
                             x.RegisterEmail,
                             x.IncorporationCertificate,
                             x.IncorporationDate
                         }).ToListAsync();

                    var company = await _db.Company.Where(x =>
                          x.IsDeleted == false && x.IsActive == true && x.CompanyId == companyId)
                         .Select(x => new
                         {
                             x.RegisterCompanyName,
                             x.NavigationLogo,

                         }).ToListAsync();

                    JobData.CompanyDetails = companyList;
                    JobData.CompanyLogoName = company;

                    res.Message = "All Job List Found";
                    res.Status = true;
                    res.Data = JobData;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Job

        public class HelperForCandidate
        {
            public object Totaljob { get; set; }
            public object CompanyDetails { get; set; }
            public object CompanyLogoName { get; set; }
        }
        public class HelperForjob
        {
            public List<GetJobBycompanyandjobid> jobDetails { get; set; }
            public List<HelperCompany> CompanyDetails { get; set; }
            public List<JobQuestions> JobQuesions { get; set; }
        }
        public class HelperCompany
        {
            public string RegisterCompanyName { get; set; }
            public string IncorporationCertificate { get; set; }
            public DateTimeOffset IncorporationDate { get; set; }
            public string PhoneNumber { get; set; }
            public string RegisterEmail { get; set; }
            public string RegisterAddress { get; set; }
            public string CIN { get; set; }
            public string CompanyGst { get; set; }
            public int CompanyId { get; set; }
        }
        public class JobQuestions
        {
            public Guid QuesionsId { get; set; } = Guid.NewGuid();
            public string QuesionTitle { get; set; } = string.Empty;
        }

        #region Api For Getting All Job By Without Authorize

        /// <summary>s
        /// API >> Get >> api/job/getjobbyid
        /// Created By Ankit Jain on 08-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobbyid")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetAllData(int companyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetJobByIdClass obj = new GetJobByIdClass();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted
                && x.JobCategory == JobCategory.Active_Job).FirstOrDefaultAsync();
                if (job != null)
                {
                    obj.JobPostId = job.JobPostId;
                    obj.JobTitle = job.JobTitle;
                    obj.LocationId = job.LocationId;
                    obj.Location = job.Location;
                    obj.IsPriority = job.IsPriority;
                    obj.JobDescription = job.JobDescription;
                    obj.DepartmentId = job.DepartmentId;
                    obj.Department = job.Department;
                    obj.Offer = job.Offer;
                    obj.Openings = job.Openings;
                    obj.TargetHireDate = job.TargetHireDate;
                    //obj.SaleryStRange = job.SaleryStRange;
                    //obj.SaleryEdRange = job.SaleryEdRange;
                    obj.SaleryRange = job.SaleryRange;
                    obj.SaleryEndRange = job.SaleryEndRange;
                    obj.JobType = (int)Enum.Parse(typeof(JobType), job.JobType.Replace(' ', '_'));
                    obj.HiringFlow = job.HiringFlow;
                    obj.MinExperience = job.MinExperience;
                    obj.IsExtended = job.IsExtended;
                    obj.ExtendedDays = job.ExtendedDays;
                    obj.TotalCandidate = _db.Candidates.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted).ToList().Count();
                    obj.PublishToCareers = job.PublishToCareers;
                    obj.PublishToPortal = job.PublishToPortal;
                    obj.PriorityId = job.PriorityId;
                    obj.IsPublished = job.IsPublished;
                    obj.OrgName = job.OrgName;
                    res.Message = "Job Found";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Job List Empty";
                    res.Status = false;
                    res.Data = obj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Job

        #region Api To Getting All Active Jobs By Job Category Filter

        /// <summary>
        /// API >> Get >> api/job/getjobbycategory
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="JobCategory"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobbycategory")]
        public async Task<ResponseBodyModel> GetJobFilter(JobCategory JobCategory)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobList = await (from j in _db.JobPosts
                                     where j.IsActive == true && j.IsDeleted == false && j.JobCategory == JobCategory && j.CompanyId == claims.companyId
                                     select new GetJobFilterHelper
                                     {
                                         JobPostId = j.JobPostId,
                                         JobTitle = j.JobTitle,
                                         Location = j.Location,
                                         IsPriority = j.IsPriority,
                                         TotalCandidates = _db.Candidates.Count(x => x.JobId == j.JobPostId),
                                         Offer = j.Offer,
                                         Openings = j.Openings,
                                         DueDate = j.TargetHireDate,
                                         NewCandidate = _db.Candidates.Count(x => x.NewCandidate == true && x.JobId == j.JobPostId),
                                         PriorityId = j.PriorityId,
                                         JobCreatedBy = _db.Employee.Where(x => x.EmployeeId == j.CreatedBy /*x.CreatedBy == j.CreatedBy*/).Select(x => x.DisplayName).FirstOrDefault(),
                                         ArchivedCount = _db.Candidates.Where(x => x.StageType == StageFlowType.Archived && x.JobId == j.JobPostId).ToList().Count()
                                     }).ToListAsync();

                if (jobList.Count > 0)
                {
                    res.Message = "Job List";
                    res.Status = true;
                    res.Data = jobList.OrderByDescending(x => x.PriorityId).ToList();
                }
                else
                {
                    res.Message = "Job List Empty";
                    res.Status = false;
                    res.Data = jobList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Getting All Active Jobs By Job Category Filter

        #region Api For Getting Job Details Flow For Diffrent Types

        /// <summary>
        /// API >> Get >> api/job/getjobdetailsbytype
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="type"></param>
        [HttpGet]
        [Route("getjobdetailsbytype")]
        public async Task<ResponseBodyModel> GetJobDetailsByType(int jobId, int type)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            SummaryModal summary = new SummaryModal();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == jobId && x.CompanyId == claims.companyId);

                var emptyList = new List<string>();
                if (job == null)
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                }
                else
                {
                    if (type == 1) // Summary //summary section Created  by shriya Create on 26-04-2022
                    {
                        DateTimeOffset targetdate = job.TargetHireDate.HasValue ? (DateTimeOffset)job.TargetHireDate : job.CreatedOn.AddDays(30);
                        DateTimeOffset today = DateTimeOffset.Now;

                        summary.OffersJob = job.Offer;
                        summary.OpeningJob = job.Openings;
                        if (targetdate >= today)
                        {
                            TimeSpan duedate = targetdate - today;
                            summary.DueDays = duedate.Days;
                        }
                        else
                        {
                            summary.DueDays = 0;
                        }

                        #region pie chart

                        var allCandidateList = _db.Candidates.Where(x => x.JobId == jobId &&
                                x.IsActive == true && x.IsDeleted == false).ToList();
                        var sourceInJob = allCandidateList.Select(x => x.Source)
                                .ToList().Distinct().ToList();
                        List<SourcesPie> sourcePie = new List<SourcesPie>();
                        foreach (var item in sourceInJob)
                        {
                            SourcesPie obj = new SourcesPie
                            {
                                Name = Enum.GetName(typeof(JobHiringSourceConstants), item).Replace("_", " "),
                                Value = allCandidateList.Where(x => x.Source == item)
                                       .ToList().Count,
                            };
                            sourcePie.Add(obj);
                        }
                        summary.PieChart = sourcePie;

                        #endregion pie chart

                        #region HiringFunnel graph

                        List<HiringFunnel> hiringFunnels = new List<HiringFunnel>();
                        //var stageInJob = allCandidateList.Select(x => x.StageType).ToList().Distinct().ToList();
                        var stageInJob = _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == jobId
                                           && x.IsActive && !x.IsDeleted).OrderBy(x => x.StageOrder).ToList();
                        foreach (var satge in stageInJob)
                        {
                            HiringFunnel obj = new HiringFunnel
                            {
                                Stat = _db.HiringStages.Where(x => x.StageId == satge.StageId)
                                     .Select(x => x.StageName).FirstOrDefault(),
                                //Count = allCandidateList.Where(x => x.Stage == satge)
                                //       .ToList().Count,
                                Count = _db.Candidates.Where(c => c.JobId == job.JobPostId && c.IsActive == true
                                         && c.IsDeleted == false && c.StageId == satge.StageId).ToList().Count,
                            };
                            hiringFunnels.Add(obj);
                        }
                        summary.HiringFunnel = hiringFunnels;

                        #endregion HiringFunnel graph

                        #region job Information about behalf on Id
                        List<GetJobDetailsByType4> InfoJob = new List<GetJobDetailsByType4>();
                        GetJobDetailsByType4 data = new GetJobDetailsByType4()
                        {
                            JobId = job.JobPostId,
                            IsPriority = job.IsPriority,
                            JobTitle = job.JobTitle,
                            Details = job.JobTitle + " - " +
                                      job.JobType + " - " +
                                      "(" + job.Location + ")" + " - " +
                                         ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience + " to " + job.MaxExperience
                                        : "")),
                            JobDescription = job.JobDescription,
                            JobInfo = new JobInfo()
                            {
                                Location = job.Location,
                                Department = job.Department,
                                JobType = job.JobType,
                                Openings = job.Openings,
                                TargetHireDate = job.TargetHireDate,
                                Budget = job.SaleryRange + " to " + job.SaleryEndRange,
                                MinExperience = job.MinExperience,
                                JobTitle = job.JobTitle,
                                OrgName = job.OrgName,
                                CompetativeSalary = job.CompetativeSalary,
                                ConfidentialSalary = job.ConfidentialSalary,
                                JobExperience = ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience + " - " + job.MaxExperience
                                        : "")),
                            },
                        };
                        InfoJob.Add(data);
                        summary.JobInfo = InfoJob;

                        #endregion job Information about behalf on Id

                        summary.CandidateSource = allCandidateList.Count();

                        #region Requirter performance (Candidate sourced in past weeks )

                        List<RequirterCount> RequireList = new List<RequirterCount>();
                        DateTime date = DateTime.Now;
                        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                        DataHelperResponse dateHelperDTO = new DataHelperResponse();
                        dateHelperDTO.startDate = firstDayOfMonth;
                        dateHelperDTO.endDate = lastDayOfMonth;
                        var requirterId = _db.Candidates.Where(x => x.JobId == jobId &&
                                x.IsActive == true && x.IsDeleted == false)
                            .Select(x => x.CreatedBy).Distinct().ToList();

                        RequirterCount requte = new RequirterCount();
                        requte.Week = weekdate();

                        foreach (var item in requirterId)
                        {
                            requte.RequirterName = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.EmployeeId == item).Select(x => x.FirstName + "" + x.LastName).FirstOrDefault();
                            foreach (var item1 in requte.Week)
                            {
                                var val = allCandidateList.Where(x => x.CreatedBy == item && (x.CreatedOn >= item1.StartOfWeek && x.CreatedOn <= item1.EndOfWeek)).Count();

                                requte.value = val;
                            }
                        }
                        RequireList.Add(requte);

                        summary.RequirterCount = RequireList;

                        #endregion Requirter performance (Candidate sourced in past weeks )

                        res.Message = "summary about job module";
                        res.Status = true;
                        res.Data = summary;
                    }
                    else if (type == 2) // Hiring Team
                    {
                    }
                    else if (type == 3) // Hiring Flow
                    {
                        AddEditHiringFlow obj = new AddEditHiringFlow();
                        List<StageMaster> ListStage = new List<StageMaster>();
                        var hiringFlow = await _db.HiringFlow.FirstOrDefaultAsync(x => x.HiringFlowId == job.HiringFlow);
                        if (hiringFlow != null)
                        {
                            var stagesId = hiringFlow.OrderStagesId.Split(',');
                            foreach (var stage in stagesId)
                            {
                                var i = Convert.ToInt32(stage);
                                var stages = await _db.Stages.FirstOrDefaultAsync(x => x.StageId == i);
                                if (stages != null)
                                    ListStage.Add(stages);
                            }
                            obj.HiringFlowId = hiringFlow.HiringFlowId;
                            obj.HiringFlowTitle = hiringFlow.HiringFlowTitle;
                            obj.OrderStagesId = hiringFlow.OrderStagesId;
                            obj.Sourced = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Sourced).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();
                            obj.Screening = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Screening).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();
                            obj.Interview = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Interview).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();
                            obj.Preboarding = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Preboarding).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();
                            obj.Hired = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Hired).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();
                            obj.Archive = ListStage.Where(x => x.StageType == EnumClass.StageFlowType.Archived).Select(x => new StageClass()
                            {
                                StageId = x.StageId,
                                StageName = x.StageName,
                                SechduleRequired = x.SechduleRequired,
                                SechduleDateTime = x.SechduleDateTime,
                            }).ToList();

                            res.Message = "Get Hiring Flow";
                            res.Status = true;
                            res.Data = obj;
                        }
                        else
                        {
                            res.Message = "Hiring Flow Not Found";
                            res.Status = false;
                        }
                    }
                    else if (type == 4) //
                    {
                        /// <summary>
                        /// Created By Harshit Mitra on 01-02-2022
                        /// </summary>

                        GetJobDetailsByType4 data = new GetJobDetailsByType4()
                        {
                            JobId = job.JobPostId,
                            IsPriority = job.IsPriority,
                            JobTitle = job.JobTitle,
                            Details = job.JobTitle + " - " +
                                      job.JobType + " - " +
                                      "(" + job.Location + ")" + " - " +
                                         ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience + " to " + job.MaxExperience
                                        : "")),
                            JobDescription = job.JobDescription,
                            JobInfo = new JobInfo()
                            {
                                Location = job.Location,
                                Department = job.Department,
                                JobType = job.JobType,
                                Openings = job.Openings,
                                TargetHireDate = job.TargetHireDate,
                                ConfidentialSalary = job.ConfidentialSalary,
                                CompetativeSalary = job.CompetativeSalary,
                                Budget = job.SaleryRange + " to " + job.SaleryEndRange,
                                JobExperience = ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience + " to " + job.MaxExperience
                                        : "")),
                                MinExperience = job.MinExperience,
                            },
                        };
                        res.Message = "Job Info";
                        res.Status = true;
                        res.Data = data;
                    }
                    else if (type == 5) // Application Form
                    {
                        res.Message = "In Progress";
                        res.Status = true;
                        res.Data = emptyList;
                    }
                    else // If Type is null or Not in List
                    {
                        res.Message = "This Type Is Not Supported";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting Job Details Flow For Diffrent Types

        #region Api To Get Job Detail With Stage Flow by Type

        /// <summary>
        /// API >> Get >> api/job/getjobdetailbytypeforstage
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="stageId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobdetailbytypeforstage")]
        public async Task<ResponseBodyModel> GetJobDetailByTypeForStageFlow(int jobId, Guid? stageId, int Count, int Page)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            JobDetailByTypeForStageFlow resobj = new JobDetailByTypeForStageFlow();
            List<CandidateListOnStage> listOnStages = new List<CandidateListOnStage>();
            List<CandidateListOnStageData> listOnStagesdata = new List<CandidateListOnStageData>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == jobId && x.CompanyId == claims.companyId);
                if (job != null)
                {

                    resobj.JobId = job.JobPostId;
                    resobj.IsPriority = job.IsPriority;
                    resobj.JobTitle = job.JobTitle;
                    resobj.JobCategory = job.JobCategory;
                    resobj.Details = job.JobTitle + " - " +
                                      job.JobType + " - " +
                                      "(" + job.Location + ")" + " - " +
                                         ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience
                                         + " to " + job.MaxExperience : ""));
                    var allcanditates = await _db.Candidates.Where(x => x.JobId == job.JobPostId && !x.IsDeleted
                                           && x.IsActive && x.CompanyId == claims.companyId).ToListAsync();
                    var hiringStaged = await _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId ==
                                       job.JobPostId && x.IsActive && !x.IsDeleted).ToListAsync();
                    resobj.StageCount = hiringStaged
                        .OrderBy(x => x.StageOrder)
                        .Select(x => new NewStageCounts
                        {
                            StageId = x.StageId,
                            StageOrder = x.StageOrder,
                            StageTitle = x.StageName,
                            Count = allcanditates.Count(c => c.StageId == x.StageId),
                            IsInterview = x.SechduleRequired,

                        }).ToList();

                    resobj.StageTitle = hiringStaged.Where(x => x.StageId == stageId).Select(x => x.StageName).FirstOrDefault();
                    foreach (var item in allcanditates)
                    {
                        if (item.StageId == stageId)
                        {
                            CandidateListOnStage stage = new CandidateListOnStage()
                            {
                                JobId = jobId,
                                StageId = (Guid)item.StageId,
                                StageOrder = hiringStaged.Where(x => x.StageId == item.StageId).Select(x => x.StageOrder).FirstOrDefault(),
                                StageName = hiringStaged.Where(x => x.StageId == item.StageId).Select(x => x.StageName).FirstOrDefault(),
                                CandidateId = item.CandidateId,
                                CandidateName = item.CandidateName,
                                Source = Enum.GetName(typeof(JobHiringSourceConstants), item.Source).Replace("_", " "),
                                AppliedDate = item.CreatedOn.ToString("MMM dd yyyy"),
                                MobileNumber = item.MobileNumber,
                                Email = item.Email,
                                InterviewDate = item.InterviewSechduleDate,
                                IsPreboarding = item.StageType == StageFlowType.Preboarding,
                                IsInterview = hiringStaged.Where(x => x.StageId == item.StageId).Select(x => x.SechduleRequired).FirstOrDefault(),
                                IsPrebordingStarted = item.IsPreboardingStarted,
                                NewCandidate = item.NewCandidate,
                            };
                            listOnStages.Add(stage);
                        }
                    }

                    CandidateListOnStageData data = new CandidateListOnStageData
                    {

                        TotalData = listOnStages.Count,
                        Counts = Count,
                        List = listOnStages.Skip((Page - 1) * Count).Take(Count).ToList(),
                    };
                    listOnStagesdata.Add(data);
                    resobj.CandidateList = listOnStagesdata;
                    ;

                    res.Message = "Job Detail For Stage";
                    res.Status = true;
                    res.Data = resobj;
                }
                else
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Job Detail With Stage Flow by Type

        #region Api For Search Job on Behaf Of Job Title

        /// <summary>
        /// API >> Get >> api/job/searchbyjobtitle
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("searchbyjobtitle")]
        public async Task<ResponseBodyModel> SearchJobByTitle(string strings)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (string.IsNullOrWhiteSpace(strings) || string.IsNullOrEmpty(strings))
                {
                    res.Message = "Search Cannot Be Null";
                    res.Status = false;
                }
                else
                {
                    var jobList = await (from j in _db.JobPosts
                                         join e in _db.Employee on j.CreatedBy equals e.EmployeeId
                                         where j.IsActive == true && j.IsDeleted == false && j.CompanyId == claims.companyId && j.JobTitle.Contains(strings)
                                         select new GetJob
                                         {
                                             JobPostId = j.JobPostId,
                                             JobTitle = j.JobTitle,
                                             Location = j.Location,
                                             IsPriority = j.IsPriority,
                                             TotalCandidates = _db.Candidates.Where(x => x.JobId == j.JobPostId).ToList().Count,
                                             Openings = j.Openings,
                                             DueDate = j.TargetHireDate,
                                             OrgName = j.OrgName,
                                             PublishToPortal = j.PublishToPortal,
                                             PublishToCareers = j.PublishToCareers,
                                             ExtendedDays = j.ExtendedDays,
                                             IsExtended = j.IsExtended,
                                             MinExperience = j.MinExperience,
                                             HiringFlow = j.HiringFlow,
                                             JobType = j.JobType,
                                             SaleryRange = j.SaleryRange,
                                             SaleryEndRange = j.SaleryEndRange,
                                             TargetHireDate = j.TargetHireDate,
                                             Department = j.Department,
                                             DepartmentId = j.DepartmentId,
                                             JobDescription = j.JobDescription,
                                             LocationId = j.LocationId,
                                             PriorityName = j.PriorityName,
                                             CreatedOn = j.CreatedOn,
                                             PriorityId = j.PriorityId,
                                             JobCreatedBy = e.DisplayName,
                                             Offer = _db.Candidates.Where(x => x.PrebordingStages == PreboardingStages.Hired && x.JobId == j.JobPostId).ToList().Count(),
                                         }).ToListAsync();
                    if (jobList.Count > 0)
                    {
                        res.Message = "Job List";
                        res.Status = true;
                        res.Data = jobList;
                    }
                    else
                    {
                        res.Message = "Job Not Found";
                        res.Status = false;
                        res.Data = jobList;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Search Job on Behaf Of Job Title

        #region Api To Make Job Archived

        /// <summary>
        /// API >> Get >> api/job/makejobarchive
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("makejobarchive")]
        public async Task<ResponseBodyModel> MakeJobArchive(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == jobId && x.CompanyId == claims.companyId);
                if (job != null)
                {
                    job.JobCategory = JobCategory.Archived_job;
                    _db.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Job Change to Archived";
                    res.Status = true;
                    res.Data = job;
                }
                else
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Make Job Archived

        #region Api To Make Job Activeg

        /// <summary>
        /// API >> Get >> api/job/makejobactive
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("makejobactive")]
        public async Task<ResponseBodyModel> MakeJobAative(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == jobId && x.CompanyId == claims.companyId);
                if (job != null)
                {
                    job.JobCategory = JobCategory.Active_Job;
                    _db.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Job Change to Active";
                    res.Status = true;
                    res.Data = job;
                }
                else
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Make Job Archived

        #region Api For Edit Job

        /// <summary>
        /// API >> Put >> api/job/editjob
        /// Created By Harshit Mitra on 01-02-2022
        /// Modify By Ankit on 05/08/2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editjob")]
        public async Task<ResponseBodyModel> EditJob(JobPost model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    //if (model.MaxExperience < model.MinExperience)
                    //{
                    //    res.Message = "Check Experience Range";
                    //    res.Status = false;
                    //    return res;
                    //}
                    var location = _db.Locations.FirstOrDefault(x => x.LocationId == model.LocationId && x.CompanyId == claims.companyId);
                    var department = _db.Department.FirstOrDefault(x => x.DepartmentId == model.DepartmentId);
                    var obj = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == model.JobPostId);
                    if (obj != null)
                    {
                        obj.JobTitle = model.JobTitle;
                        obj.LocationId = location.LocationId;
                        obj.Location = location.LocationName;
                        obj.IsPriority = model.IsPriority;
                        obj.JobDescription = model.JobDescription;
                        obj.UpdatedOn = DateTime.Now;
                        obj.DepartmentId = department.DepartmentId;
                        obj.Department = department.DepartmentName;
                        obj.Openings = model.Openings;
                        obj.TargetHireDate = model.TargetHireDate;
                        obj.SaleryRange = model.SaleryRange;
                        obj.SaleryEndRange = model.SaleryEndRange;
                        obj.JobType = Enum.GetName(typeof(JobType), Convert.ToInt32(model.JobType)).Replace('_', ' ');
                        obj.HiringFlow = model.HiringFlow;
                        obj.MinExperience = model.MinExperience;
                        obj.MaxExperience = model.MaxExperience;
                        obj.IsExtended = model.IsExtended;
                        obj.ExtendedDays = model.IsExtended == true ? model.ExtendedDays : 0;
                        obj.PublishToCareers = model.PublishToCareers;
                        obj.PublishToPortal = model.PublishToPortal;
                        obj.PriorityId = model.PriorityId;
                        obj.CompetativeSalary = model.CompetativeSalary;
                        obj.ConfidentialSalary = model.ConfidentialSalary;
                        _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Job Updated";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Job Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "No Data";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Edit Job

        #region Api For Delete Job

        /// <summary>
        /// API >> Put >> api/job/deletejob
        /// Created By Harshit Mitra on 01-02-2022
        /// Modify By Ankit on 05-08-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletejob")]
        public async Task<ResponseBodyModel> DeleteJob(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == jobId && x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId);
                if (job != null)
                {
                    job.IsDeleted = true;
                    job.IsActive = false;
                    job.DeletedOn = DateTime.Now;
                    job.UpdatedOn = DateTime.Now;
                    _db.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Job Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Delete Job

        #region Api For Adding Job Template

        /// <summary>
        /// API >> Post >> api/job/addtemplate
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addtemplate")]
        public async Task<ResponseBodyModel> AddTemplates(JobPostTemplate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                JobPostTemplate obj = new JobPostTemplate()
                {
                    TemplateTitle = model.TemplateTitle,
                    TemplateDescription = model.TemplateDescription,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                };
                _db.JobTemplates.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Template Added";
                res.Status = true;
                res.Data = obj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Adding Job Template

        #region Api For Getting Template By Id

        /// <summary>
        /// API >> Get >> api/job/templatebyid
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("templatebyid")]
        public async Task<ResponseBodyModel> GetTemplateById(int templateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templates = await _db.JobTemplates.FirstOrDefaultAsync(x => x.TemplateId == templateId && x.CompanyId == claims.companyId);
                if (templates != null)
                {
                    res.Message = "Template By Id";
                    res.Status = true;
                    res.Data = templates;
                }
                else
                {
                    res.Message = "Template Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting Template By Id

        #region Api For Getting All Templates

        /// <summary>
        /// API >> Get >> api/job/getalltemplates
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltemplates")]
        public async Task<ResponseBodyModel> GetAllTemplates()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templates = await _db.JobTemplates.OrderByDescending(x => x.CreatedOn == DateTime.Now && x.CompanyId == claims.companyId).ToListAsync();
                if (templates.Count > 0)
                {
                    res.Message = "All Template List";
                    res.Status = true;
                    res.Data = templates;
                }
                else
                {
                    res.Message = "Template List is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Templates

        #region Api For Getting All Active Job Templates

        /// <summary>
        /// API >> Get >> api/job/activetemplates
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("activetemplates")]
        public async Task<ResponseBodyModel> GetAllActiveTemplate()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templates = await _db.JobTemplates.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();
                if (templates.Count > 0)
                {
                    res.Message = "All Template List";
                    res.Status = true;
                    res.Data = templates;
                }
                else
                {
                    res.Message = "Template List is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Active Job Templates

        #region Api To Get All Active Template List

        /// <summary>
        /// API >> Get >> api/job/getactivetemplatelist
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getactivetemplatelist")]
        public async Task<ResponseBodyModel> GetAllActiveTemplate2()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var templates = await _db.JobTemplates.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId)
                    .Select(x => new
                    {
                        x.TemplateId,
                        x.TemplateTitle,
                    }).ToListAsync();
                if (templates.Count > 0)
                {
                    res.Message = "All Template List";
                    res.Status = true;
                    res.Data = templates;
                }
                else
                {
                    res.Message = "Template List is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All Active Template List

        #region Api For Eding Job Template

        /// <summary>
        /// API >> Post >> api/job/edittemplate
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("edittemplate")]
        public async Task<ResponseBodyModel> EditTemplates(JobPostTemplate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var template = await _db.JobTemplates.FirstOrDefaultAsync(x => x.TemplateId == model.TemplateId && x.CompanyId == claims.companyId);
                if (template != null)
                {
                    template.TemplateTitle = model.TemplateTitle;
                    template.TemplateDescription = model.TemplateDescription;
                    template.UpdatedOn = DateTime.Now;

                    _db.Entry(template).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Template Edited";
                    res.Status = true;
                    res.Data = template;
                }
                else
                {
                    res.Message = "Template Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Eding Job Template

        #region Api For Delete Job Template

        /// <summary>
        /// API >> Post >> api/job/deletetemplate
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletetemplate")]
        public async Task<ResponseBodyModel> DeleteTemplates(JobPostTemplate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var template = await _db.JobTemplates.FirstOrDefaultAsync(x => x.TemplateId == model.TemplateId && x.CompanyId == claims.companyId);
                if (template != null)
                {
                    template.IsDeleted = true;
                    template.IsActive = false;
                    template.DeletedOn = DateTime.Now;

                    _db.Entry(template).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Template Edited";
                    res.Status = true;
                    res.Data = template;
                }
                else
                {
                    res.Message = "Template Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Delete Job Template

        #region Api To Get List Of Job With Filter

        /// <summary>
        /// API >> Get >> api/job/jobfiltercareer
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="jopPostId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("jobfiltercareer")]
        public async Task<ResponseBodyModel> GetJobListByFilterOnCarrer(int? locationId, int? departmentId, int? jopPostId, bool isCareer)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<JobOpeningList> jobPost = new List<JobOpeningList>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (locationId == null)
                {
                    if (departmentId == null)
                    {
                        if (jopPostId == null)
                        {
                            jobPost = await _db.JobPosts.Where(x => x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                        else
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.JobPostId == jopPostId
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                    }
                    else
                    {
                        var department = Enum.GetName(typeof(Department), departmentId).Replace('_', ' ');
                        if (jopPostId == null)
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.Department == department
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                        else
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.Department == department
                                        && x.JobPostId == jopPostId
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                    }
                }
                else
                {
                    if (departmentId == null)
                    {
                        if (jopPostId == null)
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.LocationId == locationId
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                        else
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.LocationId == locationId
                                        && x.JobPostId == jopPostId
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                    }
                    else
                    {
                        var department = Enum.GetName(typeof(Department), departmentId).Replace('_', ' ');
                        if (jopPostId == null)
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.LocationId == locationId
                                        && x.Department == department
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                        else
                        {
                            jobPost = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.PublishToCareers == isCareer && x.JobCategory == JobCategory.Active_Job
                                        && x.LocationId == locationId
                                        && x.Department == department
                                        && x.JobPostId == jopPostId
                                        ).Select(x => new JobOpeningList
                                        {
                                            JobPostId = x.JobPostId,
                                            JobTitle = x.JobTitle,
                                            Details = x.Department + " " + x.Location,
                                            Location = x.Location,
                                            Department = x.Department,
                                        }).ToListAsync();
                        }
                    }
                }

                if (jobPost.Count > 0)
                {
                    res.Message = "Job Post List";
                    res.Status = true;
                    res.Data = jobPost;
                }
                else
                {
                    res.Message = "List is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get List Of Job With Filter

        #region Api To Get Job Details Page

        /// <summary>
        /// API >> Get >> api/job/jobdetailscareer
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        /// <param name="jobPostId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("jobdetailscareer")]
        public async Task<ResponseBodyModel> GetJobDetailOnCarrer(int jobPostId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.IsActive == true && x.IsDeleted == false && x.JobPostId == jobPostId && x.CompanyId == claims.companyId);
                if (job != null)
                {
                    GetJobDetailsOnCareer obj = new GetJobDetailsOnCareer()
                    {
                        JobId = job.JobPostId,
                        JobTitle = job.JobTitle,
                        Details = job.JobTitle + " - " +
                                      job.JobType + " - " +
                                      "(" + job.Location + ")" + " - " +
                                         ((job.MinExperience == "" && job.MaxExperience == "") ? "Not Mention"
                                        : (job.MinExperience != "" && job.MaxExperience == "" ? job.MinExperience
                                        : (job.MinExperience == "" && job.MaxExperience != "") ? job.MaxExperience
                                        : (job.MinExperience != "" && job.MaxExperience != "") ? job.MinExperience + " to " + job.MaxExperience
                                        : "")),
                        JobDescription = job.JobDescription,
                    };
                    res.Message = "Job Details";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Job Post Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Job Details Page

        #region Api To Get Job List On Carrer and Potal

        /// <summary>
        /// API >> Get >> api/job/getjoblistfilter
        /// Created By Harshit Mitra on 23-02-2022
        /// </summary>
        /// <param name="isCareer"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getjoblistfilter")]
        public async Task<ResponseBodyModel> GetJobListOnCarrerAndPotal(bool isCareer)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobList = await _db.JobPosts.Where(x => x.IsDeleted == false && x.IsActive == true && x.PublishToCareers == isCareer
                                && x.JobCategory == JobCategory.Active_Job && x.CompanyId == claims.companyId).Select(X => new
                                {
                                    JobId = X.JobPostId,
                                    X.JobTitle,
                                    X.Department,
                                    X.Location,
                                    Details = X.JobTitle + " - " +
                                     X.JobType + " - " +
                                     "(" + X.Location + ")" + " - " +
                                        ((X.MinExperience == "" && X.MaxExperience == "") ? "Not Mention"
                                        : (X.MinExperience != "" && X.MaxExperience == "" ? X.MinExperience
                                        : (X.MinExperience == "" && X.MaxExperience != "") ? X.MaxExperience
                                        : (X.MinExperience != "" && X.MaxExperience != "") ? X.MinExperience + " to " + X.MaxExperience
                                        : "")),
                                    X.JobDescription,
                                }).ToListAsync();
                if (jobList.Count > 0)
                {
                    res.Message = "Job List";
                    res.Status = true;
                    res.Data = jobList;
                }
                else
                {
                    res.Message = "Job List Is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Job List On Carrer and Potal

        #region Api For Get All Jobs

        /// <summary>
        /// api/job/getstagedropdown
        /// Created On 24-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getstagedropdown")]
        public async Task<ResponseBodyModel> GetStagedDropDown(int jobPostId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<JobPost> list = new List<JobPost>();
            try
            {
                var job = await _db.JobPosts.Where(x => x.IsActive == true && x.IsDeleted == false && x.JobPostId == jobPostId).FirstOrDefaultAsync();
                if (job != null)
                {
                    var hiringflow = await _db.HiringFlow.FirstOrDefaultAsync(x => x.HiringFlowId == job.HiringFlow);
                    if (hiringflow != null)
                    {
                        var stagedIds = hiringflow.OrderStagesId.Split(',')?.Select(Int32.Parse)?.ToList();
                        var staged = await _db.Stages.Where(x => stagedIds.Contains(x.StageId))
                                .Select(x => new
                                {
                                    x.StageId,
                                    x.StageName,
                                }).ToListAsync();
                        res.Message = "Stage List";
                        res.Status = true;
                        res.Data = staged.OrderBy(x => x.StageId).ToList();
                    }
                    else
                    {
                        res.Message = "Hiring Flow Not Found";
                        res.Status = false;
                        res.Data = new List<int>();
                    }
                }
                else
                {
                    res.Message = "Job Not Found";
                    res.Status = false;
                    res.Data = new List<int>();
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Get All Jobs

        #region This Api Use to Add Reason

        /// <summary>
        /// Created By Ankit On 18-08-2022
        /// Api >> Post >> api/job/addreason
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addreason")]
        public async Task<ResponseBodyModel> AddReason(ReasoneRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = true;
                }
                else
                {
                    var candiatedata = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId);
                    if (candiatedata != null)
                    {
                        var stage = _db.HiringStages.Include("Job").FirstOrDefault(x => x.Job.JobPostId == candiatedata.JobId && x.StageId == model.StageId);
                        if (stage != null)
                        {
                            if (stage.StageType == StageFlowType.Interview)
                            {
                                if (((DateTimeOffset)model.StartTime).ToFileTime() < ((DateTimeOffset)model.EndTime).ToFileTime())
                                {
                                    var stageStatusList = await _db.StageStatuses.Where(x => x.CandidateId == model.CandidateId && x.JobId == stage.Job.JobPostId).ToListAsync();
                                    StageStatus obj = new StageStatus();
                                    obj.CandidateId = model.CandidateId;
                                    obj.EmployeeId = claims.employeeId;
                                    obj.JobId = stage.Job.JobPostId;
                                    obj.StageOrder = stageStatusList.Count();
                                    obj.StageId = (Guid)candiatedata.StageId;
                                    obj.Reason = model.Reason;
                                    obj.CreatedBy = claims.employeeId;
                                    obj.CreatedOn = DateTime.Now;
                                    obj.IsActive = true;
                                    obj.CompanyId = claims.companyId;
                                    obj.OrgId = claims.orgId;
                                    obj.IsDeleted = false;
                                    _db.StageStatuses.Add(obj);
                                    await _db.SaveChangesAsync();

                                    if (stage.StageType == StageFlowType.Interview)
                                    {
                                        CandidateInterview obj2 = new CandidateInterview
                                        {
                                            CandidateId = model.CandidateId,
                                            JobId = candiatedata.JobId,
                                            InterviewType = model.InterviewType,
                                            InterviewTypeName = Enum.GetName(typeof(InterviewType), model.InterviewType).Replace("_", " "),
                                            EmployeeId = model.EmployeeId,
                                            EndTime = model.EndTime,
                                            StartTime = model.StartTime,
                                            Email = candiatedata.Email,
                                            ReviewURL = model.Url,
                                            StageId = (Guid)stage.StageId,
                                            CandidateName = candiatedata.CandidateName,
                                            IsReviewSubmited = false,
                                            MobileNumber = candiatedata.MobileNumber,
                                            IsActive = true,
                                            IsDeleted = false,
                                            CreatedBy = claims.employeeId,
                                            CreatedOn = DateTime.Now,
                                            CompanyId = claims.companyId,
                                            OrgId = claims.orgId
                                        };
                                        _db.CandidateInterviews.Add(obj2);
                                        await _db.SaveChangesAsync();

                                        tokenDataForInterviewMail mailObject = new tokenDataForInterviewMail
                                        {
                                            JobId = candiatedata.JobId,
                                            CurrentId = obj2.Id,
                                            CandidateId = model.CandidateId,
                                            EmployeeId = obj2.EmployeeId,
                                            StageId = model.StageId,
                                            StartTime = model.StartTime,
                                            EndTime = model.EndTime,
                                        };
                                        var key = ConfigurationManager.AppSettings["EncryptKey"];
                                        var data = JsonConvert.SerializeObject(mailObject);
                                        var encryptData = EncryptDecrypt.EncryptData(key, data);
                                        obj2.ReviewURL = model.Url + encryptData;
                                        _db.Entry(obj2).State = System.Data.Entity.EntityState.Modified;
                                        await _db.SaveChangesAsync();

                                        candiatedata.CurrentMeetingSecduleId = obj2.Id;
                                        candiatedata.UpdatedBy = claims.employeeId;
                                        candiatedata.UpdatedOn = DateTime.Now;

                                        _db.Entry(candiatedata).State = System.Data.Entity.EntityState.Modified;
                                        await _db.SaveChangesAsync();
                                        HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread(obj2, claims, MailTypeInHiring.Sechedule));

                                        if (stage.StageType == StageFlowType.Preboarding)
                                        {
                                            candiatedata.PrebordingEmployeeId = model.EmployeeId;
                                            candiatedata.UpdatedBy = claims.employeeId;
                                            candiatedata.UpdatedOn = DateTime.Now;

                                            _db.Entry(candiatedata).State = System.Data.Entity.EntityState.Modified;
                                            await _db.SaveChangesAsync();
                                        }

                                        var employeedata = _db.Employee.Where(x => x.EmployeeId == model.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                                        Notification pcNoti = new Notification
                                        {
                                            Title = stage.StageType == StageFlowType.Interview ? "Interview Sechedule" : candiatedata.CandidateName + " Moved To Next Stage",
                                            Message = stage.StageType == StageFlowType.Interview ? employeedata + " your Interview is sechedule for " + candiatedata.CandidateName + " on " + ((DateTimeOffset)model.StartTime).ToString() :
                                                    candiatedata.CandidateName + " moved to " + stage.StageName + " stage " + "in " + stage.Job.JobTitle,
                                            CreateDate = DateTime.Now,
                                            IsActive = true,
                                            IsDeleted = false,
                                            EmployeeId = model.EmployeeId,
                                            ForPC = true,
                                            CompanyId = claims.companyId,
                                        };
                                        NotificationController _noti = new NotificationController();
                                        _ = await _noti.AddNotification(pcNoti);
                                        res.Message = "Added Satage Reason";
                                        res.Status = true;
                                        res.Data = obj;
                                    }
                                }
                                else
                                {
                                    res.Message = "Start Time Should Be Smaller Then End Time";
                                    res.Status = false;
                                }
                            }
                            else
                            {
                                var stageStatusList = await _db.StageStatuses.Where(x => x.CandidateId == model.CandidateId && x.JobId == stage.Job.JobPostId).ToListAsync();
                                StageStatus obj = new StageStatus();
                                obj.CandidateId = model.CandidateId;
                                obj.EmployeeId = claims.employeeId;
                                obj.JobId = stage.Job.JobPostId;
                                obj.StageOrder = stageStatusList.Count();
                                obj.StageId = (Guid)candiatedata.StageId;
                                obj.Reason = model.Reason;
                                obj.CreatedBy = claims.employeeId;
                                obj.CreatedOn = DateTime.Now;
                                obj.IsActive = true;
                                obj.CompanyId = claims.companyId;
                                obj.OrgId = claims.orgId;
                                obj.IsDeleted = false;
                                _db.StageStatuses.Add(obj);
                                await _db.SaveChangesAsync();


                                if (stage.StageType == StageFlowType.Interview)
                                {
                                    CandidateInterview obj2 = new CandidateInterview
                                    {
                                        CandidateId = model.CandidateId,
                                        JobId = candiatedata.JobId,
                                        InterviewType = model.InterviewType,
                                        InterviewTypeName = Enum.GetName(typeof(InterviewType), model.InterviewType).Replace("_", " "),
                                        EmployeeId = model.EmployeeId,
                                        EndTime = model.EndTime,
                                        StartTime = model.StartTime,
                                        Email = candiatedata.Email,
                                        ReviewURL = model.Url,
                                        StageId = (Guid)stage.StageId,
                                        CandidateName = candiatedata.CandidateName,
                                        MobileNumber = candiatedata.MobileNumber,
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreatedBy = claims.employeeId,
                                        CreatedOn = DateTime.Now,
                                        CompanyId = claims.companyId,
                                        OrgId = claims.orgId
                                    };
                                    _db.CandidateInterviews.Add(obj2);
                                    await _db.SaveChangesAsync();

                                    tokenDataForInterviewMail mailObject = new tokenDataForInterviewMail
                                    {
                                        JobId = candiatedata.JobId,
                                        CurrentId = obj2.Id,
                                        CandidateId = model.CandidateId,
                                        EmployeeId = obj2.EmployeeId,
                                        StageId = model.StageId,
                                        StartTime = model.StartTime,
                                        EndTime = model.EndTime,
                                    };
                                    var key = ConfigurationManager.AppSettings["EncryptKey"];
                                    var data = JsonConvert.SerializeObject(mailObject);
                                    var encryptData = EncryptDecrypt.EncryptData(key, data);
                                    obj2.ReviewURL = model.Url + encryptData;
                                    _db.Entry(obj2).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();


                                    candiatedata.CurrentMeetingSecduleId = obj2.Id;
                                    candiatedata.UpdatedBy = claims.employeeId;
                                    candiatedata.UpdatedOn = DateTime.Now;

                                    _db.Entry(candiatedata).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();
                                    HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread(obj2, claims, MailTypeInHiring.Sechedule));
                                }
                                if (stage.StageType == StageFlowType.Preboarding)
                                {
                                    candiatedata.PrebordingEmployeeId = model.EmployeeId;
                                    candiatedata.UpdatedBy = claims.employeeId;
                                    candiatedata.UpdatedOn = DateTime.Now;

                                    _db.Entry(candiatedata).State = System.Data.Entity.EntityState.Modified;
                                    await _db.SaveChangesAsync();
                                }

                                var employeedata = _db.Employee.Where(x => x.EmployeeId == model.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                                Notification pcNoti = new Notification
                                {
                                    Title = stage.StageType == StageFlowType.Interview ? "Interview Sechedule" : candiatedata.CandidateName + " Moved To Next Stage",
                                    Message = stage.StageType == StageFlowType.Interview ? employeedata + " your Interview is sechedule for " + candiatedata.CandidateName + " on " + ((DateTimeOffset)model.StartTime).ToString() :
                                            candiatedata.CandidateName + " moved to " + stage.StageName + " stage " + "in " + stage.Job.JobTitle,
                                    CreateDate = DateTime.Now,
                                    IsActive = true,
                                    IsDeleted = false,
                                    EmployeeId = model.EmployeeId,
                                    ForPC = true,
                                    CompanyId = claims.companyId,
                                };
                                NotificationController _noti = new NotificationController();
                                _ = await _noti.AddNotification(pcNoti);


                                res.Message = "Added Satage Reason";
                                res.Status = true;
                                res.Data = obj;
                            }
                        }
                        else
                        {
                            res.Message = "Stage Not Found";
                            res.Status = false;
                        }

                    }
                    else
                    {
                        res.Message = "Candiate Not Found";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public void SendMailInThread(CandidateInterview obj, ClaimsHelperModel claims, MailTypeInHiring mailType, string cancleReason = null)
        {
            switch (mailType)
            {
                case MailTypeInHiring.Sechedule:
                    Thread.Sleep(5000); // 1000 = 1 sec
                    _ = AddMeetingScheduleDate(obj.CandidateId, obj.EmployeeId, obj.InterviewType, obj.StageId, claims);
                    break;

                case MailTypeInHiring.Resechedule:
                    Thread.Sleep(5000); // 1000 = 1 sec
                    _ = ReAddMeetingScheduleDate(obj.CandidateId, obj.EmployeeId, obj.InterviewType, obj.StageId, claims);
                    break;

                case MailTypeInHiring.Cancled:
                    _ = ReAddMeetingScheduleDate(obj.CandidateId, obj.EmployeeId, obj.InterviewType, obj.StageId, claims);
                    break;
            }

        }
        public class ReasoneRequest
        {
            public int EmployeeId { get; set; }
            public Guid StageId { get; set; }
            public int CandidateId { get; set; }
            public string Reason { get; set; }
            public int InterviewType { get; set; }
            public DateTimeOffset? EndTime { get; set; }
            public DateTimeOffset? StartTime { get; set; }
            public string Url { get; set; }
        }
        public class tokenDataForInterviewMail
        {
            public int JobId { get; set; }
            public int CurrentId { get; set; }
            public int CandidateId { get; set; }
            public int EmployeeId { get; set; }
            public Guid StageId { get; set; }
            public DateTimeOffset? EndTime { get; set; }
            public DateTimeOffset? StartTime { get; set; }
            public int ReScheduleCount { get; set; } = 0;
        }
        #endregion

        #region Api To Add Candidate Interview

        /// <summary>s
        /// API >> Post >> api/job/Readdmeetingscheduledate
        /// Created By Harshit Mitra on 02-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Readdmeetingscheduledate")]
        public async Task<ResponseBodyModel> ReAddMeetingScheduleDate(int candidateId, int employeeId, int interviewType, Guid reseheduleStageId, ClaimsHelperModel claims)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                logger.Debug("Enter Into AddMeetingScheduleDate");
                var candidate = _db.CandidateInterviews.Where(x => x.CandidateId == candidateId && x.CompanyId == claims.companyId).ToList().LastOrDefault();
                if (candidate != null)
                {
                    //var hiringteamrecuruiterId = await _db.HiringTeams.Where(x => x.Job.JobPostId == candidate.JobId && x.Designation == HiringTeamEnum.Recruiters).Select(x => x.Employee.EmployeeId).FirstOrDefaultAsync();
                    if (interviewType == 1)
                    {

                        logger.Debug("Candidate Found");

                        MeetingScheduleInterview obj = new MeetingScheduleInterview
                        {
                            CandidateId = candidate.CandidateId,
                            JobId = candidate.JobId,
                            CandidateName = candidate.CandidateName,
                            MobileNumber = candidate.MobileNumber,
                            StartTime = candidate.StartTime,
                            EndTime = candidate.EndTime,
                            InterviewType = candidate.InterviewType,
                            CandidateEmail = candidate.Email,
                            TimeZone = "Indian Time",
                            Subject = "Interview Rescheduled",
                            Message = "Interview Rescheduled",
                            InterviwerEmail = _db.Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            //HrEmail = _db.Employee.Where(x => x.OrgId == claims.orgid && x.CompanyId == claims.companyid).Select(x => x.OfficeEmail).FirstOrDefault(),
                            RecruitersEmail = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            MettingId = candidate.MettingId,
                            InterviewStatusvalue = candidate.InterviewStatusvalue,
                            MettingUrl = candidate.MeetingURL,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.meetingScheduleInterviews.Add(obj);
                        await _db.SaveChangesAsync();

                        logger.Debug("Meeting Schedule Interview Created");

                        res.Message = "Interview Rescheduled";
                        res.Status = true;
                        res.Data = obj;
                        try
                        {
                            logger.Debug("Enter Into Try Block");

                            var baseUrl = "https://prod-26.centralindia.logic.azure.com/workflows/dc24f457e12c43159af18304e59602cc/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=ULLPWFoOSC5Qkd8OxrHk4DSkqsC9V6z2ITRphGNKSNI";
                            string s = JsonConvert.SerializeObject(obj);

                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            var client = new RestClient(baseUrl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            request.AddParameter("application/json", s, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            if (candidate.InterviewStatusvalue == InterviewStatus.Rescheduled)
                            {
                                var candidateInterview = _db.CandidateInterviews.Where(x => x.CandidateId == candidateId).ToList().LastOrDefault();
                                candidateInterview.MeetingURL = obj.MettingUrl;
                                candidateInterview.MettingId = obj.MettingId;
                                _db.Entry(candidateInterview).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();

                                _ = ReSendMail(candidateInterview, obj, claims);

                                dynamic myDeserializedClass = response.Content;

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    logger.Debug("Link Genetated Status Code 200");
                                    res.Message = "Sucess";
                                    res.Status = true;
                                    res.Data = myDeserializedClass;
                                }
                            }
                            else
                            {
                                _ = CancelIntreviewCandidate(obj, claims);
                            }
                        }
                        catch (WebException ex)
                        {
                            logger.Debug("Error in schedule Interview : " + ex.Message);
                            res.Message = ex.Message;
                            res.Status = false;
                        }
                    }
                    else
                    {
                        MeetingScheduleInterview obj = new MeetingScheduleInterview
                        {
                            CandidateId = candidate.CandidateId,
                            JobId = candidate.JobId,
                            CandidateName = candidate.CandidateName,
                            MobileNumber = candidate.MobileNumber,
                            StartTime = candidate.StartTime,
                            EndTime = candidate.EndTime,
                            InterviewType = candidate.InterviewType,
                            CandidateEmail = candidate.Email,
                            TimeZone = "Indian Time",
                            Subject = "Interview Schedule",
                            Message = "Interview Schedule",
                            InterviwerEmail = _db.Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            //HrEmail = _db.Employee.Where(x => x.OrgId == claims.orgid && x.CompanyId == claims.companyid).Select(x => x.OfficeEmail).FirstOrDefault(),
                            RecruitersEmail = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.meetingScheduleInterviews.Add(obj);
                        await _db.SaveChangesAsync();
                        await SendMailFaceToface(obj, claims);
                        res.Message = "Sucess";
                        res.Status = true;
                        res.Data = obj;

                    }
                }
                else
                {
                    logger.Debug("Candidate Not Found");
                    res.Message = "Candidate Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            logger.Debug("Code Is Working");
            return res;
        }

        #endregion Api To Add Candidate Interview

        #region This Api Use to Reschedule

        /// <summary>
        /// Created By Ankit On 18-08-2022
        /// Api >> Post >> api/job/rescheduleinterview
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("rescheduleinterview")]
        public async Task<ResponseBodyModel> ReSchedule(Reschedule model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = true;
                }
                else
                {
                    var candiatedata = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId);
                    if (candiatedata != null)
                    {
                        if (model.InterviewStatusvalue == InterviewStatus.Rescheduled)
                        {
                            var stage = _db.HiringStages.Include("Job").FirstOrDefault(x => x.Job.JobPostId == candiatedata.JobId && x.StageId == model.StageId);
                            if (stage != null)
                            {

                                if (stage.StageType == StageFlowType.Interview)
                                {
                                    if (((DateTimeOffset)model.StartTime).ToFileTime() < ((DateTimeOffset)model.EndTime).ToFileTime())
                                    {
                                        var candidateinterview = _db.CandidateInterviews.Where(x => x.Id == candiatedata.CurrentMeetingSecduleId).FirstOrDefault();
                                        if (candidateinterview != null)
                                        {

                                            candidateinterview.InterviewType = model.InterviewType;
                                            candidateinterview.InterviewTypeName = Enum.GetName(typeof(InterviewType), model.InterviewType).Replace("_", " ");
                                            candidateinterview.EmployeeId = model.EmployeeId;
                                            candidateinterview.EndTime = (DateTimeOffset)model.EndTime;
                                            candidateinterview.StartTime = (DateTimeOffset)model.StartTime;
                                            candidateinterview.Email = candiatedata.Email;
                                            candidateinterview.ReviewURL = model.Url;
                                            candidateinterview.MobileNumber = candiatedata.MobileNumber;
                                            candidateinterview.UpdatedBy = claims.employeeId;
                                            candidateinterview.UpdatedOn = DateTime.Now;
                                            candidateinterview.IsReschedule = true;
                                            candidateinterview.RescheduleCount += 1;
                                            candidateinterview.IsReviewSubmited = false;
                                            candidateinterview.InterviewStatusvalue = model.InterviewStatusvalue;


                                            tokenDataForInterviewMail mailObject = new tokenDataForInterviewMail
                                            {
                                                JobId = candiatedata.JobId,
                                                CurrentId = candiatedata.CurrentMeetingSecduleId,
                                                EmployeeId = candidateinterview.EmployeeId,
                                                CandidateId = model.CandidateId,
                                                StageId = model.StageId,
                                                StartTime = model.StartTime,
                                                EndTime = model.EndTime,
                                                ReScheduleCount = candidateinterview.RescheduleCount,
                                            };
                                            var key = ConfigurationManager.AppSettings["EncryptKey"];
                                            var data = JsonConvert.SerializeObject(mailObject);
                                            var encryptData = EncryptDecrypt.EncryptData(key, data);
                                            candidateinterview.ReviewURL = model.Url + encryptData;
                                            _db.Entry(candidateinterview).State = System.Data.Entity.EntityState.Modified;
                                            await _db.SaveChangesAsync();

                                            candiatedata.CurrentMeetingSecduleId = candidateinterview.Id;
                                            candiatedata.UpdatedBy = claims.employeeId;
                                            candiatedata.UpdatedOn = DateTime.Now;

                                            _db.Entry(candiatedata).State = System.Data.Entity.EntityState.Modified;
                                            await _db.SaveChangesAsync();

                                            HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread(candidateinterview, claims, MailTypeInHiring.Resechedule));

                                            var employeedata = _db.Employee.Where(x => x.EmployeeId == model.EmployeeId).Select(x => x.DisplayName).FirstOrDefault();
                                            Notification pcNoti = new Notification
                                            {
                                                Title = stage.StageType == StageFlowType.Interview ? "Interview schedule" : candiatedata.CandidateName + " Moved To Next Stage",
                                                Message = stage.StageType == StageFlowType.Interview ? employeedata + " your Interview is schedule for " + candiatedata.CandidateName + " on " + ((DateTimeOffset)model.StartTime).ToString() :
                                                        candiatedata.CandidateName + " moved to " + stage.StageName + " stage " + "in " + stage.Job.JobTitle,
                                                CreateDate = DateTime.Now,
                                                IsActive = true,
                                                IsDeleted = false,
                                                EmployeeId = model.EmployeeId,
                                                ForPC = true,
                                                CompanyId = claims.companyId,
                                            };
                                            NotificationController _noti = new NotificationController();
                                            _ = await _noti.AddNotification(pcNoti);

                                            res.Data = candidateinterview;
                                            res.Message = "Added Satage Reason";
                                            res.Status = true;

                                        }
                                        else
                                        {
                                            res.Message = "candidateinterview Not Found";
                                            res.Status = false;
                                        }
                                    }
                                    else
                                    {
                                        res.Message = "Start Time Should Be Smaller Then End Time";
                                        res.Status = false;
                                    }
                                }
                                else
                                {
                                    res.Message = "Stage Type Not Interview";
                                    res.Status = false;
                                }
                            }
                            else
                            {
                                res.Message = "Stage Not Found";
                                res.Status = false;
                            }

                        }

                        else
                        {
                            var candidateinterviewdata = _db.CandidateInterviews.Where(x => x.Id == candiatedata.CurrentMeetingSecduleId).FirstOrDefault();
                            candidateinterviewdata.InterviewType = candidateinterviewdata.InterviewType;
                            candidateinterviewdata.InterviewTypeName = null;
                            candidateinterviewdata.EmployeeId = candidateinterviewdata.EmployeeId;
                            candidateinterviewdata.EndTime = (DateTimeOffset)candidateinterviewdata.EndTime;
                            candidateinterviewdata.StartTime = (DateTimeOffset)candidateinterviewdata.StartTime;
                            candidateinterviewdata.Email = candiatedata.Email;
                            candidateinterviewdata.ReviewURL = null;
                            candidateinterviewdata.MobileNumber = candiatedata.MobileNumber;
                            candidateinterviewdata.UpdatedBy = claims.employeeId;
                            candidateinterviewdata.UpdatedOn = DateTime.Now;
                            candidateinterviewdata.IsReschedule = false;
                            candidateinterviewdata.RescheduleCount += 1;
                            candidateinterviewdata.CancelReason = model.CancelReason;
                            candidateinterviewdata.InterviewStatusvalue = model.InterviewStatusvalue;
                            _db.Entry(candidateinterviewdata).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Data = candidateinterviewdata;
                            res.Message = "Cancel Status Reason";
                            res.Status = true;

                            //HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread(candidateinterview, claims, MailTypeInHiring.Cancled, candidateinterview.CancelReason));
                            HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailInThread(candidateinterviewdata, claims, MailTypeInHiring.Cancled));
                        }
                    }
                    else
                    {
                        res.Message = "Candiate Not Found";
                        res.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }



        public class Reschedule
        {
            public int EmployeeId { get; set; }
            public Guid StageId { get; set; }
            public int CandidateId { get; set; }
            public int InterviewType { get; set; }
            public DateTimeOffset? EndTime { get; set; }
            public DateTimeOffset? StartTime { get; set; }
            public string Url { get; set; }
            public InterviewStatus InterviewStatusvalue { get; set; }
            public string CancelReason { get; set; }
        }
        #endregion

        #region Api To Add Candidate Interview

        /// <summary>
        /// API >> Post >> api/job/addmeetingscheduledate
        /// Created By Harshit Mitra on 02-03-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addmeetingscheduledate")]
        public async Task<ResponseBodyModel> AddMeetingScheduleDate(int candidateId, int employeeId, int interviewType, Guid stageId, ClaimsHelperModel claims)
        {
            List<MultipleInterview> list = new List<MultipleInterview>();
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                logger.Debug("Enter Into AddMeetingScheduleDate");
                var candidate = _db.CandidateInterviews.Where(x => x.CandidateId == candidateId
                                && x.CompanyId == claims.companyId).ToList().LastOrDefault();
                if (candidate != null)
                {

                    if (interviewType == 1)
                    {

                        logger.Debug("Candidate Found");
                        MeetingScheduleInterview obj = new MeetingScheduleInterview
                        {
                            CandidateId = candidate.CandidateId,
                            JobId = candidate.JobId,
                            CandidateName = candidate.CandidateName,
                            MobileNumber = candidate.MobileNumber,
                            StartTime = candidate.StartTime,
                            EndTime = candidate.EndTime,
                            InterviewType = candidate.InterviewType,
                            CandidateEmail = candidate.Email,
                            TimeZone = "Indian Time",
                            Subject = "Interview Schedule",
                            Message = "Interview schedule",
                            InterviwerEmail = _db.Employee.Where(x => x.EmployeeId == employeeId)
                                             .Select(x => x.OfficeEmail).FirstOrDefault(),
                            //InterviwerEmail = JsonConvert.SerializeObject(dev),
                            RecruitersEmail = _db.Employee.Where(x => x.EmployeeId == claims.employeeId)
                                             .Select(x => x.OfficeEmail).FirstOrDefault(),
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.meetingScheduleInterviews.Add(obj);
                        await _db.SaveChangesAsync();

                        logger.Debug("Meeting Schedule Interview Created");

                        res.Message = "Interview Schedule";
                        res.Status = true;
                        res.Data = obj;
                        try
                        {
                            logger.Debug("Enter Into Try Block");

                            var baseUrl = "https://prod-26.centralindia.logic.azure.com/workflows/dc24f457e12c43159af18304e59602cc/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=ULLPWFoOSC5Qkd8OxrHk4DSkqsC9V6z2ITRphGNKSNI";
                            string s = JsonConvert.SerializeObject(obj);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            var client = new RestClient(baseUrl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            request.AddParameter("application/json", s, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response.Content);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                logger.Debug("Link Genetated Status Code 200");
                                res.Message = "Sucess";
                                res.Status = true;
                                res.Data = myDeserializedClass;

                                var candidateInterview = _db.CandidateInterviews.Where(x => x.CandidateId == candidateId && x.StageId == stageId).ToList().LastOrDefault();
                                candidateInterview.MeetingURL = myDeserializedClass.joinUrl;
                                candidateInterview.MettingId = myDeserializedClass.ID;
                                _db.Entry(candidateInterview).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();

                                _ = SendMail(candidateInterview, obj, claims);
                            }
                        }
                        catch (WebException ex)
                        {
                            logger.Debug("Error in Sechdule Interview : " + ex.Message);
                            res.Message = ex.Message;
                            res.Status = false;
                        }
                    }
                    else
                    {
                        MeetingScheduleInterview obj = new MeetingScheduleInterview
                        {
                            CandidateId = candidate.CandidateId,
                            JobId = candidate.JobId,
                            CandidateName = candidate.CandidateName,
                            MobileNumber = candidate.MobileNumber,
                            StartTime = candidate.StartTime,
                            EndTime = candidate.EndTime,
                            InterviewType = candidate.InterviewType,
                            CandidateEmail = candidate.Email,
                            TimeZone = "Indian Time",
                            Subject = "Interview schedule",
                            Message = "Interview schedule",
                            InterviwerEmail = _db.Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            //HrEmail = _db.Employee.Where(x => x.OrgId == claims.orgid && x.CompanyId == claims.companyid).Select(x => x.OfficeEmail).FirstOrDefault(),
                            RecruitersEmail = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.meetingScheduleInterviews.Add(obj);
                        await _db.SaveChangesAsync();
                        _ = SendMailFaceToface(obj, claims);
                        res.Message = "Sucess";
                        res.Status = true;
                        res.Data = obj;
                    }
                }
                else
                {
                    logger.Debug("Candidate Not Found");
                    res.Message = "Candidate Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            logger.Debug("Code Is Working");
            return res;
        }
        public class MultipleInterview
        {
            public string Email { get; set; }
        }
        public class Root
        {
            public string joinUrl { get; set; }
            public string ID { get; set; }
        }
        #endregion Api To Add Candidate Interview

        #region This Api Use To Send Teams Link
        ///// <summary>
        ///// Create By ankit Date-04-10-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("test")]
        public async Task SendMail(CandidateInterview interview, MeetingScheduleInterview meet, ClaimsHelperModel claims)
        {
            try
            {
                logger.Debug("Enter Into Send Mail");

                var candidateinterview = (from c in _db.Candidates
                                          join j in _db.JobPosts on c.JobId equals j.JobPostId
                                          where c.CandidateId == interview.CandidateId
                                          select new
                                          {
                                              c.CandidateName,
                                              c.CompanyName,
                                              j.JobTitle,
                                              c.InterViewType,
                                              interview.ReviewURL,
                                              interview.MeetingURL,
                                              c.UploadResume
                                          }).FirstOrDefault();
                var customizeTemplate = _db.HiringTemplates.Where(y => y.CompanyId == claims.companyId
                        && y.IsActive && !y.IsDeleted && y.Templatetype == TemplateTypeConstants.InterviewSchedule)
                      .Select(x => new
                      {
                          x.IsCustmized,
                          x.TemplateForCandidate,
                          x.TemplateForInterviewer,
                          x.TemplateForRecruiter,
                      }).FirstOrDefault();
                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted
                          && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var intervierName = _db.Employee.Where(x => x.OfficeEmail == meet.InterviwerEmail)
                        .Select(x => x.DisplayName).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId
                         && x.OfficeEmail == meet.RecruitersEmail)
                         .Select(x => x.DisplayName).FirstOrDefault();
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                        && y.IsActive && !y.IsDeleted)
                      .Select(x => new
                      {
                          x.RegisterAddress,
                          x.RegisterCompanyName,
                      }).FirstOrDefault();
                var st = (DateTimeOffset)interview.StartTime;
                var startDate = st.ToString("dd/MM/yyyy");
                var startTime = st.ToString("hh:mm tt");

                var et = (DateTimeOffset)interview.EndTime;
                var endTime = et.ToString("hh:mm tt");

                if (meet.CandidateEmail != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.InterviewScheduleForCandidate
                        .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview Link Url",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.CandidateEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (candidateinterview.MeetingURL != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.ScheduleInterviewdforRecruiter
                        .Replace("<|INTERVIEWERNAME|>", intervierName)
                        .Replace("<|RECRUITERNAME|>", recruiterName)
                        .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview Link Url",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.RecruitersEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (candidateinterview.ReviewURL != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.ScheduleInterviewdforInterviewer
                      .Replace("<|INTERVIERNAME|>", intervierName)
                      .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                      .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                      .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                      .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                      .Replace("<|STARTTIME|>", startTime)
                      .Replace("<|IMAGE_PATH|>", "emossy.png")
                      .Replace("<|STARTDATE|>", startDate)
                      .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                      .Replace("<|REVIEWLINK|>", candidateinterview.ReviewURL)
                      .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                      .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                      .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                      .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                      .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview And Review Link Urls",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.InterviwerEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Send Mail Error : " + ex.Message);
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region This Api Use To ReSend Teams Link Candidate
        ///// <summary>
        ///// Create By ankit Date-04-10-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("testresschedule")]
        public async Task ReSendMail(CandidateInterview interview, MeetingScheduleInterview meet, ClaimsHelperModel claims)
        {
            try
            {
                logger.Debug("Enter Into Send Mail");

                var candidateinterview = (from c in _db.Candidates
                                          join j in _db.JobPosts on c.JobId equals j.JobPostId
                                          where c.CandidateId == interview.CandidateId
                                          select new
                                          {
                                              c.CandidateName,
                                              c.CompanyName,
                                              j.JobTitle,
                                              c.InterViewType,
                                              interview.ReviewURL,
                                              interview.MeetingURL,
                                              c.UploadResume
                                          }).FirstOrDefault();
                var customizeTemplate = _db.HiringTemplates.Where(y => y.CompanyId == claims.companyId
                        && y.IsActive && !y.IsDeleted && y.Templatetype == TemplateTypeConstants.InterviewSchedule)
                      .Select(x => new
                      {
                          x.IsCustmized,
                          x.TemplateForCandidate,
                          x.TemplateForInterviewer,
                          x.TemplateForRecruiter,
                      }).FirstOrDefault();
                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var intervierName = _db.Employee.Where(x => x.OfficeEmail == meet.InterviwerEmail).Select(x => x.DisplayName).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId && x.OfficeEmail == meet.RecruitersEmail).Select(x => x.DisplayName).FirstOrDefault();
                var st = (DateTimeOffset)interview.StartTime;
                var startDate = st.ToString("dd/MM/yyyy");
                var startTime = st.ToString("hh:mm tt");

                var et = (DateTimeOffset)interview.EndTime;
                var endTime = et.ToString("hh:mm tt");
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                       && y.IsActive && !y.IsDeleted)
                     .Select(x => new
                     {
                         x.RegisterAddress,
                         x.RegisterCompanyName

                     }).FirstOrDefault();
                if (meet.CandidateEmail != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.RescheduleInterviewCandidate
                        .Replace("<|CANDIDATENAME|>", candidateinterview.CandidateName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview Link Url",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.CandidateEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (candidateinterview.MeetingURL != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.RescheduleInterviewRecruiter
                        .Replace("<|INTERVIEWERNAME|>", intervierName)
                        .Replace("<|RECRUITERNAME|>", recruiterName)
                        .Replace("<|CANDIDATENAME|>", candidateinterview.CandidateName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                      .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview Link Url",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.RecruitersEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (candidateinterview.ReviewURL != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.RescheduleInterviewInterviewer
                      .Replace("<|INTERVIERNAME|>", intervierName)
                      .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                      .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                      .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                      .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterViewType)
                      .Replace("<|STARTTIME|>", startTime)
                      .Replace("<|IMAGE_PATH|>", "emossy.png")
                      .Replace("<|STARTDATE|>", startDate)
                      .Replace("<|TEAMSLINK|>", candidateinterview.MeetingURL)
                      .Replace("<|REVIEWLINK|>", candidateinterview.ReviewURL)
                      .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                      .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                      .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                      .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                      .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Your Interview And Review Link Urls",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.InterviwerEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }


            }
            catch (Exception ex)
            {
                logger.Debug("Send Mail Error : " + ex.Message);
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region This Api Use To Send Teams Link
        ///// <summary>
        ///// Create By ankit Date-04-10-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("test")]
        public async Task SendMailFaceToface(MeetingScheduleInterview meet, ClaimsHelperModel claims)
        {
            try
            {
                var candidateinterview = (from c in _db.Candidates
                                          join j in _db.JobPosts on c.JobId equals j.JobPostId
                                          join ci in _db.CandidateInterviews on c.CandidateId equals ci.CandidateId
                                          where c.CandidateId == meet.CandidateId
                                          select new
                                          {
                                              c.CandidateName,
                                              c.CompanyName,
                                              c.InterViewType,
                                              ci.InterviewTypeName,
                                              j.JobTitle,
                                              c.UploadResume
                                          }).FirstOrDefault();

                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var intervierName = _db.Employee.Where(x => x.OfficeEmail == meet.InterviwerEmail).Select(x => x.DisplayName).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId && x.OfficeEmail == meet.RecruitersEmail).Select(x => x.DisplayName).FirstOrDefault();
                var st = (DateTimeOffset)meet.StartTime;
                var startDate = st.ToString("dd/MM/yyyy");
                var startTime = st.ToString("hh:mm tt");
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                      && y.IsActive && !y.IsDeleted)
                    .Select(x => new
                    {
                        x.RegisterAddress,
                        x.RegisterCompanyName

                    }).FirstOrDefault();
                var et = (DateTimeOffset)meet.EndTime;
                var endTime = et.ToString("hh:mm tt");
                var customizeTemplate = _db.HiringTemplates.Where(y => y.CompanyId == claims.companyId
                        && y.IsActive && !y.IsDeleted && y.Templatetype == TemplateTypeConstants.InterviewSchedule)
                      .Select(x => new
                      {
                          x.IsCustmized,
                          x.TemplateForCandidate,
                          x.TemplateForInterviewer,
                          x.TemplateForRecruiter,
                      }).FirstOrDefault();
                if (meet.CandidateEmail != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.FaceToFaceInterviewCandidate
                        .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterviewTypeName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Face to Face Interview",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.CandidateEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (meet.InterviwerEmail != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.FacetoFaceInterviewInterviewer
                        .Replace("<|INTERVIEWERNAME|>", intervierName)
                        .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterviewTypeName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Face to Face Interview",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.InterviwerEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                if (meet.RecruitersEmail != null)
                {
                    SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                    if (claims.IsSmtpProvided)
                    {
                        smtpsettings = _db.CompanySmtpMailModels
                            .Where(x => x.CompanyId == claims.companyId)
                            .Select(x => new SmtpSendMailRequest
                            {
                                From = x.From,
                                SmtpServer = x.SmtpServer,
                                MailUser = x.MailUser,
                                MailPassword = x.Password,
                                Port = x.Port,
                                ConectionType = x.ConnectType,
                            })
                            .FirstOrDefault();
                    }
                    string htmlBody = JobHelper.FacetoFaceInterviewRecruiter
                        .Replace("<|RECRUITERNAME|>", recruiterName)
                        .Replace("<|INTERVIEWERNAME|>", intervierName)
                        .Replace("<|CANDIDATENAME|>", meet.CandidateName)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                         .Replace("<|INTERVIEWTYPE|>", candidateinterview.InterviewTypeName)
                        .Replace("<|COMPANYNAME|>", candidateinterview.CompanyName)
                        .Replace("<|JOBTITLE|>", candidateinterview.JobTitle)
                        .Replace("<|STARTTIME|>", startTime)
                        .Replace("<|STARTDATE|>", startDate)
                        .Replace("<|RECRUITERPHONE|>", employeedata.MobilePhone)
                        .Replace("<|RECRUITEREMAIL|>", meet.RecruitersEmail)
                        .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Face to Face Interview",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { meet.RecruitersEmail },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This Api Use To Send Cancel intewrview Mail
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// Api >> Get >> api/job/cancelinterviewcandidate
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("cancelinterviewcandidate")]
        public async Task CancelIntreviewCandidate(MeetingScheduleInterview meet, ClaimsHelperModel claims)
        {
            try
            {
                var candidaterevoke = (from c in _db.CandidateInterviews
                                       join m in _db.meetingScheduleInterviews on c.CandidateId equals m.CandidateId
                                       join ci in _db.CandidateInterviews on c.CandidateId equals ci.CandidateId
                                       where c.CandidateId == meet.CandidateId
                                       select new
                                       {
                                           c.Email,
                                           m.InterviwerEmail,
                                           m.RecruitersEmail,
                                           ci.CancelReason,
                                       }).ToList().LastOrDefault();
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                     && y.IsActive && !y.IsDeleted)
                   .Select(x => new
                   {
                       x.RegisterAddress,
                       x.RegisterCompanyName

                   }).FirstOrDefault();
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (claims.IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                string htmlBody = JobHelper.Cancelinterview
                  .Replace("<|CANCELREASON|>", candidaterevoke.CancelReason)
                  .Replace("<|IMAGE_PATH|>", "emossy.png")
                  .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                  .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Cancel Candidate Interview",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { candidaterevoke.Email, candidaterevoke.InterviwerEmail, candidaterevoke.RecruitersEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }


            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This api use for upload Job Document

        /// <summary>
        ///Created By Ankit On 25-05-2022
        /// </summary>Api>>Post>> api/job/uploadjobdocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadjobdocuments")]

        public async Task<UploadImageResponseJob> UploadJobDocments()
        {
            UploadImageResponseJob result = new UploadImageResponseJob();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/jobdocument/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\jobdocument\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path;
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }

        #endregion This api use for upload Job Document

        #region This Api Use To Get All Current date Data for Interview sheduled
        /// <summary>
        /// Created By Ankit Jain on 10/03/2023
        /// API >> Get >> api/job/gettodayinterview
        /// </summary>
        [HttpGet]
        [Route("gettodayinterview")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetTodayInterview()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTime today = DateTime.Today;
                var candidateInterview = await _db.CandidateInterviews.Where
                           (x => x.CreatedOn == today && x.IsActive && !x.IsDeleted).ToListAsync();
                if (candidateInterview.Count > 0)
                {
                    res.Message = "Candidate List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = candidateInterview;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/job/gettodayinterview | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion
        // ------------------ Hiring Team -----------------------//

        #region This Api Use To Add HiringTeam
        /// <summary>
        /// Created By Harshit Mitra On 01-10-2022
        /// API >> Post >> api/job/addhiringteam
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addhiringteam")]
        public async Task<ResponseBodyModel> AddHiringTeam(List<HiringArrayRequest> model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    int JobId = model.First().JobId;
                    var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == JobId);
                    var hiringObject = model
                        .Where(x => x.New)
                        .Select(x => new HiringTeam
                        {
                            HiringTeamId = Guid.NewGuid(),
                            Job = job,
                            Employee = _db.Employee.FirstOrDefault(z => z.EmployeeId == x.EmployeeId),
                            PreboardingHired = false,
                            Designation = x.Designation,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,

                        }).ToList();
                    _db.HiringTeams.AddRange(hiringObject);
                    await _db.SaveChangesAsync();

                    res.Message = "Team Member Added Successfully";
                    res.Status = true;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class HiringArrayRequest
        {
            public int EmployeeId { get; set; }
            public int JobId { get; set; }
            public HiringTeamConstants Designation { get; set; }
            public bool New { get; set; }
        }
        #endregion This Api Use To Add HiringTeam

        #region This Api Use to Get All Hiring Teams

        /// <summary>
        /// Created By Ankit On 18-08-2022
        /// Api >> Get >> api/job/gethiringteam
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gethiringteam")]
        public async Task<ResponseBodyModel> GetHiringTeam(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var hiringTeam = await _db.HiringTeams
                        .Include("Job")
                        .Include("Employee")
                        .Where(x => x.Job.JobPostId == jobId && x.IsActive && !x.IsDeleted)
                        .Select(x => new
                        {
                            x.HiringTeamId,
                            x.Employee.EmployeeId,
                            JobId = x.Job.JobPostId,
                            x.Employee.DisplayName,
                            x.Designation,
                            DesignationName = x.Designation.ToString().Replace("_", " "),
                            New = false,
                        })
                        .ToListAsync();

                res.Message = "Hiring Team Found";
                res.Status = true;
                res.Data = hiringTeam;

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use to Get All Hiring Teams

        #region This Api Use To Remove By Hiring Team Member
        /// <summary>
        /// Created By Harshit Mitra On 01-10-2022
        /// API >> Delete >> api/job/removeemployee
        /// </summary>
        /// <param name="hiringTeamId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removeemployee")]
        public async Task<ResponseBodyModel> RemoveEmployee(Guid hiringTeamId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var teamMember = await _db.HiringTeams.FirstOrDefaultAsync(x => x.HiringTeamId == hiringTeamId);
                teamMember.IsActive = false;
                teamMember.IsDeleted = true;
                teamMember.DeletedBy = claims.employeeId;
                teamMember.DeletedOn = DateTime.Now;

                _db.Entry(teamMember).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Team Member Removed";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use To Remove By Hiring Team Member

        #region This Api Use to Get All the Data View

        /// <summary>
        /// Created By Ankit On 25-08-2022
        /// Api >> Get >> api/job/getviewdata
        /// </summary>
        [HttpGet]
        [Route("getviewdata")]
        public async Task<ResponseBodyModel> GetViewData(int candidateId, int jobPostId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId ==
                        candidateId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (candidate == null)
                {
                    res.Status = false;
                    res.Data = "Candidate Not Found";
                }
                else
                {
                    var hiringStaged = await _db.HiringStages.Include("Job")
                            .Where(x => x.Job.JobPostId == jobPostId && x.IsActive && !x.IsDeleted)
                            .OrderBy(x => x.StageOrder)
                            .ToListAsync();

                    PresonalHelperModel candiateData = new PresonalHelperModel()
                    {
                        CandidateName = candidate.CandidateName,
                        JobId = hiringStaged.First().Job.JobPostId,
                        JobTitle = hiringStaged.First().Job.JobTitle,
                        SourceName = Enum.GetName(typeof(JobHiringSourceConstants), candidate.Source),
                        MobileNumber = candidate.MobileNumber,
                        UploadResume = candidate.UploadResume,
                        Email = candidate.Email,
                        PreferredLocation = candidate.PreferredLocation
                    };
                    var canHiringFlow = await (from s in _db.StageStatuses
                                               join h in _db.HiringStages on s.StageId equals h.StageId
                                               join e in _db.Employee on s.EmployeeId equals e.EmployeeId into q
                                               from result in q.DefaultIfEmpty()
                                               where s.JobId == jobPostId && s.CandidateId == candidate.CandidateId
                                               select new
                                               {
                                                   s.StageOrder,
                                                   s.StageId,
                                                   h.StageName,
                                                   s.PrebordingStageId,
                                                   h.StageType,
                                                   s.Reason,
                                                   s.EmployeeId,
                                                   result.DisplayName,
                                                   s.IsReviewSubmited
                                               })
                                               .OrderBy(x => x.StageOrder)
                                               .Select(x => new
                                               {
                                                   x.StageId,
                                                   StageName = x.StageType == StageFlowType.Preboarding ? x.StageName + " - " + x.PrebordingStageId.ToString() : x.StageName,
                                                   x.Reason,
                                                   EmployeeName = x.DisplayName == null ? "External Interviewer" : x.DisplayName,
                                                   x.IsReviewSubmited,
                                               })
                                               .ToListAsync();
                    res.Message = "Job Info";
                    res.Status = true;
                    res.Data = new
                    {
                        PersonalData = candiateData,
                        CandidatHiringFlow = canHiringFlow,
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class HiredHelperModel
        {
            public int StageId { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Reason { get; set; }
            public string StageName { get; set; }
            public int PreboardingStage { get; set; }
            public string PreBoardingStageName { get; set; }
        }

        #endregion

        #region This Api Use To Dev Uat live 

        #region This Api Use to Get All Data Salery type

        /// <summary>
        /// Created By Ankit On 25-08-2022
        /// Api >> Get >> api/job/getdata
        /// </summary>
        [HttpGet]
        [Route("getdata")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await _db.JobPosts.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                if (jobData.Count > 0)
                {
                    foreach (var demo in jobData)
                    {
                        var check = jobData.Where(x => x.JobPostId == demo.JobPostId).FirstOrDefault();
                        if (check != null)
                        {
                            check.SaleryRange = demo.SaleryStRange.ToString();
                            check.SaleryEndRange = demo.SaleryEdRange.ToString();

                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region This Api Use to Get All Data experience 

        /// <summary>
        /// Created By Ankit On 25-08-2022
        /// Api >> Get >> api/job/getdataexprince
        /// </summary>
        [HttpGet]
        [Route("getdataexprince")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> GetDataExprince()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await _db.JobPosts.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                if (jobData.Count > 0)
                {
                    foreach (var demo in jobData)
                    {
                        var check = jobData.Where(x => x.JobPostId == demo.JobPostId).FirstOrDefault();
                        if (check != null)
                        {
                            check.MinExperience = demo.Experience.ToString();
                            check.MaxExperience = demo.ExperienceMax.ToString();

                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #endregion

        #region 
        /// <summary>
        /// Created By Ankit On 25-08-2022
        /// Api >> Get >> api/job/convertdoctopdf
        /// </summary>
        [HttpGet]
        [Route("convertdoctopdf")]
        [AllowAnonymous]
        public async Task<ResponseBodyModel> ConvertDocToPdf()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            //var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var jobData = await _db.JobPosts.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
                if (jobData.Count > 0)
                {
                    foreach (var demo in jobData)
                    {
                        var check = jobData.Where(x => x.JobPostId == demo.JobPostId).FirstOrDefault();
                        if (check != null)
                        {
                            check.SaleryRange = demo.SaleryStRange.ToString();
                            check.SaleryEndRange = demo.SaleryEdRange.ToString();

                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        public class GetJob
        {
            public int JobPostId { get; set; }
            public string JobTitle { get; set; }
            public string Location { get; set; }
            public bool IsPriority { get; set; }
            public int TotalCandidates { get; set; }
            public int Offer { get; set; }
            public int Openings { get; set; }
            public DateTimeOffset? DueDate { get; set; }
            public int LocationId { get; set; }
            public string JobDescription { get; set; }
            public int DepartmentId { get; set; }
            public string Department { get; set; }
            public DateTimeOffset? TargetHireDate { get; set; }
            public decimal SaleryStRange { get; set; }
            public decimal SaleryEdRange { get; set; }
            public string JobType { get; set; }
            public int HiringFlow { get; set; }
            public string MinExperience { get; set; }
            public string MaxExperience { get; set; }
            public bool IsExtended { get; set; }
            public int ExtendedDays { get; set; }
            public bool PublishToCareers { get; set; }
            public bool PublishToPortal { get; set; }
            public string OrgName { get; set; }
            public int Newcandidate { get; set; }
            public string PriorityName { get; set; }
            public string JobCreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public JobPriorityHelperConstants PriorityId { get; set; }
            public string SaleryRange { get; set; } = string.Empty;
            public string SaleryEndRange { get; set; } = string.Empty;
            public bool ConfidentialSalary { get; set; } = false;
            public bool CompetativeSalary { get; set; } = false;
            public string AllSalaryType { get; set; }

        }
        #region Helper Model Classes

        /// <summary>
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public class GetJobFilterHelper
        {
            public int JobPostId { get; set; }
            public string JobTitle { get; set; }
            public string Location { get; set; }
            public bool IsPriority { get; set; }
            public int TotalCandidates { get; set; }
            public int Offer { get; set; }
            public int Openings { get; set; }
            public DateTimeOffset? DueDate { get; set; }
            public int NewCandidate { get; set; }
            public int ArchivedCount { get; set; }
            public string PriorityName { get; set; }
            public string JobCreatedBy { get; set; }
            public JobPriorityHelperConstants PriorityId { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain on 25-08-2022
        /// </summary>
        public class ViewHelperModel
        {
            public List<PresonalHelperModel> PersonalInfo { get; set; }
            public List<HiredHelperModel> HiredInfo { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain on 25-08-2022
        /// </summary>
        public class PresonalHelperModel
        {
            public string CandidateName { get; set; }
            public string MobileNumber { get; set; }
            public int JobId { get; set; }
            public string JobTitle { get; set; }
            public string UploadResume { get; set; }
            public string SourceName { get; set; }
            public string Email { get; set; }
            public string PreferredLocation { get; set; }
        }

        /// <summary>
        /// Created By Ankit Jain on 25-08-2022
        /// </summary>

        public class HiringTeamRes
        {
            public int EmployeeId { get; set; }
            public HiringTeamConstants Designation { get; set; }
            public string EmployeeName { get; set; }
            public object FlowAssign { get; set; }
            public int StagedId { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public class GetJobDetailsByType4
        {
            public int JobId { get; set; }
            public bool IsPriority { get; set; }
            public string JobTitle { get; set; }
            public string Details { get; set; }
            public string JobDescription { get; set; }
            public JobInfo JobInfo { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 01-02-2022
        /// </summary>
        public class JobInfo
        {
            public string Location { get; set; }
            public string Department { get; set; }
            public string JobType { get; set; }
            public int Openings { get; set; }
            public DateTimeOffset? TargetHireDate { get; set; }
            public string Budget { get; set; }
            public string MinExperience { get; set; }
            public string JobTitle { get; set; }
            public string OrgName { get; set; }
            public string JobExperience { get; set; }
            public bool ConfidentialSalary { get; set; } = false;
            public bool CompetativeSalary { get; set; } = false;
        }

        /// <summary>
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public class StageCount
        {
            public int Sourced { get; set; }
            public int Screening { get; set; }
            public int Interview { get; set; }
            public int Preboarding { get; set; }
            public int Hired { get; set; }
            public int Archived { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        public class NewStageCounts
        {
            public Guid StageId { get; set; }
            public int StageOrder { get; set; }
            public string StageTitle { get; set; }
            public int Count { get; set; }
            public bool IsInterview { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public class CandidateListOnStage
        {
            public int JobId { get; set; }
            public Guid StageId { get; set; }
            public int StageOrder { get; set; }
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string Source { get; set; }
            public string AppliedDate { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public bool IsPreboarding { get; set; }
            public string StageName { get; set; }
            public DateTimeOffset? InterviewDate { get; set; }
            public bool IsInterview { get; set; }
            public bool IsPrebordingStarted { get; set; }
            public bool NewCandidate { get; set; }

        }

        /// <summary>
        /// Created By Harshit Mitra on 02-02-2022
        /// </summary>
        public class JobDetailByTypeForStageFlow
        {
            public int JobId { get; set; }
            public bool IsPriority { get; set; }
            public string JobTitle { get; set; }
            public string Details { get; set; }
            public string StageTitle { get; set; }
            public JobCategory JobCategory { get; set; }
            public List<NewStageCounts> StageCount { get; set; }
            public List<CandidateListOnStageData> CandidateList { get; set; }

        }

        /// <summary>
        /// Created By Ankit Jain on 12-16-2022
        /// </summary>
        public class CandidateListOnStageData
        {

            public int TotalData { get; set; }
            public int Counts { get; set; }
            public object List { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        public class AddGetHiringTeamEmployModel
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        public class AddGetHiringTeamModel
        {
            public int JobId { get; set; }
            public List<AddGetHiringTeamEmployModel> RecruitersIds { get; set; }
            public List<AddGetHiringTeamEmployModel> HiringManagersIds { get; set; }
            public List<AddGetHiringTeamEmployModel> InterviewPanelIds { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class TeamArray
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int Designation { get; set; }
            public List<HiringFlowAssignees> FlowAssign { get; set; }
        }



        public class HiringFlowAssignees
        {
            public int StageId { get; set; }
        }

        public class TeamArray2
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int Designation { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class AddEditHiringTeamNewModel
        {
            public int JobId { get; set; }
            public List<TeamArray> TeamArray { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on
        /// </summary>
        public class GetJobDetailsByType2
        {
            public int JobId { get; set; }
            public bool IsPriority { get; set; }
            public string JobTitle { get; set; }
            public string Details { get; set; }
            public int Offer { get; set; }
            public int Openings { get; set; }
            public int TotalCandidate { get; set; }
            public HiringFunnel HiringFunnel { get; set; }
            public JobInfo JobInfo { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on
        /// </summary>
        public class HiringFunnel
        {
            public string Stat { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on
        /// </summary>
        public class SourcesPie
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 03-02-2022
        /// </summary>
        public class GetJobByIdClass
        {
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
            public int JobType { get; set; }
            public int HiringFlow { get; set; }
            public string MinExperience { get; set; }
            public string MaxExperience { get; set; }
            public bool IsExtended { get; set; }
            public int ExtendedDays { get; set; }
            public bool PublishToCareers { get; set; }
            public bool PublishToPortal { get; set; }
            public bool IsPublished { get; set; }
            public string OrgName { get; set; }
            public int TotalCandidate { get; set; }
            public JobPriorityHelperConstants PriorityId { get; set; }
            public string SaleryRange { get; set; } = string.Empty;
            public string SaleryEndRange { get; set; } = string.Empty;
            public bool ConfidentialSalary { get; set; } = false;
            public bool CompetativeSalary { get; set; } = false;
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        public class JobOpeningList
        {
            public int JobPostId { get; set; }
            public string JobTitle { get; set; }
            public string Details { get; set; }
            public string Location { get; set; }
            public string Department { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 22-02-2022
        /// </summary>
        public class GetJobDetailsOnCareer
        {
            public int JobId { get; set; }
            public string JobTitle { get; set; }
            public string Details { get; set; }
            public string JobDescription { get; set; }
        }

        public class SummaryModal
        {
            public int OffersJob { get; set; }
            public int OpeningJob { get; set; }
            public int DueDays { get; set; }

            public int CandidateSource { get; set; }
            public List<SourcesPie> PieChart { get; set; }
            public List<HiringFunnel> HiringFunnel { get; set; }
            public List<GetJobDetailsByType4> JobInfo { get; set; }
            public List<RequirterCount> RequirterCount { get; set; }
        }

        /// <summary>
        /// Created By ankit on 25-05-2022
        /// </summary>
        public class UploadImageResponseJob
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        #endregion Helper Model Classes

        public class RequirterCount
        {
            public List<WeekDate> Week { get; set; }
            public string RequirterName { get; set; }
            public int value { get; set; }
        }

        public class WeekDate
        {
            public DateTime StartOfWeek { get; set; }
            public DateTime EndOfWeek { get; set; }
        }

        #region Fuction  logic for how many weeks in current month

        /// <summary>
        /// logic for get week start date and end date
        /// </summary>
        /// <returns></returns>
        public List<WeekDate> weekdate()
        {
            var ci = new CultureInfo("en-GB");
            List<WeekDate> firstDayOfWeekDates = new List<WeekDate>();
            var todayDate = DateTime.Today;
            var firstDate = new DateTime(todayDate.Year, todayDate.Month, 1);
            var lastDateOfYear = firstDate.AddMonths(1).AddDays(-1);
            var dayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;
            while (firstDate.DayOfWeek != dayOfWeek)
            {
                firstDate = firstDate.AddDays(1);
            }
            int daysInMonth = DateTime.DaysInMonth(todayDate.Year, todayDate.Month);
            DateTime firstOfMonth = new DateTime(todayDate.Year, todayDate.Month, 1);
            //days of week starts by default as Sunday = 0
            int sss = (int)firstDate.DayOfWeek;
            var numberOfWeeksInYear = (int)Math.Ceiling((sss + daysInMonth) / 7.0);
            firstDayOfWeekDates.Add(new WeekDate { StartOfWeek = firstDate, EndOfWeek = firstDate.AddDays(6) });

            var currentDate = firstDate;

            for (int i = 0; i < numberOfWeeksInYear; i++)
            {
                var weekLater = currentDate.AddDays(7);

                if (weekLater.Year == todayDate.Year)
                {
                    currentDate = currentDate.AddDays(7);
                    firstDayOfWeekDates.Add(new WeekDate { StartOfWeek = currentDate, EndOfWeek = currentDate.AddDays(6) });
                }
            }
            return firstDayOfWeekDates;
        }

        #endregion Fuction  logic for how many weeks in current month
    }
}