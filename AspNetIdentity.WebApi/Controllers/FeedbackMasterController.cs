using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.Feedback;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 04-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/feedback")]
    public class FeedbackMasterController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Get Feedback Type

        /// <summary>
        /// Created By Harshit Mitra On 27-05-2022
        /// API >> Get >> api/feedback/getfeedbacktype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfeedbacktype")]
        public ResponseBodyModel GetFeedbackType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbackType = Enum.GetValues(typeof(FeedbackTypeConstants))
                                   .Cast<FeedbackTypeConstants>()
                                   .Select(x => new
                                   {
                                       FeedbackTypeId = (int)x,
                                       FeedbackTypeName = Enum.GetName(typeof(FeedbackTypeConstants), x).Replace("_", " "),
                                   }).ToList();

                res.Message = "Feedback Type List";
                res.Status = true;
                res.Data = feedbackType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Feedback Type

        #region Api To Add Feedback Category

        /// <summary>
        /// Modify By Harshit Mitra on 11-05-2022
        /// API >> Post >> api/feedback/addfeedbackcategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addfeedbackcategory")]
        public async Task<ResponseBodyModel> AddFeedbackCategory(AddFeedbackCategoryModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<FeedbackCategory> list = new List<FeedbackCategory>();
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
                    if (!model.ForDepartmentOnly)
                    {
                        var department = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId);
                        foreach (var item in model.DesignationIds)
                        {
                            FeedbackCategory obj = new FeedbackCategory
                            {
                                CategoryName = model.CategoryName,
                                DepartmentId = department.DepartmentId,
                                DepartmentName = department.DepartmentName,
                                DesignationId = item,
                                DesignationName = _db.Designation.Where(x => x.DesignationId == item).Select(x => x.DesignationName).FirstOrDefault(),

                                CreatedBy = claims.userId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _db.FeedbackCategories.Add(obj);
                            await _db.SaveChangesAsync();

                            List<FeedbackQuestions> list2 = new List<FeedbackQuestions>();
                            foreach (var data in model.FeedbackQuestions)
                            {
                                FeedbackQuestions obj2 = new FeedbackQuestions
                                {
                                    FBCategoryId = obj.FBCategoryId,
                                    Questions = data.Questions,

                                    CreatedBy = claims.userId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                    IsActive = true,
                                    IsDeleted = false,
                                };
                                _db.FeedbackQuestions.Add(obj2);
                                await _db.SaveChangesAsync();
                                list2.Add(obj2);
                            }
                            obj.Questions = list2;
                            list.Add(obj);
                        }
                        res.Message = "Feedback Category Added";
                        res.Status = true;
                        res.Data = list;
                    }
                    else
                    {
                        var department = await _db.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId);
                        FeedbackCategory obj = new FeedbackCategory
                        {
                            CategoryName = model.CategoryName,
                            DepartmentId = department.DepartmentId,
                            DepartmentName = department.DepartmentName,
                            DesignationId = 0,
                            DesignationName = null,

                            CreatedBy = claims.userId,
                            CreatedOn = DateTime.Now,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        _db.FeedbackCategories.Add(obj);
                        await _db.SaveChangesAsync();

                        List<FeedbackQuestions> list2 = new List<FeedbackQuestions>();
                        foreach (var data in model.FeedbackQuestions)
                        {
                            FeedbackQuestions obj2 = new FeedbackQuestions
                            {
                                FBCategoryId = obj.FBCategoryId,
                                Questions = data.Questions,

                                CreatedBy = claims.userId,
                                CreatedOn = DateTime.Now,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                                IsActive = true,
                                IsDeleted = false,
                            };
                            _db.FeedbackQuestions.Add(obj2);
                            await _db.SaveChangesAsync();
                            list2.Add(obj2);
                        }
                        obj.Questions = list2;
                        list.Add(obj);

                        res.Message = "Feedback Category Added";
                        res.Status = true;
                        res.Data = list;
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

        #endregion Api To Add Feedback Category

        #region Api To Get Feedback Category List With Filter

        /// <summary>
        /// API >> Get >> api/feedback/getfeedbackcategorytlist
        /// Created By Harshit Mitra on 04-04-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getfeedbackcategorytlist")]
        public async Task<ResponseBodyModel> AddFeedbackCategory(int? departentId, int? designationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            dynamic listDynamics = null;
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbcakCategory = await _db.FeedbackCategories.Where(x => x.CompanyId == claims.companyId &&
                        x.OrgId == claims.orgId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
                if (departentId == null || departentId == 0)
                {
                    if (designationId == null || designationId == 0)
                    {
                        var list = feedbcakCategory.Select(x => new
                        {
                            x.FBCategoryId,
                            x.CategoryName,
                            x.DepartmentName,
                            x.DesignationName,
                            TotalQuestion = _db.FeedbackQuestions.Where(y => x.FBCategoryId == y.FBCategoryId)
                                    .ToList().Count,
                        }).ToList();
                        listDynamics = list;
                    }
                    else
                    {
                        var list = feedbcakCategory.Where(x => x.DesignationId == designationId)
                            .Select(x => new
                            {
                                x.FBCategoryId,
                                x.CategoryName,
                                x.DepartmentName,
                                x.DesignationName,
                                TotalQuestion = _db.FeedbackQuestions.Where(y => x.FBCategoryId == y.FBCategoryId)
                                    .ToList().Count,
                            }).ToList();
                        listDynamics = list;
                    }
                }
                else
                {
                    if (designationId == null || designationId == 0)
                    {
                        var list = feedbcakCategory.Where(x => x.DepartmentId == departentId)
                            .Select(x => new
                            {
                                x.FBCategoryId,
                                x.CategoryName,
                                x.DepartmentName,
                                x.DesignationName,
                                TotalQuestion = _db.FeedbackQuestions.Where(y => x.FBCategoryId == y.FBCategoryId)
                                    .ToList().Count,
                            }).ToList();
                        listDynamics = list;
                    }
                    else
                    {
                        var list = feedbcakCategory.Where(x => x.DesignationId == designationId && x.DepartmentId == departentId)
                            .Select(x => new
                            {
                                x.FBCategoryId,
                                x.CategoryName,
                                x.DepartmentName,
                                x.DesignationName,
                                TotalQuestion = _db.FeedbackQuestions.Where(y => x.FBCategoryId == y.FBCategoryId)
                                    .ToList().Count,
                            }).ToList();
                        listDynamics = list;
                    }
                }
                if (listDynamics.Count > 0)
                {
                    res.Message = "Feedback Category List";
                    res.Status = true;
                    res.Data = listDynamics;
                }
                else
                {
                    res.Message = "Feedback Category Not Found";
                    res.Status = false;
                    res.Data = listDynamics;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Feedback Category List With Filter

        #region Api To Get Question List By Department and Designation

        /// <summary>
        /// Created By Harshit Mitra on 09-04-2022
        /// API >> Get >> api/feedback/getquestionlist
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getquestionlist")]
        public async Task<ResponseBodyModel> GetQuestionList(int employeeId, int designationId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetAddFeedbackModelClass> list = new List<GetAddFeedbackModelClass>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                int departmentId = await _db.Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.DepartmentId).FirstOrDefaultAsync();
                var feedbackCategory = await _db.FeedbackCategories.Where(x => x.CompanyId == claims.companyId && x.OrgId == claims.orgId &&
                        x.IsDeleted == false && x.IsActive == true && x.DepartmentId == departmentId && x.DesignationId == designationId).ToListAsync();
                if (feedbackCategory.Count > 0)
                {
                    foreach (var item in feedbackCategory)
                    {
                        GetAddFeedbackModelClass obj = new GetAddFeedbackModelClass
                        {
                            CategoryId = item.FBCategoryId,
                            CategoryName = item.CategoryName,
                            QuestionList = _db.FeedbackQuestions.Where(x => x.FBCategoryId == item.FBCategoryId)
                                    .Select(x => new GetAddQuestionList
                                    {
                                        QuestionId = x.QuestionId,
                                        Question = x.Questions,
                                        FeedBackMarks = 5,
                                    }).ToList(),
                        };
                        list.Add(obj);
                    }
                    if (feedbackCategory.Count > 0)
                    {
                        res.Message = "Question List";
                        res.Status = true;
                        res.Data = list;
                    }
                    else
                    {
                        res.Message = "No Question Avaliable";
                        res.Status = false;
                        res.Data = list;
                    }
                }
                else
                {
                    res.Message = "Feedback Category Not Found";
                    res.Status = false;
                    res.Data = list;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Question List By Department and Designation

        #region Api To Add User Feedback

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// API >> Post >> api/feedback/adduserfeedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adduserfeedback")]
        public async Task<ResponseBodyModel> AddUserFeedback(AddEmployeeFeedbackModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var lastFeedbackDate = _db.EmployeeFeedbacks.Where(x => x.FeedbackGivenTo == model.EmployeeId &&
                        x.FeedBackGivenBy == claims.employeeId && x.CreatedOn.Year == DateTime.Now.Year)
                        .ToList().OrderByDescending(x => x.CreatedOn).Select(x => x.CreatedOn).FirstOrDefault();

                var checkDate = lastFeedbackDate.AddMonths(1);
                if (checkDate.Date > DateTime.Now.Date)
                {
                    res.Message = "You already Gave Feedback to this User You Can't Give Feedback to the user till next month";
                    res.Status = false;
                    return res;
                }
                else
                {
                    decimal totalQuestion = 0, totalMarks = 0;
                    var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId &&
                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId && x.IsDeleted == false && x.IsActive == true);
                    if (employee != null)
                    {
                        EmployeeFeedback obj = new EmployeeFeedback
                        {
                            FeedBackGivenBy = claims.employeeId,
                            FeedBackProviderName = claims.displayName,
                            FeedbackGivenTo = model.EmployeeId,
                            FeedBackProviderToName = employee.DisplayName,
                            GivenComment = model.GivenComment,

                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                        };
                        _db.EmployeeFeedbacks.Add(obj);
                        await _db.SaveChangesAsync();
                        foreach (var item in model.EmployeeFeedback)
                        {
                            foreach (var que in item.QuestionList)
                            {
                                EmployeeFeeedBackQuestion qobj = new EmployeeFeeedBackQuestion
                                {
                                    EmpFeedbackId = obj.EmpFeedbackId,
                                    CategoryId = item.CategoryId,
                                    QuestionId = que.QuestionId,
                                    Question = que.Question,
                                    ProvidedMarks = que.FeedBackMarks,

                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedBy = claims.employeeId,
                                    CreatedOn = DateTime.Now,
                                    CompanyId = claims.companyId,
                                    OrgId = claims.orgId,
                                };
                                totalQuestion++;
                                totalMarks += qobj.ProvidedMarks;
                                _db.EmployeeFeeedBackQuestions.Add(qobj);
                                await _db.SaveChangesAsync();
                            }
                        }
                        obj.TotalMarks = (int)totalMarks;
                        obj.TotalQuestion = (int)totalQuestion;
                        obj.AverageMarks = (float)Math.Max(Decimal.Divide(totalMarks, totalQuestion), 2);
                        _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Message = "Feedback Added";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "Employee Not Found";
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

        #endregion Api To Add User Feedback

        #region Api To Get FeedBack History on User Side

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// API >> Get >> api/feedback/feedbackhistory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("feedbackhistory")]
        public async Task<ResponseBodyModel> GetFeedbackHistory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbackList = await _db.EmployeeFeedbacks.Where(x => x.FeedBackGivenBy == claims.userId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .Select(x => new
                        {
                            x.FeedbackGivenTo,
                            x.FeedBackProviderToName,
                            x.CreatedOn,
                            x.TotalQuestion,
                            x.TotalMarks,
                            x.AverageMarks,
                        }).ToListAsync();

                if (feedbackList.Count > 0)
                {
                    feedbackList = feedbackList.OrderByDescending(x => x.CreatedOn).ToList();
                    res.Message = "Feedback List";
                    res.Status = true;
                    res.Data = feedbackList;
                }
                else
                {
                    res.Message = "No Feedback Added";
                    res.Status = false;
                    res.Data = feedbackList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get FeedBack History on User Side

        #region Api To Get FeedBack History on Admin Side

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// API >> Get >> api/feedback/allfeedbackhistory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("allfeedbackhistory")]
        public async Task<ResponseBodyModel> GetAllFeedbackHistory()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbackList = await _db.EmployeeFeedbacks.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .Select(x => new
                        {
                            x.FeedBackGivenBy,
                            x.FeedBackProviderName,
                            x.FeedbackGivenTo,
                            x.FeedBackProviderToName,
                            x.CreatedOn,
                            x.TotalQuestion,
                            x.TotalMarks,
                            x.AverageMarks,
                        }).ToListAsync();

                if (feedbackList.Count > 0)
                {
                    feedbackList = feedbackList.OrderByDescending(x => x.CreatedOn).ToList();
                    res.Message = "Feedback List";
                    res.Status = true;
                    res.Data = feedbackList;
                }
                else
                {
                    res.Message = "No Feedback Added";
                    res.Status = false;
                    res.Data = feedbackList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get FeedBack History on Admin Side

        #region Api To Get User Feedback Dashboard

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// API >> Get >> api/feedback/getfeedbackdashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfeedbackdashboard")]
        public async Task<ResponseBodyModel> GetFeedbackDashboard()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetFeedBackDashboardModel response = new GetFeedBackDashboardModel();
            GetFeedbackBarModel bar = new GetFeedbackBarModel();
            List<string> monthNames = new List<string>();
            List<UserDashboardCategoryList> list = new List<UserDashboardCategoryList>();
            List<int> values = new List<int>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                #region For Bar Chart

                var currentYear = DateTime.Now.Year;
                var feedBackHistory = await _db.EmployeeFeedbacks.Where(x => x.FeedbackGivenTo == claims.employeeId && x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == claims.companyId && claims.orgId == claims.orgId).ToListAsync();
                for (int i = 1; i <= 12; i++)
                {
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                    var value = feedBackHistory.Where(x => x.CreatedOn.Year == currentYear &&
                            x.CreatedOn.Month == i).ToList().Count;
                    monthNames.Add(monthName);
                    values.Add(value);
                }
                bar.Name = monthNames;
                bar.Value = values;
                response.BarChart = bar;

                #endregion For Bar Chart

                #region For Overall Average And Category List

                var userReciveFeedback = await (from x in _db.EmployeeFeedbacks
                                                join z in _db.EmployeeFeeedBackQuestions on x.EmpFeedbackId equals z.EmpFeedbackId
                                                where x.FeedbackGivenTo == claims.employeeId && x.IsDeleted == false && x.IsActive == true &&
                                                x.CompanyId == claims.companyId && claims.orgId == claims.orgId && x.CreatedOn.Month == DateTime.Now.Month
                                                select new UserReciveFeedbackModel
                                                {
                                                    EmpFeedbakId = x.EmpFeedbackId,
                                                    CategoryId = z.CategoryId,
                                                    QuestionId = z.QuestionId,
                                                    Question = z.Question,
                                                    ProvidedMarks = z.ProvidedMarks,
                                                }).ToListAsync();

                if (userReciveFeedback.Count > 0)
                {
                    var overallAverage = 0.0;
                    var categoryist = userReciveFeedback.Select(x => x.CategoryId).Distinct().ToList();
                    var categoryCount = categoryist.Count;
                    //// Loop For Finding Category
                    foreach (var item in categoryist)
                    {
                        var category = _db.FeedbackCategories.FirstOrDefault(x => x.FBCategoryId == item);
                        UserDashboardCategoryList obj = new UserDashboardCategoryList
                        {
                            CategoryName = category.CategoryName,
                            OverallMarks = 0,
                        };
                        var questionList = userReciveFeedback.Where(x => x.CategoryId == item).Select(x => x.QuestionId).Distinct().ToList();
                        var toalQuestion = questionList.Count;
                        var totalQuestionMarks = 0;
                        foreach (var que in questionList)
                        {
                            var questions = userReciveFeedback.Where(x => x.QuestionId == que).ToList();
                            var totalQuestion = questions.Count;
                            UserReportQuestions queobj = new UserReportQuestions
                            {
                                QuestionId = que,
                                Questions = questions.Where(x => x.QuestionId == que).Select(x => x.Question).First(),
                                Marks = questions.Where(x => x.QuestionId == que).Select(x => x.ProvidedMarks).ToList().Sum() / totalQuestion,
                            };
                            totalQuestionMarks += queobj.Marks;
                        }
                        obj.OverallMarks = (float)Math.Round(decimal.Divide(totalQuestionMarks, toalQuestion), 2);
                        overallAverage += obj.OverallMarks;
                        list.Add(obj);
                    }
                    response.AverageScore = (float)Math.Round(decimal.Divide((decimal)overallAverage, categoryCount), 2);
                    response.CategoryList = list;
                }
                else
                {
                    response.AverageScore = 0;
                    response.CategoryList = list;
                }

                #endregion For Overall Average And Category List

                res.Message = "Dashboard";
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

        #endregion Api To Get User Feedback Dashboard

        #region This Api Used to Get Year

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// API >> Get >> api/feedback/getYear
        /// </summary>
        [HttpGet]
        [Route("getYear")]
        public async Task<ResponseBodyModel> GetFeedbackLineGraph(int employeeid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbackList = await _db.EmployeeFeedbacks.Where(x => x.FeedbackGivenTo == employeeid &&
                                        !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId)
                                        .Select(x => new EmpYaerResponse
                                        {
                                            Year = x.CreatedOn.Year.ToString(),
                                        }).Distinct().ToListAsync();
                if (feedbackList.Count > 0)
                {
                    feedbackList = feedbackList.OrderByDescending(x => x.Year).ToList();
                    res.Message = "Year List";
                    res.Status = true;
                    res.Data = feedbackList;
                }
                else
                {
                    res.Message = "No Feedback Added";
                    res.Status = false;
                    res.Data = feedbackList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Used to Get Year

        #region Api To Get User Feedback Report

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// API >> Get >> api/feedback/feedbackreport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("feedbackreport")]
        public async Task<ResponseBodyModel> GetUserSelectedFeedbackReport(int userId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetFeedBackReportModel response = new GetFeedBackReportModel();
            List<UserReportCategoryList> list = new List<UserReportCategoryList>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var userReciveFeedback = await (from x in _db.EmployeeFeedbacks
                                                join z in _db.EmployeeFeeedBackQuestions on x.EmpFeedbackId equals z.EmpFeedbackId
                                                where x.FeedbackGivenTo == userId && x.IsDeleted == false && x.IsActive == true &&
                                                x.CompanyId == claims.companyId && claims.orgId == claims.orgId
                                                select new UserReciveFeedbackModel
                                                {
                                                    EmpFeedbakId = x.EmpFeedbackId,
                                                    CategoryId = z.CategoryId,
                                                    QuestionId = z.QuestionId,
                                                    Question = z.Question,
                                                    ProvidedMarks = z.ProvidedMarks,
                                                }).ToListAsync();

                if (userReciveFeedback.Count > 0)
                {
                    var overallAverage = 0.0;
                    var categoryist = userReciveFeedback.Select(x => x.CategoryId).Distinct().ToList();
                    var categoryCount = categoryist.Count;
                    //// Loop For Finding Category
                    foreach (var item in categoryist)
                    {
                        List<UserReportQuestions> quelist = new List<UserReportQuestions>();
                        var category = _db.FeedbackCategories.FirstOrDefault(x => x.FBCategoryId == item);
                        UserReportCategoryList obj = new UserReportCategoryList
                        {
                            CategoryId = item,
                            CategoryName = category.CategoryName,
                            OverallMarks = 0,
                            QuestionList = new List<UserReportQuestions>(),
                        };
                        var questionList = userReciveFeedback.Where(x => x.CategoryId == item).Select(x => x.QuestionId).Distinct().ToList();
                        var toalQuestion = questionList.Count;
                        var totalQuestionMarks = 0;
                        foreach (var que in questionList)
                        {
                            var questions = userReciveFeedback.Where(x => x.QuestionId == que).ToList();
                            var totalQuestion = questions.Count;
                            UserReportQuestions queobj = new UserReportQuestions
                            {
                                QuestionId = que,
                                Questions = questions.Where(x => x.QuestionId == que).Select(x => x.Question).First(),
                                Marks = questions.Where(x => x.QuestionId == que).Select(x => x.ProvidedMarks).ToList().Sum() / totalQuestion,
                            };
                            obj.QuestionList.Add(queobj);
                            totalQuestionMarks += queobj.Marks;
                        }
                        obj.OverallMarks = (float)Math.Round(decimal.Divide(totalQuestionMarks, toalQuestion), 2);
                        overallAverage += obj.OverallMarks;
                        list.Add(obj);
                    }
                    response.AverageScore = (float)Math.Round(decimal.Divide((decimal)overallAverage, categoryCount), 2);
                    response.CategoryList = list;
                }
                else
                {
                    response.AverageScore = 0;
                    response.CategoryList = list;
                }

                res.Message = "Dashboard";
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

        #endregion Api To Get User Feedback Report

        #region Api To Delete FeedbackCategory

        /// <summary>
        /// Created By Harshit Mitra on 09-05-2022
        /// API >> Put >> api/feedback/deletefbcategory
        /// </summary>
        /// <param name="fbCategoryId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletefbcategory")]
        public async Task<ResponseBodyModel> DeleteFeedbackCategory(int fbCategoryId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var feedbackCategory = await _db.FeedbackCategories.Where(x => x.FBCategoryId == fbCategoryId).FirstOrDefaultAsync();
                if (feedbackCategory != null)
                {
                    feedbackCategory.IsDeleted = true;
                    feedbackCategory.IsActive = false;
                    feedbackCategory.DeletedOn = DateTime.Now;
                    feedbackCategory.DeletedBy = claims.employeeId;
                    _db.Entry(feedbackCategory).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Category Deleted";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Category Not Found";
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

        #endregion Api To Delete FeedbackCategory

        #region Api To Get Department List On Add Feedback Category and Feedback Category List

        /// <summary>
        /// Created By Harshit Mitra on 10-05-2022
        /// API >> Get >> api/feedback/getdepartmentlistonfb
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdepartmentlistonfb")]
        public async Task<ResponseBodyModel> GetDepartmentList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await _db.Department.Where(x => x.IsDeleted == false && x.IsActive == true &&
                        x.CompanyId == claims.companyId && x.DepartmentName != "Administrator")
                        .Select(x => new GetDepartmentListHelperModel
                        {
                            DepartmentId = x.DepartmentId,
                            DepartmentName = x.DepartmentName,
                        }).ToListAsync();

                if (department.Count > 0)
                {
                    //GetDepartmentListHelperModel obj = new GetDepartmentListHelperModel
                    //{
                    //    DepartmentId = 0,
                    //    DepartmentName = "Others",
                    //};
                    //department.Add(obj);
                    res.Message = "Department list Found";
                    res.Status = true;
                    res.Data = department;
                }
                else
                {
                    res.Message = "No Department list Found";
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

        #endregion Api To Get Department List On Add Feedback Category and Feedback Category List

        #region Api To Get Department List On Add Feedback UserFeedback

        /// <summary>
        /// Created By Harshit Mitra on 10-05-2022
        /// API >> Get >> api/feedback/getdepartmentlistaddfeedback
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdepartmentlistaddfeedback")]
        public async Task<ResponseBodyModel> GetDepartmentListOnAddUserFeedback()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                var department = await _db.Department.Where(x => x.IsDeleted == false && x.IsActive == true &&
                        x.CompanyId == claims.companyId && x.DepartmentName != "Administrator" && x.DepartmentId == employee.DepartmentId)
                        .Select(x => new GetDepartmentListHelperModel
                        {
                            DepartmentId = x.DepartmentId,
                            DepartmentName = x.DepartmentName,
                        }).ToListAsync();

                if (department.Count > 0)
                {
                    //GetDepartmentListHelperModel obj = new GetDepartmentListHelperModel
                    //{
                    //    DepartmentId = 0,
                    //    DepartmentName = "Others",
                    //};
                    //department.Add(obj);
                    res.Message = "Department list Found";
                    res.Status = true;
                    res.Data = department;
                }
                else
                {
                    res.Message = "No Department list Found";
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

        #endregion Api To Get Department List On Add Feedback UserFeedback

        #region Api To Check Self Department

        /// <summary>
        /// Created By Harshit Mitra on 10-05-2022
        /// API >> Get >> api/feedback/checkselfdepartment
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("checkselfdepartment")]
        public async Task<ResponseBodyModel> CheckSelfDepartment(int departmentId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetAddFeedbackModelClass> list = new List<GetAddFeedbackModelClass>();
            EmployeeCheckResponseModel response = new EmployeeCheckResponseModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                if (employee != null)
                {
                    if (employee.DepartmentId == departmentId)
                    {
                        response.CategoryQuestionList = list;
                        response.IsDepartmentSame = true;

                        res.Message = "Employee Match";
                        res.Status = true;
                        res.Data = response;
                    }
                    else
                    {
                        var feedbackCategory = await _db.FeedbackCategories.Where(x => x.CompanyId == claims.companyId && x.OrgId == claims.orgId &&
                        x.IsDeleted == false && x.IsActive == true && x.DepartmentId == departmentId && x.DesignationId == 0).ToListAsync();
                        if (feedbackCategory.Count > 0)
                        {
                            foreach (var item in feedbackCategory)
                            {
                                GetAddFeedbackModelClass obj = new GetAddFeedbackModelClass
                                {
                                    CategoryId = item.FBCategoryId,
                                    CategoryName = item.CategoryName,
                                    QuestionList = _db.FeedbackQuestions.Where(x => x.FBCategoryId == item.FBCategoryId)
                                            .Select(x => new GetAddQuestionList
                                            {
                                                QuestionId = x.QuestionId,
                                                Question = x.Questions,
                                                FeedBackMarks = 5,
                                            }).ToList(),
                                };
                                list.Add(obj);
                            }
                        }
                        if (list.Count > 0)
                        {
                            response.CategoryQuestionList = list;
                            response.IsDepartmentSame = false;

                            res.Message = "Employee Not Match Question Found";
                            res.Status = true;
                            res.Data = response;
                        }
                        else
                        {
                            response.CategoryQuestionList = list;
                            response.IsDepartmentSame = false;

                            res.Message = "Question List Not Found";
                            res.Status = false;
                            res.Data = response;
                        }
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
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

        #endregion Api To Check Self Department

        #region This Api Used for get LineChart Feedback

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// API >> Get >> api/feedback/getLineChart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getLineChart")]
        public async Task<ResponseBodyModel> GetFeedbackLineGraph(int? year, int employeeid)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetFeedBackDashboardModell response = new GetFeedBackDashboardModell();
            List<UserDashboardFeedbackLineList> linelist = new List<UserDashboardFeedbackLineList>();
            List<InnerSeriesPartLineGraph> feedGraph = new List<InnerSeriesPartLineGraph>();
            List<int> feedbackIdList = new List<int>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                #region For Line Graph Month

                var currentYear = DateTime.Now.Year;
                year = year == null ? currentYear : year;
                var feedBackHistory = await (from x in _db.EmployeeFeedbacks
                                             join z in _db.Employee on x.FeedbackGivenTo equals z.EmployeeId
                                             where x.FeedbackGivenTo == employeeid && x.IsDeleted == false && x.IsActive == true &&
                                             x.CompanyId == claims.companyId && claims.orgId == claims.orgId && x.CreatedOn.Year == year
                                             select new
                                             {
                                                 x.EmpFeedbackId,
                                                 x.CreatedOn,
                                             }).ToListAsync();
                for (int i = 1; i <= 12; i++)
                {
                    if (year == null || year == 0)
                    {
                        feedbackIdList = feedBackHistory.Where(x => x.CreatedOn.Year == DateTime.Now.Year && x.CreatedOn.Month == i).Select(x => x.EmpFeedbackId).ToList();
                    }
                    else
                    {
                        feedbackIdList = feedBackHistory.Where(x => x.CreatedOn.Year == year && x.CreatedOn.Month == i).Select(x => x.EmpFeedbackId).ToList();
                    }
                    var questionList = _db.EmployeeFeeedBackQuestions.Where(x => feedbackIdList.Contains(x.EmpFeedbackId)).ToList();
                    var questionMarks = questionList.Sum(x => x.ProvidedMarks);
                    InnerSeriesPartLineGraph Tg = new InnerSeriesPartLineGraph
                    {
                        Name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i),
                        Value = questionList.Count > 0 ? (float)Math.Round(decimal.Divide(questionMarks, questionList.Count), 10) : 0,
                    };
                    feedGraph.Add(Tg);
                }
                List<feedbackGraph> listGraph = new List<feedbackGraph>();
                feedbackGraph ticObj = new feedbackGraph
                {
                    Name = "feedback",
                    Series = feedGraph,
                };
                listGraph.Add(ticObj);
                response.FeedbackList = listGraph;

                #endregion For Line Graph Month

                #region For Overall Average And Category List

                var userReciveFeedback = await (from x in _db.EmployeeFeedbacks
                                                join z in _db.EmployeeFeeedBackQuestions on x.EmpFeedbackId equals z.EmpFeedbackId
                                                where x.FeedbackGivenTo == employeeid && x.IsDeleted == false && x.IsActive == true &&
                                                x.CompanyId == claims.companyId && claims.orgId == claims.orgId && x.CreatedOn.Month == DateTime.Now.Month
                                                select new UserReciveFeedbackModel
                                                {
                                                    EmpFeedbakId = x.EmpFeedbackId,
                                                    CategoryId = z.CategoryId,
                                                    QuestionId = z.QuestionId,
                                                    Question = z.Question,
                                                    ProvidedMarks = z.ProvidedMarks,
                                                }).ToListAsync();

                if (userReciveFeedback.Count > 0)
                {
                    var overallAverage = 0.0;
                    var categoryist = userReciveFeedback.Select(x => x.CategoryId).Distinct().ToList();
                    var categoryCount = categoryist.Count;
                    //// Loop For Finding Category
                    foreach (var item in categoryist)
                    {
                        var category = _db.FeedbackCategories.FirstOrDefault(x => x.FBCategoryId == item);
                        UserDashboardFeedbackLineList obj = new UserDashboardFeedbackLineList
                        {
                            CategoryName = category.CategoryName,
                            OverallMarks = 0,
                        };
                        var questionList = userReciveFeedback.Where(x => x.CategoryId == item).Select(x => x.QuestionId).Distinct().ToList();
                        var toalQuestion = questionList.Count;
                        var totalQuestionMarks = 0;
                        foreach (var que in questionList)
                        {
                            var questions = userReciveFeedback.Where(x => x.QuestionId == que).ToList();
                            var totalQuestion = questions.Count;
                            UserReportQuestions queobj = new UserReportQuestions
                            {
                                QuestionId = que,
                                Questions = questions.Where(x => x.QuestionId == que).Select(x => x.Question).First(),
                                Marks = questions.Where(x => x.QuestionId == que).Select(x => x.ProvidedMarks).ToList().Sum() / totalQuestion,
                            };
                            totalQuestionMarks += queobj.Marks;
                        }
                        obj.OverallMarks = (float)Math.Round(decimal.Divide(totalQuestionMarks, toalQuestion), 2);
                        overallAverage += obj.OverallMarks;
                        linelist.Add(obj);
                    }
                    response.AverageScore = (float)Math.Round(decimal.Divide((decimal)overallAverage, categoryCount), 2);
                    response.CategoryList = linelist;
                }
                else
                {
                    response.AverageScore = 0;
                    response.CategoryList = linelist;
                }

                #endregion For Overall Average And Category List

                res.Message = "Dashboard";
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

        #endregion This Api Used for get LineChart Feedback

        #region Helper Model Class

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// </summary>
        public class EmpYaerResponse
        {
            public string Year { get; set; }
        }

        public class EmployeeCheckResponseModel
        {
            public bool IsDepartmentSame { get; set; }
            public object CategoryQuestionList { get; set; }
        }

        public class GetDepartmentListHelperModel
        {
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 07-04-2022
        /// </summary>
        public class GetCategoryTypeModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 08-04-2022
        /// </summary>
        public class AddFeedbackCategoryModel
        {
            public string CategoryName { get; set; }
            public int DepartmentId { get; set; }
            public bool ForDepartmentOnly { get; set; }
            public List<int> DesignationIds { get; set; }
            public List<FeedbackQuestions> FeedbackQuestions { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// </summary>
        public class GetAddFeedbackModelClass
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public List<GetAddQuestionList> QuestionList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// </summary>
        public class GetAddQuestionList
        {
            public int QuestionId { get; set; }
            public string Question { get; set; }
            public int FeedBackMarks { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// </summary>
        public class AddEmployeeFeedbackModel
        {
            public int EmployeeId { get; set; }
            public string GivenComment { get; set; }
            public List<GetAddFeedbackModelClass> EmployeeFeedback { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 01-04-2022
        /// </summary>
        public class GetFeedBackDashboardModel
        {
            public float AverageScore { get; set; }
            public GetFeedbackBarModel BarChart { get; set; }
            public List<UserDashboardCategoryList> CategoryList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-04-2022
        /// </summary>
        public class GetFeedbackBarModel
        {
            public List<string> Name { get; set; }
            public List<int> Value { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// </summary>
        public class GetFBDashboardCategory
        {
            public string CategoryName { get; set; }
            public List<GetAddQuestionList> QuestionList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// </summary>
        public class UserReciveFeedbackModel
        {
            public int EmpFeedbakId { get; set; }
            public int CategoryId { get; set; }
            public int QuestionId { get; set; }
            public string Question { get; set; }
            public int ProvidedMarks { get; set; }
        }

        /// <summary>
        /// Created By Harshut Mitra on 12-04-2022
        /// </summary>
        public class UserDashboardCategoryList
        {
            public string CategoryName { get; set; }
            public float OverallMarks { get; set; }
        }

        public class UserDashboardFeedbackList
        {
            public string CategoryName { get; set; }
            public float OverallMarks { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// </summary>
        public class GetFeedBackReportModel
        {
            public float AverageScore { get; set; }
            public List<UserReportCategoryList> CategoryList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// </summary>
        public class UserReportCategoryList
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public float OverallMarks { get; set; }
            public List<UserReportQuestions> QuestionList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 12-04-2022
        /// </summary>
        public class UserReportQuestions
        {
            public int QuestionId { get; set; }
            public string Questions { get; set; }
            public int Marks { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// </summary>
        public class UserDashboardFeedbackLineList
        {
            public string CategoryName { get; set; }
            public float OverallMarks { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// </summary>
        public class GetFeedBackDashboardModell
        {
            public float AverageScore { get; set; }
            public List<UserDashboardFeedbackLineList> CategoryList { get; set; }
            public List<feedbackGraph> FeedbackList { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 10-05-2022
        /// </summary>
        public class feedbackGraph
        {
            public string Name { get; set; }
            public List<InnerSeriesPartLineGraph> Series { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 06-05-2022
        /// </summary>
        public class InnerSeriesPartLineGraph
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }

        #endregion Helper Model Class
    }
}