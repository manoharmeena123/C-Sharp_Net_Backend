using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetIdentity.WebApi.Services
{
    public class FeedbackServices
    {
        public ApplicationDbContext _db;
        private CategoryService categoryService;

        public FeedbackServices()
        {
            _db = new ApplicationDbContext();
            categoryService = new CategoryService();
        }
        public string GetEmployeeName(int Id)
        {
            var emp = _db.Employee.Where(x => x.EmployeeId == Id).FirstOrDefault();
            if (emp != null)
            {
                return emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
            }
            return "";
        }
        public List<FeedbackMaster> GetAllfeedback()
        {
            try
            {
                return _db.Feedbacks.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Role GetRoleById(int id)
        {
            try
            {
                return _db.Role.Where(r => r.RoleId == id).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<FeedbackVM> GetAllFeedback()
        {
            var feeds = GetAllfeedback();
            List<FeedbackVM> response = new List<FeedbackVM>();
            foreach (var f in feeds)
            {
                FeedbackVM data = new FeedbackVM();
                data.FeedbackId = f.FeedbackId;
                data.UpdatedDate = f.UpdatedDate;
                data.AverageScore = f.AverageScore;
                data.ReceiverEmployeeId = f.ReceiverEmployeeId;
                data.RatedByEmpId = f.RatedByEmpId;
                data.RoleId = f.RoleId;
                // data.CategoryId = f.CategoryId;
                data.CategoryTypeId = f.CategoryTypeId;

                var empname = GetEmployeeName(f.ReceiverEmployeeId);
                if (empname != null)
                    data.EmpName = empname;
                var role = GetRoleById(f.RoleId);
                if (role != null)
                {
                    data.RoleType = role.RoleType;
                }
                var cat = categoryService.GetCategoryTypeById(f.CategoryTypeId);
                if (cat != null)
                {
                    data.CategoryType = cat.Category_Type;
                }
                var rateByEmp = GetEmployeeName(f.RatedByEmpId);
                if (rateByEmp != null)
                {
                    data.RatedBy = rateByEmp;
                }
                if (data != null)
                {
                    response.Add(data);
                }
            }
            return response;
        }

        public int GetMyAvgScore(int empid)
        {
            try
            {
                var MyFeedback = _db.Feedbacks.Where(x => x.ReceiverEmployeeId == empid).ToList();
                int Calculate = 0;
                Double CalculateAvg = 0;
                if (MyFeedback.Count > 0)
                {
                    foreach (var myFeedback in MyFeedback)
                    {
                        var MyScore = _db.FeedbackScore.Where(x => x.FeedbackId == myFeedback.FeedbackId).ToList();
                        Double innerCalculate = 0;
                        Double innerCalculateAvg = 0;
                        foreach (var myScore in MyScore)
                        {
                            innerCalculate += myScore.QuestionScore;
                        }
                        innerCalculateAvg = innerCalculate / MyScore.Count();
                        CalculateAvg += Math.Round(innerCalculateAvg);

                        var feedback = (from ad in _db.Feedbacks where ad.FeedbackId == myFeedback.FeedbackId select ad).FirstOrDefault();
                        if (feedback != null)
                        {
                            feedback.AverageScore = Convert.ToInt32(innerCalculateAvg);
                            _db.SaveChanges();
                        }
                    }
                    Calculate = Convert.ToInt32(Math.Round(CalculateAvg / MyFeedback.Count()));
                }
                else
                {
                    Calculate = 0;
                }

                return Calculate;
            }
            catch
            {
                throw;
            }
        }

        public List<CategoryAvg> GetCategoryScore(int empid)
        {
            try
            {
                List<CategoryAvg> CategoryDataList = new List<CategoryAvg>();
                var GetRoleId = _db.Employee.Where(x => x.EmployeeId == empid).FirstOrDefault();
                var GetCat = _db.Category.Where(x => x.UsertypeId == GetRoleId.RoleId).ToList();
                foreach (var item in GetCat)
                {
                    CategoryAvg Cat = new CategoryAvg();
                    var GetFeedback = _db.Feedbacks.Where(x => x.ReceiverEmployeeId == empid).ToList();
                    foreach (var item1 in GetFeedback)
                    {
                        var GetFeedbackScore = _db.FeedbackScore.Where(x => x.CategoryId == item.CategoryId && x.FeedbackId == item1.FeedbackId).ToList();
                        var GetAvg = GetFeedbackScore.Count();
                        var cc = GetFeedbackScore.Select(x => x.QuestionScore).Sum();

                        // var GetAvg = db.Questions.Select(x => x.CategoryId).
                        int FinalAvg = 0;
                        if (GetAvg != 0)
                        {
                            FinalAvg = cc / GetAvg;
                        }

                        Cat.Avg = FinalAvg;
                        Cat.CategoryId = item.CategoryId;
                        var catname = _db.Category.Where(a => a.CategoryId == item.CategoryId).FirstOrDefault();
                        Cat.CategoryName = catname.CategoryName;
                        CategoryDataList.Add(Cat);
                    }
                }

                return CategoryDataList;
            }
            catch
            {
                throw;
            }
        }

        //public ResultDTO<ParticipateDTO> GetParticipantByDate(DateTime stDate, DateTime endDate)
        //{
        //    try
        //    {
        //        var result = new ResultDTO<ParticipateDTO>();
        //        result.TotalCount = _unitOfWork.ParticipateRepository.Get(false).Where(p => p.CreatedOn >= stDate && p.CreatedOn <= endDate).Count();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<FeedbackVM> FilterFeedbackByEmpId(List<FeedbackVM> feeds, FeedbackFilter data)
        {
            List<FeedbackVM> feedFilters = new List<FeedbackVM>();
            foreach (var e in data.Employees)
            {
                var f = feeds.Where(x => x.ReceiverEmployeeId == e.EmployeeId).ToList();
                if (f != null)
                {
                    foreach (var x in f)
                    {
                        feedFilters.Add(x);
                    }
                }
            }
            return feedFilters;
        }

        public List<FeedbackVM> FilterFeedbackByRoleId(List<FeedbackVM> feeds, FeedbackFilter data)
        {
            List<FeedbackVM> feedFilters = feeds.Where(x => x.RoleId == data.RoleId).ToList();
            return feedFilters;
        }

        //  public  List<FeedbackVM> FilterFeedbackByCategoryTypeId(List<FeedbackVM> feeds, FeedbackFilter data)
        //  {
        //      List<FeedbackVM> feedFilters = feeds.Where(x => x.CategoryId == data.CategoryId).ToList();
        //      return feedFilters;
        //  }

        public List<FeedbackVM> FilterFeedbackByCategoryTypeId(List<FeedbackVM> feeds, FeedbackFilter data)
        {
            List<FeedbackVM> feedFilters = feeds.Where(x => x.CategoryTypeId == data.CategoryTypeId).ToList();
            return feedFilters;
        }

        public List<FeedbackVM> FilterFeedbackByRateByEmpId(List<FeedbackVM> feeds, FeedbackFilter data) ////
        {
            List<FeedbackVM> feedFilters = feeds.Where(x => x.RatedByEmpId == data.RatedByEmpId).ToList();
            return feedFilters;
        }

        public List<FeedbackVM> FilterFeedback(FeedbackFilter feedbackFilter)
        {
            List<FeedbackVM> feeds = GetAllFeedback();
            if (feedbackFilter.Employees != null && feedbackFilter.RoleId != 0 && feedbackFilter.CategoryTypeId != 0) ////
            {
                List<FeedbackVM> filterByEmployee = FilterFeedbackByEmpId(feeds, feedbackFilter);
                List<FeedbackVM> filterByRole = new List<FeedbackVM>();
                List<FeedbackVM> filterByCategory = new List<FeedbackVM>();
                if (filterByEmployee != null)
                {
                    filterByRole = FilterFeedbackByRoleId(filterByEmployee, feedbackFilter);
                }

                if (filterByRole != null)
                {
                    filterByCategory = FilterFeedbackByCategoryTypeId(filterByEmployee, feedbackFilter);
                }
                return filterByCategory;
            }
            else if (feedbackFilter.Employees != null && feedbackFilter.RoleId != 0)
            {
                List<FeedbackVM> filterByEmployee = FilterFeedbackByEmpId(feeds, feedbackFilter);
                List<FeedbackVM> filterByRole = new List<FeedbackVM>();
                if (filterByEmployee != null)
                {
                    filterByRole = FilterFeedbackByRoleId(filterByEmployee, feedbackFilter);
                }

                return filterByRole;
            }
            else if (feedbackFilter.Employees != null && feedbackFilter.CategoryTypeId != 0) ////CategoryId to CategoryTypeId
            {
                List<FeedbackVM> filterByEmployee = FilterFeedbackByEmpId(feeds, feedbackFilter);
                List<FeedbackVM> filterByCategory = new List<FeedbackVM>();
                if (filterByEmployee != null)
                {
                    filterByCategory = FilterFeedbackByCategoryTypeId(filterByEmployee, feedbackFilter);
                }

                return filterByCategory;
            }
            else if (feedbackFilter.RoleId != 0 && feedbackFilter.CategoryTypeId != 0) ////CategoryId to CategoryTypeId
            {
                List<FeedbackVM> filterByRole = FilterFeedbackByRoleId(feeds, feedbackFilter);
                List<FeedbackVM> filterByCategory = new List<FeedbackVM>();
                if (filterByRole != null)
                {
                    filterByCategory = FilterFeedbackByCategoryTypeId(filterByRole, feedbackFilter);
                }
                return filterByCategory;
            }
            else if (feedbackFilter.RoleId != 0)
            {
                List<FeedbackVM> filterByRole = FilterFeedbackByRoleId(feeds, feedbackFilter);

                return filterByRole;
            }
            else if (feedbackFilter.CategoryTypeId != 0)
            {
                List<FeedbackVM> filterByCategory = FilterFeedbackByCategoryTypeId(feeds, feedbackFilter);

                return filterByCategory;
            }
            else
            {
                List<FeedbackVM> filterByEmployee = FilterFeedbackByEmpId(feeds, feedbackFilter);
                return filterByEmployee;
            }
        }

        //return all feedbacks of who give feebacks
        public List<FeedbackMaster> GetFeebackByRatedByEmpId(int id)
        {
            try
            {
                return GetAllfeedback().Where(x => x.RatedByEmpId == id).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<FeedbackScore> GetAllFeedbackScore()
        {
            try
            {
                return _db.FeedbackScore.ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<FeedbackScore> GetAllFeedbackScoreByCatndFeedId(List<Category> cats, int FeedId)
        {
            try
            {
                List<FeedbackScore> feeds = GetAllFeedbackScore().Where(f => f.FeedbackId == FeedId).ToList();
                List<FeedbackScore> feedsdata = new List<FeedbackScore>();
                foreach (var x in cats)
                {
                    var f = feeds.Where(ff => ff.CategoryId == x.CategoryId).ToList();
                    feedsdata.AddRange(f);
                }
                return feedsdata;
            }
            catch
            {
                throw;
            }
        }

        public List<FeedbackDTO> GetFeebackByEmpId(int id)
        {
            List<FeedbackMaster> allFeedback = GetFeebackByRatedByEmpId(id);
            List<FeedbackDTO> feedbacks = new List<FeedbackDTO>();

            if (allFeedback.Count() != 0)
            {
                List<CategoryType> allCatType = categoryService.GetAllCategoryType();
                foreach (var x in allFeedback)
                {
                    CategoryType catType = allCatType.Where(c => c.CategoryTypeId == x.CategoryTypeId).FirstOrDefault();
                    if (catType != null)
                    {
                        FeedbackDTO feedbackdata = new FeedbackDTO();
                        feedbackdata.feedback = x;
                        feedbackdata.categoryType = catType;
                        feedbacks.Add(feedbackdata);
                        List<Category> cats = categoryService.GetAllCategories().Where(cc => cc.CategoryTypeId == catType.CategoryTypeId).ToList();
                        List<FeedbackScore> score = GetAllFeedbackScoreByCatndFeedId(cats, x.FeedbackId);
#pragma warning disable CS0219 // The variable 'xx' is assigned but its value is never used
                        var xx = 0;
#pragma warning restore CS0219 // The variable 'xx' is assigned but its value is never used
                    }
                }
            }

            return feedbacks;
        }

        public double GetAVGbyMonth(DateTime start, DateTime end)
        {
            List<FeedbackMaster> feeds = GetAllfeedback();
            int val = 0, feedCount = 0;
            foreach (var x in feeds)
            {
                feedCount++;
                if (x.CreatedDate >= start && x.CreatedDate <= end)
                {
                    val += x.AverageScore;
                }
            }
            return (double)(val / feedCount);
        }

        ///
    }
}