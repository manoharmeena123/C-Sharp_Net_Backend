using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 07-02-2022
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/hiring")]
    public class HiringFlowMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Add Default Hiring Flow and Stages In It

        /// <summary>
        /// Created By Harshit Mitra on 07-02-2022
        /// API >> Post >> api/hiring/addhiringflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addhiringflow")]
        public async Task<ResponseBodyModel> AddDefaultFlow(AddEditHiringFlow model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AddEditHiringFlow flowobj = new AddEditHiringFlow();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                HiringFlowMaster obj = new HiringFlowMaster()
                {
                    HiringFlowTitle = model.HiringFlowTitle,
                    IsActive = true,
                    IsDeleted = false,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId,
                    CreatedOn = DateTime.Now,
                    CreatedBy = claims.employeeId,
                };
                _db.HiringFlow.Add(obj);
                await _db.SaveChangesAsync();

                List<StageMaster> stageobj = new List<StageMaster>();
                List<int> orderobj = new List<int>();

                /// Adding Stage For Sourced
                foreach (var loop in model.Sourced)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Sourced,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                /// Adding Stage For Screening
                foreach (var loop in model.Screening)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Screening,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                /// Adding Stage For Interview
                foreach (var loop in model.Interview)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Interview,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                /// Adding Stage For Preboarding
                foreach (var loop in model.Preboarding)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Preboarding,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                /// Adding Stage For Hired
                foreach (var loop in model.Hired)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Hired,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                /// Adding Stage For Archive
                foreach (var loop in model.Archive)
                {
                    StageMaster stage = new StageMaster()
                    {
                        HiringFlowId = obj.HiringFlowId,
                        StageName = loop.StageName,
                        StageType = EnumClass.StageFlowType.Archived,
                        SechduleRequired = loop.SechduleRequired,
                        SechduleDateTime = loop.SechduleDateTime,
                        CompanyId = obj.CompanyId,
                        OrgId = obj.OrgId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Stages.Add(stage);
                    await _db.SaveChangesAsync();
                    stageobj.Add(stage);
                    orderobj.Add(stage.StageId);
                }
                obj.OrderStagesId = string.Join(",", orderobj);
                obj.UpdatedOn = DateTime.Now;
                _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();

                flowobj.HiringFlowId = obj.HiringFlowId;
                flowobj.HiringFlowTitle = obj.HiringFlowTitle;
                flowobj.OrderStagesId = obj.OrderStagesId;
                flowobj.Sourced = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Sourced).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();
                flowobj.Screening = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Screening).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();
                flowobj.Interview = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Interview).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();
                flowobj.Preboarding = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Preboarding).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();
                flowobj.Hired = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Hired).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();
                flowobj.Archive = stageobj.Where(x => x.StageType == EnumClass.StageFlowType.Archived).Select(x => new StageClass
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    SechduleRequired = x.SechduleRequired,
                    SechduleDateTime = x.SechduleDateTime,
                }).ToList();

                res.Message = "Flow Added";
                res.Status = true;
                res.Data = flowobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Default Hiring Flow and Stages In It

        #region Api To Get Hiring Flow By Id

        /// <summary>
        /// API >> Put >> api/hiring/gethiringflow
        /// Created By Harshit Mitra on 07-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gethiringflow")]
        public async Task<ResponseBodyModel> GetHiringFlow(int hiringFlowId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            AddEditHiringFlow obj = new AddEditHiringFlow();
            List<StageMaster> ListStage = new List<StageMaster>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var hiringFlow = await _db.HiringFlow.FirstOrDefaultAsync(x => x.HiringFlowId == hiringFlowId && x.CompanyId == claims.companyId);
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
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Hiring Flow By Id

        #region Api To Get List Of Hiring Flow

        /// <summary>
        /// API >> Get >> api/hiring/activeflowlist
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("activeflowlist")]
        public async Task<ResponseBodyModel> GetActiveHiingFlowList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var flow = await _db.HiringFlow.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).
                    Select(x => new
                    {
                        Id = x.HiringFlowId,
                        Title = x.HiringFlowTitle,
                    }).ToListAsync();
                if (flow.Count > 0)
                {
                    res.Message = "Flow List";
                    res.Status = true;
                    res.Data = flow;
                }
                else
                {
                    res.Message = "Flow List Is Empty";
                    res.Status = false;
                    res.Data = flow;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get List Of Hiring Flow

        #region Api To Get Hiring Flow List By Stages Structure

        /// <summary>
        /// API >> Get >> api/hiring/flowliststructure
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("flowliststructure")]
        public async Task<ResponseBodyModel> GetFlowListByStagesStructure()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var hiringFlow = await (from f in _db.HiringFlow
                                        where f.IsActive == true && f.IsDeleted == false && f.CompanyId == claims.companyId
                                        select new HiringFlowStructure()
                                        {
                                            HiringFlowId = f.HiringFlowId,
                                            HiringFlowTitle = f.HiringFlowTitle,
                                            OrderStagesId = f.OrderStagesId,
                                            Stages = (from s in _db.Stages
                                                      where s.IsActive == true && s.IsDeleted == false && s.HiringFlowId == f.HiringFlowId
                                                      select new StageClass()
                                                      {
                                                          StageId = s.StageId,
                                                          StageName = s.StageName,
                                                          SechduleRequired = s.SechduleRequired,
                                                          SechduleDateTime = s.SechduleDateTime,
                                                      }).ToList(),
                                        }).ToListAsync();
                if (hiringFlow != null)
                {
                    res.Message = "Hiring Flow List";
                    res.Status = true;
                    res.Data = hiringFlow;
                }
                else
                {
                    res.Message = "Hiring Flow List Is Empty";
                    res.Status = false;
                    res.Data = hiringFlow;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Hiring Flow List By Stages Structure

        #region Api To Search Employee By Name Or Email

        /// <summary>
        /// API >> Get >> api/hiring/searchempbynameoremail
        /// Created By Harshit Mitra on 07-02-2022
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("searchempbynameoremail")]
        public async Task<ResponseBodyModel> GetEmployeesearch(string searchString)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmployeeSearchModel> employee = new List<EmployeeSearchModel>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (searchString != null)
                {
                    employee = await _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId /*&& x.OrgId == claims.orgId*/
                    && (x.FirstName.Contains(searchString) || x.LastName.Contains(searchString) || x.PersonalEmail.Contains(searchString))
                    ).Select(x => new EmployeeSearchModel()
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.FirstName + " " + x.LastName,
                        Email = x.PersonalEmail,
                    }).ToListAsync();
                }
                else
                {
                    employee = await _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId /*&& x.OrgId == claims.orgId*/
                    ).Select(x => new EmployeeSearchModel()
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.FirstName + " " + x.LastName,
                        Email = x.PersonalEmail,
                    }).ToListAsync();
                }
                if (employee.Count > 0)
                {
                    res.Message = "Employee Search";
                    res.Status = true;
                    res.Data = employee;
                }
                else
                {
                    res.Message = "Employee List Is Empty";
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

        #endregion Api To Search Employee By Name Or Email

        #region Helper Model Classes

        /// <summary>
        /// Created By Harshit Mitra on 07-02-2022
        /// </summary>
        public class StageClass
        {
            public int StageId { get; set; }
            public string StageName { get; set; }
            public bool SechduleRequired { get; set; }
            public DateTime? SechduleDateTime { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 07-02-2022
        /// </summary>
        public class AddEditHiringFlow
        {
            public int HiringFlowId { get; set; }
            public string HiringFlowTitle { get; set; }
            public string OrderStagesId { get; set; }
            public List<StageClass> Sourced { get; set; }
            public List<StageClass> Screening { get; set; }
            public List<StageClass> Interview { get; set; }
            public List<StageClass> Preboarding { get; set; }
            public List<StageClass> Hired { get; set; }
            public List<StageClass> Archive { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 07-02-2022
        /// </summary>
        public class EmployeeSearchModel
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Email { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        public class HiringFlowStructure
        {
            public int HiringFlowId { get; set; }
            public string HiringFlowTitle { get; set; }
            public string OrderStagesId { get; set; }
            public List<StageClass> Stages { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 08-02-2022
        /// </summary>
        public class StageListByJobId
        {
            public int StageId { get; set; }
            public string StageName { get; set; }
        }

        #endregion Helper Model Classes

        // ----------------- New Stage Flow ----------------//

        #region API To Get Job Hiring Flow
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> Get >> api/hiring/getjobhiringflow
        /// </summary>
        /// <param name="jobPostId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getjobhiringflow")]
        public async Task<ResponseBodyModel> GetJobHiringFlow(int jobPostId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateListOnStage = await _db.Candidates.Where(x => x.JobId == jobPostId).ToListAsync();
                var hiringStage = await _db.HiringStages.Include("Job")
                        .Where(x => x.Job.JobPostId == jobPostId && x.IsActive && !x.IsDeleted)
                        .OrderBy(x => x.StageOrder).ToListAsync();

                res.Message = "Job Hiring Flow";
                res.Status = true;
                res.Data = JobHelper.GetListOfStage(hiringStage, candidateListOnStage); ;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region API To Add New Stage In Between A Stage
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> Post >> api/hiring/addnewstageinbetween
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addnewstageinbetween")]
        public async Task<ResponseBodyModel> AddNewStageInBetween(AddingNewStageRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateListOnStage = await _db.Candidates.Where(x => x.JobId == model.JobPostId).ToListAsync();
                var allStaged = await _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == model.JobPostId).OrderBy(x => x.StageOrder == 0).ThenBy(x => x.StageOrder).ToListAsync();
                int countStage = (allStaged.Count(x => x.StageType == model.StageType) + 1);
                int position = (allStaged.FindLastIndex(x => x.StageType == model.StageType && x.IsActive && !x.IsDeleted) + 1);
                var lastnName = allStaged.Where(x => x.StageType == model.StageType).LastOrDefault();
                HiringStage newObj = new HiringStage
                {
                    StageId = Guid.NewGuid(),
                    Job = allStaged.First().Job,
                    StageName = (model.StageType.ToString() + " - " + (allStaged.Count(x => x.StageType == model.StageType) + 1)),
                    StageType = model.StageType,
                    StageOrder = 0,
                    SechduleRequired = (model.StageType == StageFlowType.Interview),
                    CreatedBy = 0,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId,
                    IsDefault = false,
                };
                _db.HiringStages.Add(newObj);
                await _db.SaveChangesAsync();
                allStaged.RemoveAll(x => x.IsDeleted && !x.IsActive);
                var newStages = JobHelper.AddingStageInPosition(allStaged, position, newObj);
                res.Message = "Stage Updated";
                res.Status = true;
                res.Data = JobHelper.GetListOfStage(newStages, candidateListOnStage);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class AddingNewStageRequest : UpdateHiringFlowRequest
        {
            public int JobPostId { get; set; }
            public Guid StageId { get; set; }
        }
        public class UpdateHiringFlowRequest
        {
            public StageFlowType StageType { get; set; }
        }
        #endregion

        #region API To Delete hiring Stage
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> Post >> api/hiring/deletestagefromflow
        /// </summary>
        /// <param name="stageId"></param>
        /// <param name="jobPostId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletestagefromflow")]
        public async Task<ResponseBodyModel> DeleteStageFromFlow(Guid stageId, int jobPostId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateListOnStage = await _db.Candidates.Where(x => x.JobId == jobPostId).ToListAsync();
                var allStaged = await _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == jobPostId && x.IsActive && !x.IsDeleted).OrderBy(x => x.StageOrder).ToListAsync();
                int position = allStaged.FindIndex(x => x.StageId == stageId);
                var newStages = JobHelper.RemoveStageFromPosition(allStaged, position, claims.employeeId);
                newStages.RemoveAt(0);

                res.Message = "Stage Deleted";
                res.Status = true;
                res.Data = JobHelper.GetListOfStage(newStages, candidateListOnStage);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region API To Edit Stage Name
        /// <summary>
        /// Created By Harshit Mitra On 30-09-2022
        /// API >> Post >> api/hiring/editstagename
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editstagename")]
        public async Task<ResponseBodyModel> EditStageName(EditStageNameRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stage = await _db.HiringStages.FirstOrDefaultAsync(x => x.StageId == model.StageId);
                if (stage != null)
                {
                    stage.StageName = model.StageName;
                    stage.UpdatedBy = claims.employeeId;
                    stage.UpdatedOn = DateTime.Now;
                    _db.Entry(stage).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Stage Name Updated";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Stage Not Found";
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
        public class EditStageNameRequest
        {
            public Guid StageId { get; set; }
            public string StageName { get; set; }
        }
        #endregion

        #region Api To Get Stage List on Behalf Of Job Id

        /// <summary>
        /// API >> Get >> api/hiring/stagelistbyjobid
        /// Created By Harshit Mitra on 08-02-2022
        /// Updated By Harshit Mitra on 01-10-2022
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("stagelistbyjobid")]
        public async Task<ResponseBodyModel> GetStageListByJobId(int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stages = await _db.HiringStages.Include("Job")
                        .Where(x => x.Job.JobPostId == jobId && x.IsActive && !x.IsDeleted)
                        .OrderBy(x => x.StageOrder)
                        .Select(x => new
                        {
                            x.StageId,
                            x.StageName,
                            x.StageOrder,
                            InterviewRequired = x.SechduleRequired,
                        })
                        .ToListAsync();
                res.Message = "Job Stage List";
                res.Status = true;
                res.Data = stages;

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Stage List on Behalf Of Job Id
    }
}