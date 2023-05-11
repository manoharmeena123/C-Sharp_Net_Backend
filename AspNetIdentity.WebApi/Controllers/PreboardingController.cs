using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;
using static AspNetIdentity.WebApi.Controllers.JobController;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Helper.ClientHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;
using Document = iTextSharp.text.Document;
using TextInfo = System.Globalization.TextInfo;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 08-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/preboard")]
    public class PreboardingController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get Preaboarding Data By Type

        /// <summary>
        /// API >> Get >> api/preboard/preboardingbytype
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("preboardingbytype")]
        public async Task<ResponseBodyModel> PreboardingByType(PreboardingStages type, int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            PreboardingModelClass obj = new PreboardingModelClass();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var candidateList = await _db.Candidates.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                                && (x.StageType == StageFlowType.Preboarding || x.StageType == StageFlowType.Hired)).ToListAsync();
                PreBoardingCount Count = new PreBoardingCount()
                {
                    Start = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Start).ToList().Count,
                    CollectInfo = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Collect_Info).ToList().Count,
                    VerifyInfo = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Verfiy_Info).ToList().Count,
                    RealeaseOffer = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Release_Offer).ToList().Count,
                    OfferAcceptance = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Offer_Acceptance).ToList().Count,
                    Hired = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Hired).ToList().Count,
                    Joined = candidateList.Where(x => x.PrebordingStages == PreboardingStages.Joined).ToList().Count,
                };
                var candidateOnPreboarding = candidateList.Where(x => x.PrebordingStages == type).
                    Select(x => new CandidateOnPreboarding()
                    {
                        CandidateId = x.CandidateId,
                        CandidateName = x.CandidateName,
                        MobileNumber = x.MobileNumber,
                        Email = x.Email,
                        jobRoleId = x.JobId,
                        JobRole = _db.JobPosts.Where(j => j.JobPostId == x.JobId).Select(j => j.JobTitle).FirstOrDefault(),
                        PendingSince = x.PendingSince,
                        JoinedDate = x.JoinedDate,
                        IsCredentialProvided = x.IsCredentialProvided,
                        CompanyId = x.CompanyId,
                        OrgId = x.OrgId
                    }).ToList();

                obj.StageName = Enum.GetName(typeof(PreboardingStages), type).Replace('_', ' ');
                obj.Count = Count;
                obj.CandidateList = candidateOnPreboarding;

                res.Message = "Candidate list Found";
                res.Status = true;
                if (page.HasValue && count.HasValue && search != null)
                {
                    var text = textInfo.ToUpper(search);
                    res.Data = new PaginationDataHiring
                    {
                        TotalData = candidateOnPreboarding.Count,
                        Counts = (int)count,
                        List = candidateOnPreboarding.Where(x => x.CandidateName.ToUpper().StartsWith(text) || (x.JobRole.ToUpper().StartsWith(text) ||
                                x.MobileNumber.ToUpper().StartsWith(text))).Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        StageName = Enum.GetName(typeof(PreboardingStages), type).Replace('_', ' '),
                    };
                }
                else if (page.HasValue && count.HasValue)
                {
                    res.Data = new PaginationDataHiring
                    {
                        TotalData = candidateOnPreboarding.Count,
                        Counts = (int)count,
                        List = candidateOnPreboarding.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        StageName = Enum.GetName(typeof(PreboardingStages), type).Replace('_', ' '),
                    };
                }
                else
                {
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

        #endregion Api To Get Preaboarding Data By Type

        #region This Api Use For Get All The Data In Dashboard

        /// <summary>
        /// Created By Mayank Prajapati on 09-02-2022
        /// Updated By Ankit on 26-08-2022
        /// API >> Get >> api/preboard/getalldatainhiringmodule
        /// </summary>
        [HttpGet]
        [Route("getalldatainhiringmodule")]
        public async Task<ResponseBodyModel> GetAllApplication()
        {
            PreboardingModelClass response = new PreboardingModelClass();
            ResponseBodyModel res = new ResponseBodyModel();
            HelperForCandidate CandidateRelated = new HelperForCandidate();
            GetJobModuleHelperClass CanidateList = new GetJobModuleHelperClass();
            var currentYear = DateTime.Now.Year;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var allCandidateApp = await _db.Candidates.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (allCandidateApp.Count > 0)
                {
                    #region Thi API is Total Candidate List.

                    CandidateRelated.TotalApplication = allCandidateApp.Count(x => x.IsActive && !x.IsDeleted);
                    CandidateRelated.HiredCandidate = allCandidateApp.Count(x => x.IsActive && !x.IsDeleted &&
                                x.StageType == StageFlowType.Hired);
                    CandidateRelated.RejectedCandidate = allCandidateApp.Count(x => x.IsActive && !x.IsDeleted &&
                                x.StageType == StageFlowType.Archived);

                    res.Message = "Candidate Count List Found";
                    res.Status = true;
                    res.Data = CandidateRelated;
                    #endregion Thi API is Total Candidate List.

                    #region Department Hiring Pie Chart.

                    List<HelpForCiechart> PieChart = new List<HelpForCiechart>();
                    foreach (var item in _db.JobPosts.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId)
                    .Select(x => x.JobTitle).Distinct().ToList())
                    {
                        HelpForCiechart obj = new HelpForCiechart
                        {
                            Name = item,
                            Value = _db.JobPosts.Where(x => x.JobTitle == item).Select(x => x.Openings).Sum()
                        };
                        PieChart.Add(obj);
                        CandidateRelated.DepartmentPieChart = PieChart;
                    }

                    #endregion Department Hiring Pie Chart.

                    #region Piechart in sorce

                    var jobhiringsource = Enum.GetValues(typeof(JobHiringSourceConstants))
                                          .Cast<JobHiringSourceConstants>()
                                          .Select(x => new HelperModelForEnum
                                          {
                                              TypeId = (int)x,
                                              TypeName = Enum.GetName(typeof(JobHiringSourceConstants), x).Replace("_", " ")
                                          }).ToList();
                    List<SourceListOrderBy> PieChartRequirement = new List<SourceListOrderBy>();
                    var sourcedata = _db.Sources.Where(x => x.IsActive && !x.IsDelete).ToList();
                    foreach (var item in jobhiringsource)
                    {
                        SourceListOrderBy obj = new SourceListOrderBy
                        {
                            Name = item.TypeName,
                            Value = allCandidateApp.Count(x => x.Source == item.TypeId),
                        };
                        PieChartRequirement.Add(obj);
                        CandidateRelated.PieChartSource = PieChartRequirement;
                    }

                    #endregion Piechart in sorce

                    #region This Api use StageFunel

                    var stagesdata = Enum.GetValues(typeof(StageFlowType))
                                         .Cast<StageFlowType>()
                                         .Where(x => x != StageFlowType.Archived
                                         && x != StageFlowType.Sourced
                                         && x != StageFlowType.Hired)
                                         .Select(x => new
                                         {
                                             TypeId = x,
                                             TypeName = Enum.GetName(typeof(StageFlowType), x).Replace("_", " ")
                                         }).ToList();
                    var funnelCandidates = (from c in allCandidateApp
                                                //join s in _db.Stages on c.StageId equals s.StageId
                                            where !c.IsDeleted && c.CompanyId == claims.companyId && c.IsActive
                                            select new GetFunnel
                                            {
                                                CandidateId = c.CandidateId,
                                                StageId = c.StageId,
                                                StageType = c.StageType,
                                                StageName = _db.HiringStages.Where(x => x.StageId == c.StageId).Select(x => x.StageName).FirstOrDefault(),
                                                PreboardingArchiveStage = c.PreboardingArchiveStage,
                                                PrebordingStages = c.PrebordingStages,
                                                HiringArchiveStage = c.HiringArchiveStage
                                            }).ToList();
                    List<StageFunnel> listFunnel = new List<StageFunnel>();
                    foreach (var item in stagesdata)
                    {
                        StageFunnel objInnerFunnel = new StageFunnel
                        {
                            StageName = item.TypeName,
                            Count = funnelCandidates.Count(x => x.StageType == item.TypeId),
                        };
                        listFunnel.Add(objInnerFunnel);
                    }
                    StageFunnel objFunnel = new StageFunnel
                    {
                        StageName = Enum.GetName(typeof(StageFlowType), StageFlowType.Hired),
                        Count = funnelCandidates.Count(x => x.PrebordingStages == PreboardingStages.Hired),
                    };
                    listFunnel.Add(objFunnel);
                    CandidateRelated.FunelGraph = listFunnel;

                    #endregion This Api use StageFunel

                    #region This Api Use For Get Deparment

                    List<HelpForDepartment> Barchart = new List<HelpForDepartment>();

                    var departmentdata = _db.JobPosts.Where(x => !x.IsDeleted && x.IsActive &&
                      x.CompanyId == claims.companyId && x.Department != "Administrator").Distinct().ToList();

                    if (departmentdata.Count > 0)
                    {
                        foreach (var item in departmentdata)
                        {
                            HelpForDepartment Depobj = new HelpForDepartment
                            {
                                Name = item.Department,
                                Value = _db.Candidates.Where(x => x.JobId == item.JobPostId && x.IsActive && !x.IsDeleted).Select(x => x.CandidateId).ToList().Count,
                            };
                            Barchart.Add(Depobj);
                            CandidateRelated.BarChartDepartment = Barchart;
                        }
                    }

                    #endregion This Api Use For Get Deparment

                    #region This Api Ratio In Total Hired And Total Applicaton Candidate

                    var TotalApplication = allCandidateApp.Count(x => x.IsActive && !x.IsDeleted);
                    var TotalHired = allCandidateApp.Count(x => x.StageType == StageFlowType.Hired);
                    var totalOfferAccept = allCandidateApp.Count(x => x.PrebordingStages == PreboardingStages.Offer_Acceptance);
                    List<HelpForRatiocandidate> RatioinHiredcandidate = new List<HelpForRatiocandidate>();
                    if (TotalApplication != 0)
                    {
                        if (TotalHired != 0)
                        {
                            HelpForRatiocandidate RatioObj = new HelpForRatiocandidate
                            {
                                TotalHired = TotalHired,
                                TotalApplication = TotalApplication,
                                TotalOffer = totalOfferAccept,
                            };
                            RatioinHiredcandidate.Add(RatioObj);
                            var offerAccept = GetRatioData(RatioObj.TotalHired, RatioObj.TotalOffer);
                            var Ratiodata = GetRatio(RatioObj.TotalApplication, RatioObj.TotalHired);
                            var percentage = GetPercent(RatioObj.TotalApplication, RatioObj.TotalHired);
                            List<HelpForRatioData> RatioinHired = new List<HelpForRatioData>();
                            if (Ratiodata != null)
                            {
                                HelpForRatioData Ratio = new HelpForRatioData
                                {
                                    TotalHiredCandidate = Ratiodata,
                                    TotalHiredpercentage = percentage,
                                    TotalOfferRecived = offerAccept,
                                };
                                RatioinHired.Add(Ratio);
                                CandidateRelated.RatioHiredCandidate = RatioinHired;
                            }
                        }
                    }

                    #endregion This Api Ratio In Total Hired And Total Applicaton Candidate
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class GetFunnel
        {
            public int CandidateId { get; set; }
            public StageFlowType StageType { get; set; }
            public string StageName { get; set; }
            public Guid? PreboardingArchiveStage { get; set; }
            public PreboardingStages PrebordingStages { get; set; }
            public Guid? StageId { get; set; }
            public Guid? HiringArchiveStage { get; set; }
        }

        public string GetRatio(int TotalApplication, int TotalHired)
        {
            string ratio;
            try
            {
                for (var num = TotalApplication; num > 1; num--)
                {
                    if ((TotalHired % num) == 0 && (TotalApplication % num) == 0)
                    {
                        TotalHired /= num;

                        TotalApplication /= num;
                    }
                }
                ratio = TotalHired + ":" + TotalApplication;
            }
            catch (Exception)
            {
                throw;
            }
            return ratio;
        }

        public string GetRatioData(int TotalHired, int TotalOffer)
        {
            string ratio;
            try
            {
                for (var num = TotalHired; num > 1; num--)
                {
                    if ((TotalOffer % num) == 0 && (TotalHired % num) == 0)
                    {
                        TotalOffer /= num;

                        TotalHired /= num;
                    }
                }
                ratio = TotalOffer + ":" + TotalHired;
            }
            catch (Exception)
            {
                throw;
            }
            return ratio;
        }
        public int GetPercent(double TotalApplication, double TotalHired)
        {
            double percentage;
            try
            {
                percentage = (TotalHired / TotalApplication) * 100;
            }
            catch (Exception)
            {
                throw;
            }
            return (int)percentage;
        }

        #endregion This Api Use For Get All The Data In Dashboard

        #region Helper model Class

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class GetJobModuleHelperClass
        {
            public List<HelperForCandidate> CandidateList { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class HelperForCandidate
        {
            public int TotalApplication { get; set; }
            public int HiredCandidate { get; set; }
            public int RejectedCandidate { get; set; }

            public List<HelpForCiechart> DepartmentPieChart { get; set; }
            public List<SourceListOrderBy> PieChartSource { get; set; }
            public List<StageFunnel> FunelGraph { get; set; }

            public List<HelpForDepartment> BarChartDepartment { get; set; }

            public List<HelpForRatioData> RatioHiredCandidate { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class HelpForCiechart
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class HelpForDepartment
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class HelpForRatiocandidate
        {
            public int TotalHired { get; set; }
            public int TotalApplication { get; set; }
            public int TotalOffer { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class HelpForRatioData
        {
            public string TotalHiredCandidate { get; set; }
            public double TotalHiredpercentage { get; set; }
            public string TotalOfferRecived { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class SourceListOrderBy
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class StageFunnel
        {
            public int Count { get; set; }
            public string StageName { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 30-08-2022
        /// </summary>
        public class TotalAll
        {
            public int TotalCandidate { get; set; }
            public int TotalHierd { get; set; }
            public int TotalOfferAccepted { get; set; }
        }

        #endregion Helper model Class

        #region This Api Use Send By Offer Letter
        /// <summary>
        /// Created By Ankit 08/08/2022
        /// Api >> Post >> api/preboard/sendpdf
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("sendpdf")]
        public async Task<ResponseBodyModel> SendPdf(AddOffer model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = _db.Candidates.FirstOrDefault(x => x.CandidateId == model.CandidateId
                && x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                if (data != null)
                {
                    StageStatus obj = new StageStatus
                    {
                        CandidateId = model.CandidateId,
                        StageId = (Guid)data.StageId,
                        EmployeeId = claims.employeeId,
                        CompanyId = claims.companyId,
                        Reason = model.Reason,
                        PrebordingStageId = data.PrebordingStages,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        JobId = data.JobId,
                        CreatedBy = claims.employeeId,
                        OrgId = claims.orgId,
                        StageOrder = _db.StageStatuses.Count(x => x.JobId == data.JobId && x.CandidateId == data.CandidateId),
                    };
                    _db.StageStatuses.Add(obj);
                    await _db.SaveChangesAsync();

                    data.OfferLetter = model.OfferLetter;
                    data.JoinedDate = model.JoinedDate;
                    data.UpdatedOn = DateTime.Now;
                    _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    await SendOfferLetter(data.CandidateId, claims);

                    data.PrebordingStages = PreboardingStages.Offer_Acceptance;
                    data.UpdatedOn = DateTime.Now;
                    _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Offer Letter Send";
                    res.Data = data;
                    res.Status = true;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = data;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use Send By Offer Letter

        #region This Api Use To add multiple Reason

        /// <summary>
        /// Created By Ankit 08/08/2022
        /// Api >> Post >> api/preboard/addmultiplereason
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addmultiplereason")]

        public async Task<ResponseBodyModel> AddMultipleReason(AddMultiReason model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = _db.Candidates.FirstOrDefault(x => x.CandidateId == model.CandidateId && x.StageId == model.StageId
                && x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                if (data != null)
                {
                    var stageList = await _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == data.JobId
                   && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                    if (stageList != null)
                    {
                        StageStatus obj = new StageStatus
                        {
                            Reason = model.Reason,
                            StageId = (Guid)data.StageId,
                            JobId = data.JobId,
                            EmployeeId = claims.employeeId,
                            CandidateId = data.CandidateId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.employeeId,
                            OrgId = claims.orgId,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == data.JobId && x.CandidateId == data.CandidateId),
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = "Added Multiple Remark";
                        res.Data = data;
                        res.Status = true;

                    }
                    else
                    {
                        res.Message = "Data Not Found";
                        res.Status = false;
                        res.Data = data;
                    }
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = data;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public class AddMultiReason
        {
            public int CandidateId { get; set; }
            public Guid StageId { get; set; }
            public string Reason { get; set; }
        }

        #endregion This Api Use Send By Offer Letter

        #region This Api Use Hired

        /// <summary>
        /// Created By Ankit Date - 08-08-2022
        /// Api >> Post >> api/preboard/hired
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("hired")]
        public async Task<ResponseBodyModel> Hired(CandidateHired model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = _db.Candidates.Where(x => x.CandidateId == model.CandidateId &&
                        x.JobId == model.JobId && x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (candidate != null)
                {
                    var stageList = await _db.HiringStages.Include("Job").Where(x => x.Job.JobPostId == candidate.JobId
                    && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                    if (stageList != null)
                    {
                        StageStatus obj = new StageStatus
                        {
                            CandidateId = model.CandidateId,
                            StageId = (Guid)candidate.StageId,
                            EmployeeId = claims.employeeId,
                            CompanyId = claims.companyId,
                            Reason = model.Reason,
                            PrebordingStageId = candidate.PrebordingStages,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            JobId = candidate.JobId,
                            CreatedBy = claims.employeeId,
                            OrgId = claims.orgId,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == candidate.JobId && x.CandidateId == candidate.CandidateId),
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();

                        candidate.StageId = stageList.Where(x => x.StageType == StageFlowType.Hired).Select(x => x.StageId).FirstOrDefault();
                        candidate.StageType = StageFlowType.Hired;
                        candidate.PrebordingStages = PreboardingStages.Hired;
                        _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Candidate Hired";
                        res.Data = candidate;
                        res.Status = true;
                    }
                }
                else
                {
                    res.Message = "Candidate is Not Found";
                    res.Data = candidate;
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

        #endregion This Api Use Hired

        #region This Api Use Candidate Joined

        /// <summary>
        /// Created By Ankit Date - 08-08-2022
        /// Api >> Post >> api/preboard/candidatejoined
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("candidatejoined")]
        public async Task<ResponseBodyModel> CandidateJoin(CandidateJoined model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidatedata = _db.Candidates.Where(x => x.CandidateId == model.CandidateId &&
                        x.JobId == model.JobId && x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (candidatedata != null)
                {
                    var stage = await _db.HiringStages.FirstOrDefaultAsync(s => s.StageId == candidatedata.StageId
                    && s.IsActive && !s.IsDeleted && s.CompanyId == claims.companyId);
                    if (stage != null)
                    {
                        StageStatus obj = new StageStatus
                        {
                            CandidateId = model.CandidateId,
                            StageId = stage.StageId,
                            EmployeeId = claims.employeeId,
                            CompanyId = claims.companyId,
                            Reason = model.Reason,
                            PrebordingStageId = candidatedata.PrebordingStages,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            JobId = candidatedata.JobId,
                            CreatedBy = claims.employeeId,
                            OrgId = claims.orgId,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == candidatedata.JobId && x.CandidateId == candidatedata.CandidateId),
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();
                    }

                    candidatedata.PrebordingStages = PreboardingStages.Joined;
                    _db.Entry(candidatedata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Candidate is Joined";
                    res.Data = candidatedata;
                    res.Status = true;
                }
                else
                {
                    res.Message = "Candidate is Not Found";
                    res.Data = candidatedata;
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

        #endregion This Api Use Hired

        #region This Api Use To Update Join Date

        /// <summary>
        /// Created By Ankit Date - 16-09-2022
        /// Api >> Put >> api/preboard/updatejoineddate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatejoineddate")]
        public async Task<ResponseBodyModel> UpdateJoin(UpdateJoindate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Data = await _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.IsActive
                && !x.IsDeleted && x.CompanyId == claims.companyId && x.JobId == model.JobId).FirstOrDefaultAsync();
                if (Data != null)
                {
                    if (model.JoinedDate != null)
                    {
                        Data.JoinedDate = model.JoinedDate;
                        _db.Entry(Data).State = EntityState.Modified;
                        _db.SaveChanges();
                        res.Message = "Candidate Joind Date Updated";
                        res.Data = Data;
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Joining Date Null";
                        res.Data = null;
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Not Updated";
                    res.Data = Data;
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

        #endregion This Api Use  Archived Candidate

        public class UpdateJoindate
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public DateTime? JoinedDate { get; set; }
        }


        #region This Api Use Add Credential Preboarding

        /// <summary>
        /// Created By Ankit Date-08-08-2022
        /// Api >> Post >> api/preboard/addcredential
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcredential")]
        public async Task<ResponseBodyModel> AddCredential(CredentialData model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<CredentialData> Obj = new List<CredentialData>();
            //CredentilaHelpermodel1 response = new CredentilaHelpermodel1();
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
                    var candidate = _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId
                    && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    var Demo1 = _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId
                    && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    if (candidate != null)
                    {
                        foreach (var item in model.Credential)
                        {
                            CredentilData obj = new CredentilData()
                            {
                                CandidateId = candidate.CandidateId,
                                Name = item.Name,
                                Url = item.Url,
                                UserName = item.UserName,
                                Password = item.Password,
                                CreatedOn = DateTime.Now,
                            };
                            //Obj.CredentialData = Credential;
                            _db.CredentilDatas.Add(obj);
                            await _db.SaveChangesAsync();
                            res.Message = "Candidate Credential Added";
                            res.Status = true;
                            res.Data = obj;
                        }
                        SendPdf(candidate);

                        Demo1.IsCredentialProvided = true;
                        _db.Entry(Demo1).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = "Mail Send Successfully";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Candidate Credential ot Added";
                        res.Status = true;
                        res.Data = candidate;
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

        #endregion This Api Use Add Credential Preboarding

        #region This Api Use  Archived Candidate

        /// <summary>
        /// Created By Ankit Date - 08-08-2022
        /// Api >> Post >> api/preboard/archived
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("archived")]
        [HttpPost]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> Archivedred(Candidate model)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Data = await _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (Data != null)
                {
                    Data.PrebordingStages = PreboardingStages.Archived;
                    _db.Entry(Data).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Candidate is Archived";
                    res.Data = Data;
                    res.Status = true;
                }
                else
                {
                    res.Message = "Model Is Null";
                    res.Data = Data;
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

        #endregion This Api Use  Archived Candidate

        #region Api To Get Candidate List In Joined

        /// <summary>
        /// API >> Get >> api/preboard/preboardjoined
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("preboardjoined")]
        public async Task<ResponseBodyModel> HiredCandidateList(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateList = await _db.Candidates.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId
                                    && (x.StageType == StageFlowType.Preboarding || x.StageType == StageFlowType.Hired)).ToListAsync();
                var candidateListOnJoined = candidateList.Where(s => s.PrebordingStages == PreboardingStages.Joined).
                    Select(x => new CandidateOnJoined()
                    {
                        CandidateId = x.CandidateId,
                        CandidateName = x.CandidateName,
                        MobileNumber = x.MobileNumber,
                        Email = x.Email,
                        DateOfBirth = x.DateOfBirth,
                        RelevantExperience = x.RelevantExperience,
                        Experience = x.Experience,
                        Qualifications = x.Qualifications,
                        NoticePeriod = x.NoticePeriod,
                        InterViewType = x.InterViewType,
                        Availabiltys = x.Availabilitys,
                        Gender = x.Gender,
                        JoinedDate = x.JoinedDate,
                        JobRole = _db.JobPosts.Where(j => j.JobPostId == x.JobId).Select(j => j.JobTitle).FirstOrDefault(),
                        JoinedOn = x.CreatedOn,
                        jobRoleId = x.JobId
                    }).OrderBy(x => x.CandidateName).ToList();

                if (candidateListOnJoined.Count != 0)
                {
                    if (page == null || count == null)
                    {
                        res.Message = "Candidate list Found";
                        res.Status = true;
                        res.Data = candidateListOnJoined;
                    }
                    else
                    {
                        res.Message = "Candidate list Found";
                        res.Status = true;
                        if (page.HasValue && count.HasValue)
                        {
                            res.Data = new PaginationData
                            {
                                TotalData = candidateListOnJoined.Count,
                                Counts = (int)count,
                                List = candidateListOnJoined.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else
                        {
                            res.Data = candidateListOnJoined;
                        }
                    }
                }
                else
                {
                    res.Message = "Candidate List Empty";
                    res.Status = false;
                    res.Data = candidateListOnJoined;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Candidate List In Joined

        #region This Api is used to Upload Preboard Document

        /// <summary>
        /// Created By Ankit 16/05/2022
        ///Post >> api/preboard/docverified
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("docverified")]
        public async Task<ResponseBodyModel> DocVerified(Documenthelperclass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var prebordingdoc = await _db.prebordingDocuments.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (prebordingdoc != null)
                {
                    var candidate = await _db.Candidates.Where(x => x.IsActive && !x.IsDeleted &&
                     x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId).FirstOrDefaultAsync();

                    if (candidate != null)
                    {
                        StageStatus obj = new StageStatus
                        {
                            CandidateId = model.CandidateId,
                            StageId = (Guid)candidate.StageId,
                            EmployeeId = claims.employeeId,
                            CompanyId = claims.companyId,
                            JobId = candidate.JobId,
                            CreatedBy = claims.employeeId,
                            OrgId = claims.orgId,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == candidate.JobId && x.CandidateId == candidate.CandidateId),
                            Reason = model.Reason,
                            PrebordingStageId = candidate.PrebordingStages,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();

                        candidate.PrebordingStages = PreboardingStages.Release_Offer;
                        _db.Entry(candidate).State = EntityState.Modified;
                        _db.SaveChanges();
                        res.Status = true;
                        res.Message = "Candidate Verified Successfully";
                        res.Data = candidate;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Candidate Not Found";
                        res.Data = candidate;
                    }
                }
                else
                {
                    res.Status = false;
                    res.Message = "PreDocument Not Found";
                    res.Data = prebordingdoc;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload Preboard Document

        #region This Api is used to Add Document Status

        /// <summary>
        /// Created By Ankit 16/09/2022
        ///Api >>Post >> api/preboard/adddocumnenStatus
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adddocumnenStatus")]
        public async Task<ResponseBodyModel> UploadDoc(Documentdata model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Status = false;
                    res.Message = "model is invalid";
                    res.Data = null;
                }
                else
                {
                    var candidate = _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.IsActive && !x.IsDeleted
                    && x.CompanyId == claims.companyId && x.OrgId == model.OrgId).FirstOrDefault();
                    if (candidate != null)
                    {
                        CandidateDoc obj = new CandidateDoc
                        {
                            CandidateId = model.CandidateId,
                            JobId = model.JobId,
                            PanCard = model.PanCard,
                            AadharCard = model.AadharCard,
                            Passport = model.Passport,
                            BankPassbook = model.BankPassbook,
                            Marksheet10Th = model.Marksheet10Th,
                            Marksheet11Th = model.Marksheet11Th,
                            Marksheet12Th = model.Marksheet12Th,
                            UgLastSemMarksheet = model.UgLastSemMarksheet,
                            PgLastSemMarksheet = model.PgLastSemMarksheet,
                            UgDegree = model.UgDegree,
                            PgDegree = model.PgDegree,
                            Certificate = model.Certificate,
                            ExperienceLetter = model.ExperienceLetter,
                            PaySlips3months = model.PaySlips3months,
                            Resignation = model.Resignation,
                            IsDocumentSubmited = false,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = candidate.CompanyId,
                            OrgId = candidate.OrgId,
                            Url = model.Url

                        };
                        _db.candidateDocs.Add(obj);
                        await _db.SaveChangesAsync();

                        StageStatus obj1 = new StageStatus
                        {
                            CandidateId = model.CandidateId,
                            StageId = (Guid)candidate.StageId,
                            EmployeeId = claims.employeeId,
                            CompanyId = claims.companyId,
                            JobId = candidate.JobId,
                            Reason = model.Reason,
                            PrebordingStageId = candidate.PrebordingStages,
                            StageOrder = _db.StageStatuses.Count(x => x.CandidateId == candidate.CandidateId && x.JobId == candidate.JobId),
                            CreatedOn = DateTime.Now,
                            CreatedBy = claims.employeeId,
                            OrgId = claims.orgId,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        _db.StageStatuses.Add(obj1);
                        await _db.SaveChangesAsync();

                        candidate.IsPreboardingStarted = true;
                        candidate.PrebordingStages = PreboardingStages.Verfiy_Info;
                        _db.Entry(candidate).State = EntityState.Modified;
                        _db.SaveChanges();

                        await SendDoc(obj.CandidateId, obj.Url, claims);
                        res.Status = true;
                        res.Message = "Document Added Successfully";
                        res.Data = obj;
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "candidate not found";
                        res.Data = null;
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

        #endregion This Api is used to Upload Preboard Document

        #region This Api is used to Update Document Status

        /// <summary>
        /// Created By Ankit 16/09/2022
        ///Api>> Put >> api/preboard/updateDocumentStatus
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("updateDocumentStatus")]
        public async Task<ResponseBodyModel> UpdateDoc(Documentdata model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidatedocument = await _db.candidateDocs.Where(x => x.CandidateId == model.CandidateId && x.JobId == model.JobId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).FirstOrDefaultAsync();
                if (candidatedocument != null)
                {
                    candidatedocument.CandidateId = model.CandidateId;
                    candidatedocument.JobId = model.JobId;
                    candidatedocument.PanCard = model.PanCard;
                    candidatedocument.AadharCard = model.AadharCard;
                    candidatedocument.Passport = model.Passport;
                    candidatedocument.BankPassbook = model.BankPassbook;
                    candidatedocument.Marksheet10Th = model.Marksheet10Th;
                    candidatedocument.Marksheet11Th = model.Marksheet11Th;
                    candidatedocument.Marksheet12Th = model.Marksheet12Th;
                    candidatedocument.UgLastSemMarksheet = model.UgLastSemMarksheet;
                    candidatedocument.PgLastSemMarksheet = model.PgLastSemMarksheet;
                    candidatedocument.UgDegree = model.UgDegree;
                    candidatedocument.PgDegree = model.PgDegree;
                    candidatedocument.Certificate = model.Certificate;
                    candidatedocument.ExperienceLetter = model.ExperienceLetter;
                    candidatedocument.PaySlips3months = model.PaySlips3months;
                    candidatedocument.Resignation = model.Resignation;
                    candidatedocument.UpdatedBy = claims.employeeId;
                    candidatedocument.Url = model.Url;
                    candidatedocument.IsDocumentSubmited = false;
                    candidatedocument.UpdatedOn = DateTime.Now;
                    candidatedocument.IsActive = true;
                    candidatedocument.IsDeleted = false;
                    candidatedocument.CompanyId = candidatedocument.CompanyId;
                    candidatedocument.OrgId = candidatedocument.OrgId;

                    _db.Entry(candidatedocument).State = EntityState.Modified;
                    _db.SaveChanges();

                    await UpadtePreDoc(candidatedocument.CandidateId, claims);

                    res.Status = true;
                    res.Message = "Document Update Successfully";
                    res.Data = candidatedocument;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Document Not Update";
                    res.Data = candidatedocument;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload Preboard Document

        #region This Api Use To Update Document Status Send Document Link In Mail
        ///// <summary>
        ///// Create By ankit Date-17-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("sendmaildoc")]
        public async Task UpadtePreDoc(int CandidateId, ClaimsHelperModel claims)
        {
            try
            {
                var candidate = (from c in _db.Candidates
                                 join ca in _db.candidateDocs on c.CandidateId equals ca.CandidateId
                                 join j in _db.JobPosts on c.JobId equals j.JobPostId
                                 where c.CandidateId == CandidateId
                                 select new
                                 {
                                     c.CandidateName,
                                     c.CompanyName,
                                     j.JobTitle,
                                     c.InterViewType,
                                     c.Email,
                                     ca.Url
                                 }).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId
                ).Select(x => x.DisplayName).FirstOrDefault();
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

                string htmlBody = JobHelper.UpdateYourDocument
                         .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                         .Replace("<|JOBTITLE|>", candidate.JobTitle)
                         .Replace("<|UPLOADDOCUMENT|>", candidate.Url)
                         .Replace("<|RECRUITERNAME|>", recruiterName)
                         .Replace("<|COMPANYNAME|>", candidate.CompanyName)
                         .Replace("<|IMAGE_PATH|>", "emossy.png")
                         .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                         .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Document Verification",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { candidate.Email },
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

        #region This Api is used to Get Upload Document

        /// <summary>
        /// Created By Ankit 16/09/2022
        /// Api >> Get >> api/preboard/getDocument
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getDocument")]
        public async Task<ResponseBodyModel> GetDoc(int candidateId, int companyId, int orgId, int jobId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await (from cd in _db.candidateDocs
                                       join bc in _db.Candidates on cd.CandidateId equals bc.CandidateId
                                       where cd.IsActive && !cd.IsDeleted && cd.CandidateId == candidateId
                                       && cd.CompanyId == companyId && cd.OrgId == orgId && cd.JobId == jobId
                                       select new DocumentdataGet
                                       {
                                           CandidateId = cd.CandidateId,
                                           JobId = cd.JobId,
                                           CandidateName = bc.CandidateName,
                                           MobileNumber = bc.MobileNumber,
                                           PanCard = cd.PanCard,
                                           AadharCard = cd.AadharCard,
                                           Passport = cd.Passport,
                                           BankPassbook = cd.Passport,
                                           Marksheet10Th = cd.Marksheet10Th,
                                           Marksheet11Th = cd.Marksheet11Th,
                                           Marksheet12Th = cd.Marksheet12Th,
                                           UgLastSemMarksheet = cd.UgLastSemMarksheet,
                                           PgLastSemMarksheet = cd.PgLastSemMarksheet,
                                           UgDegree = cd.UgDegree,
                                           PgDegree = cd.PgDegree,
                                           Certificate = cd.Certificate,
                                           Resignation = cd.Resignation,
                                           ExperienceLetter = cd.ExperienceLetter,
                                           PaySlips3months = cd.PaySlips3months,
                                           IsDocumentSubmited = cd.IsDocumentSubmited
                                       }).FirstOrDefaultAsync();
                if (candidate != null)
                {
                    res.Status = true;
                    res.Message = "Get all The Document";
                    res.Data = candidate;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Document not Found";
                    res.Data = candidate;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload Preboard Document

        #region This Api is used to Get Prebording Upload Document

        /// <summary>
        /// Created By Ankit 16/09/2022
        /// Api >> Get >> api/preboard/getPrebordinDoc
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getPrebordinDoc")]
        public async Task<ResponseBodyModel> GetPrebordDoc(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await (from cd in _db.prebordingDocuments
                                       join bc in _db.Candidates on cd.JobId equals bc.JobId
                                       where cd.IsActive && !cd.IsDeleted && cd.CandidateId == candidateId
                                       select new GetByDocumentdata
                                       {
                                           CandidateId = cd.CandidateId,
                                           JobId = cd.JobId,
                                           CandidateName = bc.CandidateName,
                                           MobileNumber = bc.MobileNumber,
                                           PanCard = cd.PanCard,
                                           AadharCard = cd.AadharCard,
                                           Passport = cd.Passport,
                                           BankPassbook = cd.Passport,
                                           Marksheet10Th = cd.Marksheet10Th,
                                           Marksheet11Th = cd.Marksheet11Th,
                                           Marksheet12Th = cd.Marksheet12Th,
                                           UgLastSemMarksheet = cd.UgLastSemMarksheet,
                                           PgLastSemMarksheet = cd.PgLastSemMarksheet,
                                           UgDegree = cd.UgDegree,
                                           PgDegree = cd.PgDegree,
                                           Certificate = cd.Certificate,
                                           Resignation = cd.Resignation,
                                           ExperienceLetter = cd.ExperienceLetter,
                                           PaySlips3months = cd.PaySlips3months,
                                       }).FirstOrDefaultAsync();
                if (candidate != null)
                {
                    res.Status = true;
                    res.Message = "Get all The Document";
                    res.Data = candidate;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Document not Added.";
                    res.Data = candidate;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload Preboard Document

        #region This Api Use To Get Document By Candidate Id

        /// <summary>
        /// Created By Ankit 16/09/2022
        /// Api >> Get >> api/preboard/getdocumentbyId
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdocumentbyId")]
        public async Task<ResponseBodyModel> GetDocumentById(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidatedocument = await _db.candidateDocs.FirstOrDefaultAsync(x => x.CandidateId == candidateId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);

                if (candidatedocument != null)
                {
                    res.Status = true;
                    res.Message = "Get all The Document";
                    res.Data = candidatedocument;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Document not Found";
                    res.Data = candidatedocument;
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

        #region This Api is used to Add Prebording Document

        /// <summary>
        /// Created By Ankit 16/09/2022
        ///Api >>Post >> api/preboard/addDocument
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("addDocument")]
        public async Task<ResponseBodyModel> AddDoc(AddDocumentdata model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                if (model == null)
                {
                    res.Status = false;
                    res.Message = "Model Is Invalid";
                    res.Data = null;
                }

                else
                {
                    var candidatedoc = _db.candidateDocs.Where(x => x.CandidateId == model.CandidateId && x.JobId == model.JobId
                     && x.IsActive && !x.IsDeleted && x.CompanyId == model.CompanyId && x.OrgId == model.OrgId).FirstOrDefault();
                    if (candidatedoc != null)
                    {
                        var prebordindocument = await _db.prebordingDocuments.Where(x => x.CandidateId == model.CandidateId && x.JobId == model.JobId
                     && x.IsActive && !x.IsDeleted && x.CompanyId == model.CompanyId && x.OrgId == model.OrgId).FirstOrDefaultAsync();
                        if (prebordindocument == null)
                        {
                            PrebordingDocument obj = new PrebordingDocument
                            {
                                CandidateId = model.CandidateId,
                                JobId = model.JobId,
                                DocumentId = model.DocumentId,
                                PanCard = model.PanCard,
                                AadharCard = model.AadharCard,
                                Passport = model.Passport,
                                BankPassbook = model.BankPassbook,
                                Marksheet10Th = model.Marksheet10Th,
                                Marksheet11Th = model.Marksheet11Th,
                                Marksheet12Th = model.Marksheet12Th,
                                UgLastSemMarksheet = model.UgLastSemMarksheet,
                                PgLastSemMarksheet = model.PgLastSemMarksheet,
                                UgDegree = model.UgDegree,
                                PgDegree = model.PgDegree,
                                Certificate = model.Certificate,
                                ExperienceLetter = model.ExperienceLetter,
                                PaySlips3months = model.PaySlips3months,
                                Resignation = model.Resignation,
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false,
                                CompanyId = candidatedoc.CompanyId,
                                OrgId = candidatedoc.OrgId
                            };
                            _db.prebordingDocuments.Add(obj);
                            await _db.SaveChangesAsync();

                            candidatedoc.IsDocumentSubmited = true;
                            _db.Entry(candidatedoc).State = EntityState.Modified;
                            _db.SaveChanges();
                            res.Status = true;
                            res.Message = "Document Added Successfully";
                            res.Data = obj;
                        }
                        else
                        {
                            prebordindocument.CandidateId = model.CandidateId;
                            prebordindocument.JobId = model.JobId;
                            prebordindocument.PanCard = model.PanCard;
                            prebordindocument.AadharCard = model.AadharCard;
                            prebordindocument.Passport = model.Passport;
                            prebordindocument.BankPassbook = model.BankPassbook;
                            prebordindocument.Marksheet10Th = model.Marksheet10Th;
                            prebordindocument.Marksheet11Th = model.Marksheet11Th;
                            prebordindocument.Marksheet12Th = model.Marksheet12Th;
                            prebordindocument.UgLastSemMarksheet = model.UgLastSemMarksheet;
                            prebordindocument.PgLastSemMarksheet = model.PgLastSemMarksheet;
                            prebordindocument.UgDegree = model.UgDegree;
                            prebordindocument.PgDegree = model.PgDegree;
                            prebordindocument.Certificate = model.Certificate;
                            prebordindocument.ExperienceLetter = model.ExperienceLetter;
                            prebordindocument.PaySlips3months = model.PaySlips3months;
                            prebordindocument.Resignation = model.Resignation;
                            prebordindocument.UpdatedBy = claims.employeeId;
                            prebordindocument.UpdatedOn = DateTime.Now;
                            prebordindocument.IsActive = true;
                            prebordindocument.IsDeleted = false;
                            prebordindocument.CompanyId = candidatedoc.CompanyId;
                            prebordindocument.OrgId = candidatedoc.OrgId;


                            _db.Entry(prebordindocument).State = EntityState.Modified;
                            _db.SaveChanges();

                            candidatedoc.IsDocumentSubmited = true;
                            _db.Entry(candidatedoc).State = EntityState.Modified;
                            _db.SaveChanges();
                            res.Status = true;
                            res.Message = "Document Updated Successfully";
                            res.Data = prebordindocument;
                        }

                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "Document Not Added";
                        res.Data = null;
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

        #endregion This Api is used to Upload Preboard Document

        #region This Api Use To Send Document Link In Mail
        ///// <summary>
        ///// Create By ankit Date-17-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("sendmaildoc")]
        public async Task SendDoc(int CandidateId, string Url, ClaimsHelperModel claims)
        {
            try
            {
                var candidate = (from c in _db.Candidates
                                 join j in _db.JobPosts on c.JobId equals j.JobPostId
                                 where c.CandidateId == CandidateId
                                 select new
                                 {
                                     c.CandidateName,
                                     c.CompanyName,
                                     j.JobTitle,
                                     c.InterViewType,
                                     c.Email,
                                 }).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId
                ).Select(x => x.DisplayName).FirstOrDefault();
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
                string htmlBody = JobHelper.DocumentVarification
                         .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                         .Replace("<|JOBTITLE|>", candidate.JobTitle)
                         .Replace("<|UPLOADDOCUMENT|>", Url)
                         .Replace("<|RECRUITERNAME|>", recruiterName)
                         .Replace("<|COMPANYNAME|>", candidate.CompanyName)
                         .Replace("<|IMAGE_PATH|>", "emossy.png")
                         .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                         .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Document Verification",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { candidate.Email },
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

        #region This api use for upload documents

        /// <summary>
        ///Created By Ankit On 16-09-2022
        /// </summary> Api >> Post >> api/preboard/uploaddocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploaddocuments")]
        [AllowAnonymous]
        public async Task<UploadDocumentResponse> UploadDocments()
        {
            UploadDocumentResponse result = new UploadDocumentResponse();
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

                        string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Candidatedocuments/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\Candidatedocuments\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This api use for upload documents for preaboarding document

        /// <summary>
        /// Created By Ankit on 17-09-2022
        /// </summary>
        public class Documentdata
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public bool PanCard { get; set; }
            public bool AadharCard { get; set; }
            public bool Passport { get; set; }
            public bool BankPassbook { get; set; }
            public bool Marksheet10Th { get; set; }
            public bool Marksheet11Th { get; set; }
            public bool Marksheet12Th { get; set; }
            public bool UgLastSemMarksheet { get; set; }
            public bool PgLastSemMarksheet { get; set; }
            public bool UgDegree { get; set; }
            public bool PgDegree { get; set; }
            public bool Certificate { get; set; }
            public bool ExperienceLetter { get; set; }
            public bool PaySlips3months { get; set; }
            public bool Resignation { get; set; }
            public string Reason { get; set; }
            public string Url { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 17-09-2022
        /// </summary>
        public class DocumentdataGet
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public string CandidateName { get; set; }
            public string MobileNumber { get; set; }
            public bool PanCard { get; set; }
            public bool AadharCard { get; set; }
            public bool Passport { get; set; }
            public bool BankPassbook { get; set; }
            public bool Marksheet10Th { get; set; }
            public bool Marksheet11Th { get; set; }
            public bool Marksheet12Th { get; set; }
            public bool UgLastSemMarksheet { get; set; }
            public bool PgLastSemMarksheet { get; set; }
            public bool UgDegree { get; set; }
            public bool PgDegree { get; set; }
            public bool Certificate { get; set; }
            public bool ExperienceLetter { get; set; }
            public bool PaySlips3months { get; set; }
            public bool Resignation { get; set; }
            public bool IsDocumentSubmited { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 17-09-2022
        /// </summary>
        public class AddDocumentdata
        {
            public int DocumentId { get; set; }
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public string PanCard { get; set; }
            public string AadharCard { get; set; }
            public string Passport { get; set; }
            public string BankPassbook { get; set; }
            public string Marksheet10Th { get; set; }
            public string Marksheet11Th { get; set; }
            public string Marksheet12Th { get; set; }
            public string UgLastSemMarksheet { get; set; }
            public string PgLastSemMarksheet { get; set; }
            public string UgDegree { get; set; }
            public string PgDegree { get; set; }
            public string ExperienceLetter { get; set; }
            public string PaySlips3months { get; set; }
            public string Resignation { get; set; }
            public string Certificate { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
        }
        public class GetByDocumentdata
        {
            public int CandidateId { get; set; }
            public int JobId { get; set; }
            public string CandidateName { get; set; }
            public string MobileNumber { get; set; }
            public string PanCard { get; set; }
            public string AadharCard { get; set; }
            public string Passport { get; set; }
            public string BankPassbook { get; set; }
            public string Marksheet10Th { get; set; }
            public string Marksheet11Th { get; set; }
            public string Marksheet12Th { get; set; }
            public string UgLastSemMarksheet { get; set; }
            public string PgLastSemMarksheet { get; set; }
            public string UgDegree { get; set; }
            public string PgDegree { get; set; }
            public string ExperienceLetter { get; set; }
            public string PaySlips3months { get; set; }
            public string Resignation { get; set; }
            public string Certificate { get; set; }
        }

        #region This Api Use To Get Document Data

        /// <summary>
        /// Create By Ankit Jain Date-06-09-2022
        /// Api >> Get >> api/preboard/getalldocument
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldocument")]
        public async Task<ResponseBodyModel> GetAllDocument(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<DocOther> List = new List<DocOther>();
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted
                        && x.CompanyId == claims.companyId && x.CandidateId == candidateId);
                if (candidate != null)
                {
                    if (candidate.DocUrlOther == null)
                    {
                        res.Message = "No Document Found";
                        res.Status = false;
                        res.Data = null;
                    }
                    else
                    {
                        var response = new Documenthelper
                        {
                            CandidateId = candidate.CandidateId,
                            DocUrl10 = candidate.DocUrl10,
                            DocUrl12 = candidate.DocUrl12,
                            DocUrlUg = candidate.DocUrlUg,
                            DocUrlPg = candidate.DocUrlPg,
                            DocUrlPan = candidate.DocUrlPan,
                            DocUrlAAdhar = candidate.DocUrlAAdhar,
                            DocUrlOther = candidate.DocUrlOther.Split(',')
                                    .Select(x => new DocOther
                                    {
                                        DocUrlOther = x,
                                    }).ToList(),
                        };
                        res.Message = "Document Data Found";
                        res.Status = true;
                        res.Data = response;
                    }
                }
                else
                {
                    res.Message = "No Document Found";
                    res.Status = false;
                    res.Data = null;
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

        #region This Api Use to Add Reason

        /// <summary>
        /// Created By Ankit On 18-08-2022
        /// Api >> Post >> api/preboard/addnextstep
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addnextstep")]
        public async Task<ResponseBodyModel> AddNextStep(StageStatus model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candiatedata = await _db.Candidates.Where(x => x.CandidateId == model.CandidateId && x.JobId == model.JobId && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (candiatedata != null)
                {
                    StageStatus obj = new StageStatus
                    {
                        CandidateId = model.CandidateId,
                        StageId = (Guid)candiatedata.StageId,
                        EmployeeId = claims.employeeId,
                        CompanyId = claims.companyId,
                        Reason = model.Reason,
                        PrebordingStageId = candiatedata.PrebordingStages,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.StageStatuses.Add(obj);
                    await _db.SaveChangesAsync();

                    candiatedata.PrebordingStages = PreboardingStages.Verfiy_Info;
                    _db.Entry(candiatedata).State = EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Add Reason";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Not Add Reason";
                    res.Status = true;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use to Add Reason

        #region Api To Get Candidate List In Archived

        /// <summary>
        /// API >> Get >> api/preboard/preboardarchived
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("preboardarchived")]
        public async Task<ResponseBodyModel> ArchivedCandidateList(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidateList = await _db.Candidates.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId
                                    && (x.StageType == StageFlowType.Archived || x.PrebordingStages == PreboardingStages.Archived)).
                    Select(x => new CandidateOnJoined()
                    {
                        CandidateId = x.CandidateId,
                        CandidateName = x.CandidateName,
                        MobileNumber = x.MobileNumber,
                        Email = x.Email,
                        DateOfBirth = x.DateOfBirth,
                        RelevantExperience = x.RelevantExperience,
                        Experience = x.Experience,
                        Qualifications = x.Qualifications,
                        NoticePeriod = x.NoticePeriod,
                        InterViewType = x.InterViewType,
                        Availabiltys = x.Availabilitys,
                        Gender = x.Gender,
                        JobRole = _db.JobPosts.Where(j => j.JobPostId == x.JobId).Select(j => j.JobTitle).FirstOrDefault(),
                        JoinedOn = null,
                        jobRoleId = x.JobId
                    }).ToListAsync();

                if (candidateList.Count != 0)
                {
                    if (page == null || count == null)
                    {
                        res.Message = "Candidate list Found";
                        res.Status = true;
                        res.Data = candidateList;
                    }
                    else
                    {
                        res.Message = "Candidate list Found";
                        res.Status = true;
                        if (page.HasValue && count.HasValue)
                        {
                            res.Data = new PaginationData
                            {
                                TotalData = candidateList.Count,
                                Counts = (int)count,
                                List = candidateList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                        }
                        else
                        {
                            res.Data = candidateList;
                        }
                    }
                }
                else
                {
                    res.Message = "Candidate List Empty";
                    res.Status = false;
                    res.Data = candidateList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Candidate List In Archived

        #region This api use for upload documents for preaboarding document

        /// <summary>
        ///Created By Ankit On 19-05-2022
        /// </summary>route api/preboard/uploadpreboarddocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadpreboarddocuments")]
        public async Task<UploadDocumentResponse> UploadPreboardDocments()
        {
            UploadDocumentResponse result = new UploadDocumentResponse();
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

                        string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/preboarddocuments/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\preboarddocuments\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This api use for upload documents for preaboarding document

        #region This Api Use To Upload Offer Letter

        /// <summary>
        ///Created By Ankit On 19-05-2022
        ///Api >> Post >> api/preboard/uploadofferletter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadofferletter")]
        public async Task<UploadDocumentResponse> UploadOfferLetter()
        {
            UploadDocumentResponse result = new UploadDocumentResponse();
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

                        string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/offerletter/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\offerletter\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This Api Use To Upload Offer Letter

        #region This Api Use To Send A Pdf In Mail

        /// <summary>
        /// Created By Ankit Date-08-08-2022
        /// Api >> Get >> api/preboard/sendcredential
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ///
        [Route("sendcredential")]
        [HttpGet]
        public Candidate SendPdf(Candidate candidate)
        {
            Candidate res = new Candidate();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            // The variable 'ex' is declared but never used
            try
            {
                SendPdfDto2 obj;
                var data = _db.CredentilDatas.Where(x => x.CandidateId == candidate.CandidateId).FirstOrDefault();
                List<CredentilData> pod = _db.CredentilDatas.Where(a => a.CandidateId == candidate.CandidateId).ToList();
                obj = new SendPdfDto2()
                {
                    P = data,
                    Credential = pod,
                };
                CreatePdf(obj, candidate);
                SendMail(candidate);
            }
            catch (Exception)
            {
                return null;
            }

            return res;
        }

        #endregion This Api Use To Send A Pdf In Mail

        #region This Api Use Create Pdf Api in Credential

        /// <summary>
        /// Create By Ankit Date - 01/09/2022
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CreatePdf(SendPdfDto2 obj, Candidate candidate)
        {
            Document document = new Document(PageSize.A4.Rotate(), 43, 43, 43, 43);
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@"C:\Users\user\Desktop\HRMS Latest Code (04082022\-HRMS2.0Backend\AspNetIdentity.WebApi\uploadimage/HiringPdf\" + id + ".pdf", FileMode.Create));
            string DirectoryURL = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\HiringPdf");
            //for create new Folder
            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
            if (!objDirectory.Exists)
            {
                Directory.CreateDirectory(DirectoryURL);
            }
            var path = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\HiringPdf\\" + candidate.CandidateId + ".pdf");
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            Pharsecell p = new Pharsecell();
            PdfPTable table = null;
            document.Open();
            Chunk glue = new Chunk(new VerticalPositionMark());
            Paragraph para = new Paragraph();
            table = new PdfPTable(1);
            table.TotalWidth = 380f;
            table.LockedWidth = true;
            table.SpacingBefore = 20f;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            Phrase Head = new Phrase();
            Paragraph head1 = new Paragraph();
            Head.Add(new Chunk(Environment.NewLine));
            Head.Add(new Chunk("Your Login Credential", FontFactory.GetFont("Arial", 20, 1)));
            head1.Alignment = Element.ALIGN_CENTER;
            //Head.Add(new Chunk(Environment.NewLine));
            head1.Add(Head);

            Phrase ph1 = new Phrase();
            Paragraph mm = new Paragraph();
            ph1.Add(new Chunk(Environment.NewLine));
            ph1.Add(new Chunk("Dear " + candidate.CandidateName + "", FontFactory.GetFont("Arial", 10, 1)));
            ph1.Add(glue);
            ph1.Add(new Chunk(Environment.NewLine));
            ph1.Add(new Chunk("Welcome to the team " + candidate.CompanyName + "! ", FontFactory.GetFont("Arial", 10, 1)));
            ph1.Add(glue);
            ph1.Add(new Chunk(Environment.NewLine));
            ph1.Add(new Chunk("We are glad to receive your acceptance. We are so excited about having you in our team! With your experience, you will be a great addition. We hope you bring a lot of positive energy with you and we can work well together and share many successes.Your documentation process is completed.Please find your login credentials below " + "", FontFactory.GetFont("Arial", 10, 1)));
            ph1.Add(glue);
            ph1.Add(new Chunk(Environment.NewLine));
            ph1.Add(new Chunk("Best Regards", FontFactory.GetFont("Arial", 10, 1)));
            ph1.Add(glue);
            ph1.Add(new Chunk(Environment.NewLine));
            ph1.Add(new Chunk("HR Executive" + candidate.CompanyName + " ", FontFactory.GetFont("Arial", 10, 1)));
            ph1.Add(glue);
            ph1.Add(new Chunk(" ", FontFactory.GetFont("Arial", 10, 1)));
            mm.Add(ph1);
            para.Add(mm);
            Phrase ph5 = new Phrase();
            Paragraph mmc = new Paragraph();
            ph5.Add(new Chunk(" ", FontFactory.GetFont("Arial", 10, 1)));
            mmc.Add(ph5);
            para.Add(mmc);

            document.Add(para);
            table = new PdfPTable(4);
            PdfPCell cell2 = new PdfPCell(new Phrase("Employee Login"));
            cell2.Colspan = 4;
            cell2.HorizontalAlignment = 1;

            table.AddCell(cell2);
            table.AddCell("Name");
            table.AddCell("Url");
            table.AddCell("User Name");
            table.AddCell("Password");

            foreach (var item in obj.Credential)
            {
                table.AddCell(item.Name);
                table.AddCell(item.Url);
                table.AddCell(item.UserName);
                table.AddCell(item.Password);
            }
            document.Add(table);
            Paragraph para1 = new Paragraph();
            Phrase ph2 = new Phrase();
            Paragraph mm1 = new Paragraph();
            //ph2.Add(new Chunk("Total Amount:" + obj.p.ETotalAmount, FontFactory.GetFont("Arial", 10, 1)));
            mm1.Add(ph2);
            mm1.Alignment = Element.ALIGN_RIGHT;
            para1.Add(mm1);
            document.Add(para1);

            Paragraph para3 = new Paragraph();
            Phrase ph3 = new Phrase();
            Paragraph mm3 = new Paragraph();
            //ph3.Add(new Chunk("Best Regards", FontFactory.GetFont("Arial", 10, 1)));
            ph3.Add(glue);
            ph3.Add(new Chunk(Environment.NewLine));
            //ph3.Add(new Chunk("HR Executive" + data.CompanyName + " ", FontFactory.GetFont("Arial", 10, 1)));
            ph3.Add(glue);
            mm3.Alignment = Element.ALIGN_RIGHT;
            mm3.Add(ph3);
            para3.Add(mm3);
            document.Add(para3);
            document.Close();
            return true;
        }

        public class Pharsecell
        {
            public PdfPCell Pc(Phrase phrase, int align)
            {
                PdfPCell cell = new PdfPCell(phrase);
                //cell.BorderColor = System.Drawing.Color.WHITE;
                //cell.VerticalAlignment = PdfCell.ALIGN_TOP;
                cell.HorizontalAlignment = align;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                return cell;
            }
        }

        #endregion This Api Use Create Pdf Api in Credential

        #region This Api Use To Send credentials
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public bool SendMail(Candidate candidate)
        {
            try
            {
                //string pdflocation = ConfigurationManager.AppSettings["PDFLocation"] + CandidateId + ".pdf";
                SmtpMail oMail = new SmtpMail("TryIt");

                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                oMail.To = candidate.Email;

                // Set email subject
                oMail.Subject = "Your Credential";

                oMail.TextBody = "Find your Credential";

                oMail.HtmlBody = JobHelper.FindyourCredential
                            .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                            .Replace("|COMPANYNAME|>", candidate.CompanyName);

                SmtpServer oServer = new SmtpServer("smtp.office365.com");

                oServer.User = ConfigurationManager.AppSettings["MailUser"];

                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];


                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage/HiringPdf/" + candidate.CandidateId + ".pdf");
                oMail.AddAttachment(attachmentPath);

                // use 587 TLS port
                oServer.Port = 587;

                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                Console.WriteLine("start to send email over TLS...");

                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);

                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use Add Candidate Credential

        /// <summary>
        /// Created By Ankit 08/08/2022
        /// Api >> Post >> api/preboard/addcandidatecredential
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addcandidatecredential")]
        public async Task<ResponseBodyModel> RevokeMail(CredentilData model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId &&
                x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                if (candidate != null)
                {
                    CredentilData obj = new CredentilData
                    {
                        CandidateId = model.CandidateId,
                        Url = model.Url,
                        CredentialMessage = model.CredentialMessage,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    _db.CredentilDatas.Add(obj);
                    await _db.SaveChangesAsync();

                    await SendMailCredentials(obj.CandidateId, claims);
                    candidate.IsCredentialProvided = true;
                    _db.Entry(candidate).State = EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Credential Send Successfully";
                    res.Data = candidate;
                    res.Status = true;
                }
                else
                {
                    res.Message = "model is invalid";
                    res.Data = candidate;
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


        #endregion This Api Use Send By Offer Letter

        #region This Api Use To Send credentials
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summa
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("sendmailcredential")]
        public async Task SendMailCredentials(int candidateId, ClaimsHelperModel claims)
        {
            try
            {
                var candidate = (from c in _db.Candidates
                                 join R in _db.CredentilDatas on c.CandidateId equals R.CandidateId
                                 where c.CandidateId == candidateId
                                 select new
                                 {
                                     c.Email,
                                     R.CredentialMessage,
                                     R.Url,
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
                string htmlBody = JobHelper.CredentialTemplete
                  .Replace("<|FINDYOURCREDENTIAL|>", candidate.CredentialMessage)
                  .Replace("<|IMAGE_PATH|>", "emossy.png")
                  .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                  .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Find your Credential",
                    MailBody = htmlBody,
                    Url = candidate.Url,
                    MailTo = new List<string>() { candidate.Email },
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

        #region This api use for upload 

        /// <summary>
        ///Created By Ankit On 16-09-2022
        /// </summary> Api >> Post >> api/preboard/uploadcredential
        /// <returns></returns>
        [HttpPost]
        [Route("uploadcredential")]
        [AllowAnonymous]
        public async Task<UploadDocumentResponse> UploadCredential()
        {
            UploadDocumentResponse result = new UploadDocumentResponse();
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

                        string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/CandidateCredential/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\CandidateCredential\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion This api use for upload documents for preaboarding document

        #region This Api Use To Send Offer Letter
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("test")]
        public async Task SendOfferLetter(int candidateId, ClaimsHelperModel claims)
        {
            try
            {
                var candidate = (from c in _db.Candidates
                                 join j in _db.JobPosts on c.JobId equals j.JobPostId
                                 where c.CandidateId == candidateId
                                 select new
                                 {
                                     c.CandidateName,
                                     c.CompanyName,
                                     c.JoinedDate,
                                     j.JobTitle,
                                     c.Email,
                                     c.OfferLetter
                                 }).FirstOrDefault();

                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var recruiterName = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault();

                var st = (DateTime)candidate.JoinedDate;
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                      && y.IsActive && !y.IsDeleted)
                    .Select(x => new
                    {
                        x.RegisterAddress,
                        x.RegisterCompanyName

                    }).FirstOrDefault();
                var startDate = st.ToString("dd/MM/yyyy");
                var startTime = st.ToString("hh:mm tt");

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
                string htmlBody = JobHelper.OfferLetter
                         .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                         .Replace("<|JOBTITLE|>", candidate.JobTitle)
                         .Replace("<|OFFERDATE|>", startDate)
                         .Replace("<|IMAGE_PATH|>", "emossy.png")
                         .Replace("<|HRCONTACT|>", employeedata.MobilePhone)
                         .Replace("<|HRNAME|>", recruiterName)
                         .Replace("<|COMPANYNAME|>", candidate.CompanyName)
                         .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                         .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Please find Your Offer Letter",
                    MailBody = htmlBody,
                    Url = candidate.OfferLetter,
                    MailTo = new List<string>() { candidate.Email },
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

        #region This Api Use Send REminder Add Api

        /// <summary>
        /// Created By Ankit 08/08/2022
        /// Api >> Get >> api/preboard/sendreminder
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("sendreminder")]
        public async Task<ResponseBodyModel> Reminder(int candidateId, int stage)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == candidateId &&
                x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                if (candidate != null)
                {
                    await SendReminder(candidateId, stage, claims);

                    res.Message = "Send Reminder";
                    res.Data = candidate;
                    res.Status = true;
                }
                else
                {
                    res.Message = "model is invalid";
                    res.Data = candidate;
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

        #endregion This Api Use Send By Offer Letter

        #region This Api Use Send Revoke Candidate

        /// <summary>
        /// Created By Ankit 08/08/2022
        /// Api >> Post >> api/preboard/sendrevokereasonmail
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("sendrevokereasonmail")]
        public async Task<ResponseBodyModel> RevokeMail(RevokeReason model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId &&
                x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted);
                if (candidate != null)
                {
                    RevokeReason obj = new RevokeReason
                    {
                        CandidateId = model.CandidateId,
                        RevokReasons = model.RevokReasons,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    _db.RevokeReasons.Add(obj);
                    await _db.SaveChangesAsync();

                    await SendRevoke(obj.CandidateId, claims);

                    res.Message = "Remove This Candidate";
                    res.Status = true;
                }
                else
                {
                    res.Message = "model is invalid";
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

        #endregion This Api Use Send By Offer Letter

        #region This Api Use To Send Reminder Mail
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public async Task SendReminder(int CandidateId, int stage, ClaimsHelperModel claims)
        {
            try
            {
                var employee = _db.Employee.Where(x => x.IsActive && !x.IsDeleted
                && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var companylist = _db.Company.Where(y => y.CompanyId == claims.companyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                var employeedata = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.employeeId).FirstOrDefault();
                var candidate = _db.Candidates.Where(x => x.CandidateId == CandidateId).ToList().LastOrDefault();
                if (stage == 1)
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
                    string htmlBody = JobHelper.ReminderTempletesDocSubmission
                      .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                      .Replace("<|IMAGE_PATH|>", "emossy.png")
                      .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                      .Replace("<|COMPANYNAME|>", candidate.CompanyName)
                      .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                      .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { candidate.Email },
                        SmtpSettings = smtpsettings,
                    };
                    await SmtpMailHelper.SendMailAsync(sendMailObject);
                }
                else
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
                    string htmlBody = JobHelper.ReminderTempletesOfferAccpetation
                      .Replace("<|CANDIDATENAME|>", candidate.CandidateName)
                      .Replace("<|IMAGE_PATH|>", "emossy.png")
                      .Replace("<|RECRUITERNAME|>", employeedata.DisplayName)
                      .Replace("<|COMPANYNAME|>", candidate.CompanyName)
                      .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                      .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                    SendMailModelRequest sendMailObject = new SendMailModelRequest()
                    {
                        IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                        Subject = "Reminder",
                        MailBody = htmlBody,
                        MailTo = new List<string>() { candidate.Email },
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

        #region This Api Use To Send Revoke Mail
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///Api >> Get >> api/preboard/sendrevokemail
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>

        [HttpGet]
        [Route("sendrevokemail")]
        public async Task SendRevoke(int candidateId, ClaimsHelperModel claims)
        {
            try
            {
                var candidaterevoke = (from c in _db.Candidates
                                       join R in _db.RevokeReasons on c.CandidateId equals R.CandidateId
                                       where c.CandidateId == candidateId
                                       select new
                                       {
                                           c.CandidateName,
                                           c.Email,
                                           R.RevokReasons
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
                string htmlBody = JobHelper.RevokeTemplete
                  .Replace("<|REVOKEREASON|>", candidaterevoke.RevokReasons)
                  .Replace("<|IMAGE_PATH|>", "emossy.png")
                  .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                  .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = claims.IsSmtpProvided,
                    Subject = "Revoke Candidate",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { candidaterevoke.Email },
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

        #region This Api Use To Requested Candidate

        #region This Api use To Add Candidate Request

        /// <summary>
        /// Created By Ankit 16/10/2022
        ///Api >>Post >> api/preboard/addcandidaterequest
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]

        [Route("addcandidaterequest")]
        public async Task<ResponseBodyModel> AddCandidateRequest(RequestCandidate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Status = false;
                    res.Message = "model is invalid";
                    res.Data = null;
                }
                else
                {
                    RequestCandidate reqobj = new RequestCandidate
                    {
                        profile = model.profile,
                        NoOfCandidate = model.NoOfCandidate,
                        Priority = model.Priority,
                        Experience = model.Experience,
                        Description = model.Description,
                        Status = RequestCandidatStatus.Pending,
                        StatusName = Enum.GetName(typeof(RequestCandidatStatus), RequestCandidatStatus.Pending),
                        priorityName = Enum.GetName(typeof(RequestCandidatePriority), model.Priority),
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        //EmployeeName = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.userId).Select(x => x.DisplayName).FirstOrDefault(),
                        EmployeeName = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.EmployeeId == claims.employeeId).Select(x => x.DisplayName).FirstOrDefault(),
                    };
                    _db.RequestCandidates.Add(reqobj);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Request Candidate Added Successfully";
                    res.Data = reqobj;
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

        #region This Api use To Update Candidate Request

        /// <summary>
        /// Created By Ankit 16/10/2022
        ///Api >>Put >> api/preboard/updatecandidaterequest
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatecandidaterequest")]
        public async Task<ResponseBodyModel> UpdateCandidateRequest(RequestCandidate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestcandidate = await _db.RequestCandidates.FirstOrDefaultAsync(x => x.RequestCandidateId == model.RequestCandidateId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (requestcandidate != null)
                {
                    requestcandidate.profile = model.profile;
                    requestcandidate.NoOfCandidate = model.NoOfCandidate;
                    requestcandidate.Priority = model.Priority;
                    requestcandidate.UpdatedBy = claims.employeeId;
                    requestcandidate.UpdatedOn = DateTime.Now;
                    requestcandidate.Experience = model.Experience;
                    requestcandidate.Description = model.Description;
                    requestcandidate.priorityName = Enum.GetName(typeof(RequestCandidatePriority), model.Priority);
                    requestcandidate.IsActive = true;
                    requestcandidate.IsDeleted = false;
                    requestcandidate.CompanyId = claims.companyId;
                    requestcandidate.OrgId = claims.orgId;

                    _db.Entry(requestcandidate).State = EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Request Candidate Updated Successfully";
                    res.Data = requestcandidate;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Request Candidate Not Updated";
                    res.Data = requestcandidate;
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

        #region This Api Use for Get All Request Candidate
        /// <summary>
        /// created by Ankit jain on 17/10/2022
        /// Api >> Get >> api/preboard/gatallyourrequestcandidate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gatallyourrequestcandidate")]
        public async Task<ResponseBodyModel> GetAllRequestCandidate(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestcandidate = await _db.RequestCandidates.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                 && x.OrgId == claims.orgId && x.CreatedBy == claims.employeeId).OrderByDescending(x => x.CreatedOn).ToListAsync();
                if (requestcandidate.Count > 0)
                {
                    res.Message = "Get Request Candidate Data";
                    res.Status = true;
                    res.Data = requestcandidate;

                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = requestcandidate.Count,
                            Counts = (int)count,
                            List = requestcandidate.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                        res.Data = requestcandidate;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = null;
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

        #region This Api Use for Get All Request Candidate
        /// <summary>
        /// created by Ankit jain on 17/10/2022
        /// Api >> Get >> api/preboard/getrequestcandidate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrequestcandidate")]
        public async Task<ResponseBodyModel> GetRequestCandidateById(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                var requestcandidate = await _db.RequestCandidates.Where(x => x.OrgId == claims.orgId &&
                x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                 ).OrderByDescending(x => x.CreatedOn).ToListAsync();

                if (requestcandidate.Count > 0)
                {
                    res.Message = "Get Request Candidate Data";
                    res.Status = true;
                    res.Data = requestcandidate;

                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = requestcandidate.Count,
                            Counts = (int)count,
                            List = requestcandidate.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                        res.Data = requestcandidate;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = requestcandidate;
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

        #region This Api Use for Delete Request Candidate By Id
        /// <summary>
        /// created by Ankit jain on 17/10/2022
        /// Api >> Delete >> api/preboard/removerequestcandidate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removerequestcandidate")]
        public async Task<ResponseBodyModel> RemoveRequestCandidateById(int requestcandidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestcandidate = await _db.RequestCandidates.Where(x => x.RequestCandidateId == requestcandidateId && x.OrgId == claims.orgId &&
                x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                 ).FirstOrDefaultAsync();
                if (requestcandidate != null)
                {
                    requestcandidate.DeletedBy = claims.companyId;
                    requestcandidate.DeletedOn = DateTime.Now;
                    requestcandidate.IsDeleted = true;
                    requestcandidate.IsActive = false;

                    _db.Entry(requestcandidate).State = EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Remove Request Candidate Data";
                    res.Status = true;
                    res.Data = requestcandidate;
                }
                else
                {
                    res.Message = "Data Not Found";
                    res.Status = false;
                    res.Data = requestcandidate;
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

        #region This Api use To Update Candidate Request Status

        /// <summary>
        /// Created By Ankit 16/09/2022
        ///Api >>Put >> api/preboard/updatecandidaterequeststatus
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatecandidaterequeststatus")]
        public async Task<ResponseBodyModel> UpdateCandidateRequestStatus(UpdateStatus model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var requestcandidate = await _db.RequestCandidates.FirstOrDefaultAsync(x => x.RequestCandidateId == model.RequestCandidateId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (requestcandidate != null)
                {
                    requestcandidate.Status = model.Status;
                    requestcandidate.StatusName = Enum.GetName(typeof(RequestCandidatStatus), model.Status);
                    requestcandidate.UpdatedBy = claims.employeeId;
                    requestcandidate.UpdatedOn = DateTime.Now;
                    requestcandidate.IsActive = true;
                    requestcandidate.IsDeleted = false;
                    requestcandidate.CompanyId = claims.companyId;
                    requestcandidate.OrgId = claims.orgId;

                    _db.Entry(requestcandidate).State = EntityState.Modified;
                    _db.SaveChanges();

                    res.Status = true;
                    res.Message = "Request Candidate Status Updated Successfully";
                    res.Data = requestcandidate;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Request Candidate Not Updated";
                    res.Data = requestcandidate;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        public class UpdateStatus
        {
            public int RequestCandidateId { get; set; }

            public RequestCandidatStatus Status { get; set; }
        }
        #endregion

        #region This Api Use for Get Data By Id In Candidate
        /// <summary>
        /// created by Ankit jain on 17/10/2022
        /// Api >> Get >> api/preboard/getcandidatedetailonreview
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getcandidatedetailonreview")]
        public async Task<ResponseBodyModel> GetCandidateDetailsOnReview(string token)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var key = ConfigurationManager.AppSettings["EncryptKey"];
                var decryptData = EncryptDecrypt.DecryptData(key, token.Replace(" ", "+"));
                tokenDataForInterviewMail data = JsonConvert.DeserializeObject<tokenDataForInterviewMail>(decryptData);
                dynamic response;
                var candidateInterview = await _db.CandidateInterviews.Where(x => x.Id == data.CurrentId
                        && x.CandidateId == data.CandidateId).FirstOrDefaultAsync();
                if (candidateInterview.IsReschedule)
                {
                    response = await _db.Candidates.Where(x => x.CandidateId == data.CandidateId)
                        .Select(c => new
                        {
                            c.CandidateName,
                            c.Experience,
                            c.UploadResume,
                            c.MobileNumber,
                            c.Email,
                            c.CompanyName,
                            c.Gender,
                            IsReviewSubmited = candidateInterview.IsReviewSubmited,
                            IsLinkExpire = !(c.CurrentMeetingSecduleId == data.CurrentId && candidateInterview.RescheduleCount == data.ReScheduleCount),
                            ExpireDate = data.EndTime,

                        }).FirstOrDefaultAsync();
                }
                else
                {
                    response = await _db.Candidates.Where(x => x.CandidateId == data.CandidateId)
                        .Select(c => new
                        {
                            c.CandidateName,
                            c.Experience,
                            c.UploadResume,
                            c.MobileNumber,
                            c.Email,
                            c.CompanyName,
                            c.Gender,
                            IsReviewSubmited = candidateInterview.IsReviewSubmited,
                            IsLinkExpire = (c.CurrentMeetingSecduleId != data.CurrentId),
                            ExpireDate = data.EndTime,

                        }).FirstOrDefaultAsync();
                }
                res.Data = response;
                res.Message = "Candidate Data";
                res.Status = true;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region This Api use To Add Reason in inerview 

        /// <summary>
        /// Created By Ankit 1/10/2022
        ///Api >>Post >> api/preboard/addreasonininerview
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("addreasonininerview")]
        public async Task<ResponseBodyModel> AddReasonInterview(Interviewperclass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var key = ConfigurationManager.AppSettings["EncryptKey"];
                var decryptData = EncryptDecrypt.DecryptData(key, model.Token.Replace(" ", "+"));
                tokenDataForInterviewMail data = JsonConvert.DeserializeObject<tokenDataForInterviewMail>(decryptData);
                var candidate = await _db.Candidates.Where(x => x.IsActive && !x.IsDeleted &&
                     x.CandidateId == data.CandidateId /*&&x.CompanyId == claims.companyId && x.OrgId == claims.orgId*/).FirstOrDefaultAsync();

                if (candidate != null)
                {
                    var candidateInterview = await _db.CandidateInterviews.Where(x => x.Id == data.CurrentId
                        && x.CandidateId == data.CandidateId).FirstOrDefaultAsync();
                    if (candidateInterview != null)
                    {
                        candidateInterview.IsReviewSubmited = true;
                        _db.Entry(candidateInterview).State = EntityState.Modified;
                        StageStatus obj = new StageStatus
                        {
                            CandidateId = data.CandidateId,
                            StageId = (Guid)candidate.StageId,
                            CompanyId = candidate.CompanyId,
                            IsReviewSubmited = true,
                            JobId = candidate.JobId,
                            CreatedBy = claims.employeeId,
                            OrgId = candidate.OrgId,
                            EmployeeId = data.EmployeeId,
                            Reason = data.ReScheduleCount != 0 ? "Reschedule Reason " + data.ReScheduleCount + " - " + model.Reason : model.Reason,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == candidate.JobId && x.CandidateId == candidate.CandidateId),
                            PrebordingStageId = candidate.PrebordingStages,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CommentBy = InterviewCommentBy.Interviewer,
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Status = true;
                        res.Message = "Added Candidate Reason";
                        res.Data = obj;
                    }
                }
                else
                {
                    res.Status = true;
                    res.Message = "Candidate Not Found";
                    res.Data = candidate;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        public class Interviewperclass
        {
            public string Reason { get; set; }
            public string Token { get; set; }
        }
        #endregion

        #endregion

        #region Helper Model Class



        public class SendPdfDto2
        {
            public CredentilData P { get; set; }
            public List<CredentilData> Credential { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class CandidateOnPreboarding
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public string JobRole { get; set; }
            public int jobRoleId { get; set; }
            public DateTime? PendingSince { get; set; }
            public string RecrutersName { get; set; }
            public bool IsCredentialProvided { get; set; }
            public DateTime? JoinedDate { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            //public string StageName { get; set; }
        }

        public class PaginationDataHiring
        {
            public int TotalData { get; set; }
            public int Counts { get; set; }
            public object List { get; set; }
            public string StageName { get; set; }

        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class CandidateHired
        {
            public int CandidateId { get; set; }
            public string Reason { get; set; }
            public int JobId { get; set; }
            public int EmployeeId { get; set; }
            //public int StageId { get; set; }
            public bool IsInterview { get; set; }
            //public int InterviewType { get; set; }
            //public bool IsPreboarding { get; set; }
            public DateTime? SechduleDate { get; set; }
        }


        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class CandidateJoined
        {
            public int CandidateId { get; set; }
            public string Reason { get; set; }
            public int JobId { get; set; }
            //public DateTime? JoinedDate { get; set; }

        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class PreboardingModelClass
        {
            public string StageName { get; set; }
            public PreBoardingCount Count { get; set; }
            public List<CandidateOnPreboarding> CandidateList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class PreBoardingCount
        {
            public int Start { get; set; }
            public int CollectInfo { get; set; }
            public int VerifyInfo { get; set; }
            public int RealeaseOffer { get; set; }
            public int OfferAcceptance { get; set; }
            public int Hired { get; set; }
            public int Joined { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 09-02-2022
        /// </summary>
        public class CandidateOnJoined
        {
            public int CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public string JobRole { get; set; }
            public DateTime? JoinedOn { get; set; }
            public string Gender { get; set; }
            public int Availability { get; set; }
            public string Availabiltys { get; set; }
            public int jobRoleId { get; set; }

            public string InterViewType { get; set; }
            public string NoticePeriod { get; set; }
            public string Qualifications { get; set; }
            public string RelevantExperience { get; set; }
            public string Experience { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public DateTime? JoinedDate { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 11-05-2022
        /// </summary>

        public class UploadDocumentResponse
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }


        public class Documenthelperclass
        {
            public int CandidateId { get; set; }
            public string Reason { get; set; }
        }
        public class Documenthelper
        {
            public int CandidateId { get; set; }
            public string DocUrl10 { get; set; }
            public string DocUrl12 { get; set; }
            public string DocUrlUg { get; set; }
            public string DocUrlPg { get; set; }
            public string DocUrlAAdhar { get; set; }
            public string DocUrlPan { get; set; }
            public List<DocOther> DocUrlOther { get; set; }

        }

        public class DocOther
        {
            public string DocUrlOther { get; set; }
        }

        public class AddOffer
        {
            public int CandidateId { get; set; }
            public string OfferLetter { get; set; }
            public string Reason { get; set; }
            public DateTime? JoinedDate { get; set; }
        }

        public class CredentialData
        {
            public int CandidateId { get; set; }
            public List<CredentilaHelpermodel> Credential { get; set; }
        }

        public class CredentilaHelpermodel
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

        }



        #endregion Helper Model Class
    }
}