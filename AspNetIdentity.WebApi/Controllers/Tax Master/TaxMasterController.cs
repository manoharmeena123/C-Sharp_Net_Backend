using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Tax_Master;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Tax_Master
{
    [Authorize]
    [RoutePrefix("api/taxable")]
    public class TaxMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region This Api Use for Add Tax

        /// <summary>
        /// created by Mayank Prajapati on 16/8/2022
        /// Api >> Post >> api/taxable/posttax
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("posttax")]
        public async Task<ResponseBodyModel> PostTax(IncomeSlab model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Unvalid";
                    res.Status = false;
                }
                else
                {
                    IncomeSlab post = new IncomeSlab
                    {
                        Name = model.Name,
                        Details = model.Details,
                        From = model.From,
                        To = model.To,
                        Texpercent = model.Texpercent,
                        CreatedBy = claims.employeeId,
                        CompanyId = claims.companyId,
                        OrgId = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _db.IncomeSlabs.Add(post);
                    await _db.SaveChangesAsync();
                    res.Message = "Tax Data Add";
                    res.Status = true;
                    res.Data = post;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }

            return res;
        }

        #endregion This Api Use for Add Tax

        #region Get Tax

        /// <summary>
        /// created by Mayank Prajapati on 16/8/2022
        /// Api >> Post >> api/taxable/gettax
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettax")]
        public async Task<ResponseBodyModel> GetTax(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var TaxData = await _db.IncomeSlabs.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToListAsync();
                if (TaxData.Count > 0)
                {
                    res.Message = "Get Data";
                    res.Status = true;
                    res.Data = TaxData;

                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = TaxData.Count,
                            Counts = (int)count,
                            List = TaxData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                        res.Data = TaxData;
                }
                else
                {
                    res.Message = "Tax Data Not Found";
                    res.Status = false;
                    res.Data = TaxData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Tax

        #region Get Tax By Id

        [HttpGet]
        [Route("gettaxbyid")]
        public async Task<ResponseBodyModel> GetTaxById(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await _db.IncomeSlabs.Where(x => x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId).
                Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToListAsync();
                if (data != null)
                {
                    res.Message = "Tax Data Found";
                    res.Status = true;
                    res.Data = data;
                }
                else
                {
                    res.Message = "Tax Data Not Found";
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

        #endregion Get Tax By Id

        #region UpDate Tax Data

        [HttpPut]
        [Route("taxput")]
        public async Task<ResponseBodyModel> EditTaxData(IncomeSlab model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Add = await _db.IncomeSlabs.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (Add != null)
                {
                    Add.Name = model.Name;
                    Add.Details = model.Details;
                    Add.From = model.From;
                    Add.To = model.To;
                    Add.Texpercent = model.Texpercent;
                    Add.UpdatedBy = claims.employeeId;
                    Add.CompanyId = claims.companyId;
                    Add.UpdatedOn = DateTime.Now;

                    _db.Entry(Add).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Post UpDated";
                    res.Status = true;
                    res.Data = Add;
                }
                else
                {
                    res.Message = " Failed To Update";
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

        #endregion UpDate Tax Data

        #region Delete Tax Data

        [HttpDelete]
        [Route("deletetax")]
        public async Task<ResponseBodyModel> DeletTaxData(int Id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await _db.IncomeSlabs.FirstOrDefaultAsync(x =>
                    x.Id == Id && x.IsDeleted == false && x.IsActive == true);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;

                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "TaxData Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "NoPost Data Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Tax Data

        #region Proffesinal Tax APIs

        #region API To Add Proffesinal Tax

        /// <summary>
        /// Created By Harshit Mitra On 02-09-2022
        /// API >> Post >> api/taxable/addpt
        /// </summary>
        [HttpPost]
        [Route("addpt")]
        public async Task<ResponseBodyModel> AddProfessionalTax(ProfessionalTax model)
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
                    if (claims.companyId == 0 && claims.orgId == 0)
                    {
                        ProfessionalTax obj = new ProfessionalTax
                        {
                            CountryId = model.CountryId,
                            StateId = model.StateId,
                            From = model.From,
                            To = model.To,
                            TaxAmount = model.TaxAmount,
                            EndDate = model.EndDate,
                            IsLastMonthDiffrent = model.IsLastMonthDiffrent,
                            LastMonthId = model.LastMonthId,
                            LastMonthTax = model.LastMonthTax,
                            RangeStatus = model.RangeStatus,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = model.CreatedOn,
                        };
                        _db.ProfessionalTaxes.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Professional Tax Added";
                        res.Status = false;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "You are Not Able To Add Profesional Tax";
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

        #endregion API To Add Proffesinal Tax

        #endregion Proffesinal Tax APIs

        #region Professional Tax Group

        #region API To Add Proffesional Taxs

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Post >> api/taxable/addptgroup
        /// </summary>
        [HttpPost]
        [Route("addptgroup")]
        public async Task<ResponseBodyModel> AddPTTaxGroup(ProfessionalTaxGroup model)
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
                    var checkDate = await _db.PTGroup.Where(x => x.CountryId ==
                            model.CountryId && !x.IsDeleted && x.IsActive).ToListAsync();
                    if (checkDate.Count > 0)
                    {
                        var check = checkDate.OrderByDescending(x => x.StartDate).FirstOrDefault();
                        if (check.EndDate.HasValue)
                        {
                            if (((DateTime)check.EndDate).Date < model.StartDate)
                            {
                                ProfessionalTaxGroup obj = new ProfessionalTaxGroup
                                {
                                    GroupTitle = model.GroupTitle,
                                    Discription = model.Discription,
                                    CountryId = model.CountryId,
                                    StartDate = model.StartDate,
                                    EndDate = model.EndDate,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedOn = DateTime.Now,
                                };
                                _db.PTGroup.Add(obj);
                                await _db.SaveChangesAsync();

                                res.Message = "Professional Tax Added";
                                res.Status = true;
                                res.Data = obj;
                            }
                            else
                            {
                                res.Message = "You Have To Select Start Date After End Date";
                                res.Status = false;
                            }
                        }
                        else
                        {
                            res.Message = "Last Proffesional Tax Group Not Ended";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        ProfessionalTaxGroup obj = new ProfessionalTaxGroup
                        {
                            GroupTitle = model.GroupTitle,
                            Discription = model.Discription,
                            CountryId = model.CountryId,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                        };
                        _db.PTGroup.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Professional Tax Added";
                        res.Status = true;
                        res.Data = obj;
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

        #endregion API To Add Proffesional Taxs

        #region API To Get Proffesional Taxs

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Get >> api/taxable/getptgroup
        /// </summary>
        [HttpGet]
        [Route("getptgroup")]
        public async Task<ResponseBodyModel> GetPTTaxGroup()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ptGroups = await (from g in _db.PTGroup
                                      join c in _db.Country on g.CountryId equals c.CountryId
                                      where g.IsActive && !g.IsDeleted && c.IsActive && !c.IsDeleted
                                      select new
                                      {
                                          g.PTGroupId,
                                          g.GroupTitle,
                                          g.Discription,
                                          g.StartDate,
                                          g.EndDate,
                                          c.CountryName,
                                          StatesInGroup = _db.PTState.Count(y => y.IsActive &&
                                                !y.IsDeleted && y.PTGroupId == g.PTGroupId),
                                      }).ToListAsync();
                if (ptGroups.Count > 0)
                {
                    res.Message = "Professional Groups";
                    res.Status = true;
                    res.Data = ptGroups;
                }
                else
                {
                    res.Message = "No Professional Group Found";
                    res.Status = false;
                    res.Data = ptGroups;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Proffesional Taxs

        #region API To Get Country Name By PT Group Id

        /// <summary>
        /// Created By Harshit Mitra On 06-09-2022
        /// API >> Get >> api/taxable/getcountrydatabyptgorupid
        /// </summary>
        /// <param name="ptGroupId"></param>
        [HttpGet]
        [Route("getcountrydatabyptgorupid")]
        public async Task<ResponseBodyModel> GetCountryNameByPTGroupId(int ptGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var countryNameData = await (from g in _db.PTGroup
                                             join c in _db.Country on g.CountryId equals c.CountryId
                                             where g.PTGroupId == ptGroupId
                                             select new
                                             {
                                                 g.PTGroupId,
                                                 g.GroupTitle,
                                                 c.CountryId,
                                                 c.CountryName,
                                             }).FirstOrDefaultAsync();
                if (countryNameData != null)
                {
                    res.Message = "Country Data";
                    res.Status = true;
                    res.Data = countryNameData;
                }
                else
                {
                    res.Message = "Country Data Not Found";
                    res.Status = false;
                    res.Data = countryNameData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Country Name By PT Group Id

        #endregion Professional Tax Group

        #region Professional Tax State

        #region API To Add Proffesional Taxs State

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Post >> api/taxable/addptstate
        /// </summary>
        [HttpPost]
        [Route("addptstate")]
        public async Task<ResponseBodyModel> AddPTTaxState(ProfessionalTaxState model)
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
                    var checkState = await _db.PTState.FirstOrDefaultAsync(x => x.IsActive &&
                            !x.IsDeleted && x.StateId == model.StateId && x.PTStateId == model.PTStateId);
                    if (checkState == null)
                    {
                        ProfessionalTaxState obj = new ProfessionalTaxState
                        {
                            PTGroupId = model.PTGroupId,
                            StateId = model.StateId,
                            Title = model.Title,
                            Discription = model.Discription,
                            Duration = model.Duration,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                        };
                        _db.PTState.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Professional Tax State Added";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        res.Message = "This State Is Already Added IN Professional Tax Group";
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

        #endregion API To Add Proffesional Taxs State

        #region API To Get Proffesional Taxs States

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Post >> api/taxable/getptstate
        /// </summary>
        [HttpGet]
        [Route("getptstate")]
        public async Task<ResponseBodyModel> GetPTTaxState()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ptGroups = await (from g in _db.PTGroup
                                      join c in _db.Country on g.CountryId equals c.CountryId
                                      join s in _db.PTState on g.PTGroupId equals s.PTGroupId
                                      join st in _db.State on s.StateId equals st.StateId
                                      //join r in _db.PTRange on s.PTStateId equals r.PTStateId
                                      where g.IsActive && !g.IsDeleted && s.IsActive &&
                                      !s.IsDeleted /*&& r.IsActive && !r.IsDeleted*/
                                      select new
                                      {
                                          g.PTGroupId,
                                          s.PTStateId,
                                          g.GroupTitle,
                                          c.CountryName,
                                          s.Title,
                                          s.Discription,
                                          st.StateName,
                                          Duration = s.Duration.ToString(),
                                          RangeCase = _db.PTRange.Count(y => y.IsActive &&
                                                !y.IsDeleted && y.PTStateId == s.PTStateId),
                                      }).ToListAsync();
                if (ptGroups.Count > 0)
                {
                    res.Message = "Professional Groups";
                    res.Status = true;
                    res.Data = ptGroups;
                }
                else
                {
                    res.Message = "No Professional State Found";
                    res.Status = false;
                    res.Data = ptGroups;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Proffesional Taxs States

        #region API To Get State List By PT Group Id

        /// <summary>
        /// Created By Harshit Mitra On 06-09-2022
        /// API >> Get >> api/taxable/getstatelistbyptgroupid
        /// </summary>
        /// <param name="ptGroupId"></param>
        [HttpGet]
        [Route("getstatelistbyptgroupid")]
        public async Task<ResponseBodyModel> GetStateListByPTGroupId(int ptGroupId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stateNameData = await (from s in _db.PTState
                                           join st in _db.State on s.StateId equals st.StateId
                                           where s.IsActive && !s.IsDeleted && s.IsActive &&
                                           !s.IsDeleted /*&& r.IsActive && !r.IsDeleted*/
                                           select new
                                           {
                                               s.PTStateId,
                                               s.Title,
                                               st.StateName,
                                           }).ToListAsync();
                if (stateNameData.Count > 0)
                {
                    res.Message = "State List";
                    res.Status = true;
                    res.Data = stateNameData;
                }
                else
                {
                    res.Message = "State List Not Found";
                    res.Status = false;
                    res.Data = stateNameData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get State List By PT Group Id

        #region API To Get State List By PT Group Id

        /// <summary>
        /// Created By Harshit Mitra On 06-09-2022
        /// API >> Get >> api/taxable/getstatedurationbyptstateid
        /// </summary>
        /// <param name="ptStateId"></param>
        [HttpGet]
        [Route("getstatedurationbyptstateid")]
        public async Task<ResponseBodyModel> GetStateDurationByPTStateId(int ptStateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stateNameData = await (from s in _db.PTState
                                           join st in _db.State on s.StateId equals st.StateId
                                           where s.IsActive && !s.IsDeleted && s.IsActive &&
                                           !s.IsDeleted && s.PTStateId == ptStateId
                                           select new
                                           {
                                               s.PTStateId,
                                               s.Title,
                                               st.StateName,
                                               DurationId = s.Duration,
                                               Duration = s.Duration.ToString(),
                                           }).FirstOrDefaultAsync();
                if (stateNameData != null)
                {
                    res.Message = "State Duration";
                    res.Status = true;
                    res.Data = stateNameData;
                }
                else
                {
                    res.Message = "State Duration Not Found";
                    res.Status = false;
                    res.Data = stateNameData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get State List By PT Group Id

        #region API To Get State Name Duration By PT State Id

        [HttpGet]
        [Route("getstatenamebyptorupid")]
        public async Task<ResponseBodyModel> GetStateNameByPTStateId(int ptStateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var stateNameData = await (from g in _db.PTGroup
                                           join c in _db.Country on g.CountryId equals c.CountryId
                                           join s in _db.PTState on g.PTGroupId equals s.PTGroupId
                                           join st in _db.State on s.StateId equals st.StateId
                                           //join r in _db.PTRange on s.PTStateId equals r.PTStateId
                                           where g.IsActive && !g.IsDeleted && s.IsActive &&
                                           !s.IsDeleted /*&& r.IsActive && !r.IsDeleted*/
                                           select new
                                           {
                                               g.PTGroupId,
                                               s.PTStateId,
                                               g.GroupTitle,
                                               c.CountryName,
                                               s.Title,
                                               s.Discription,
                                               st.StateName,
                                               RangeCase = _db.PTRange.Count(y => y.IsActive &&
                                                     !y.IsDeleted && y.PTStateId == s.PTStateId),
                                           }).ToListAsync();
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get State Name Duration By PT State Id

        #endregion Professional Tax State

        #region Professional Tax Range

        #region API To Get Proffesinal Tax Range Enum

        /// <summary>
        /// Created By Harshit Mitra On 02-09-2022
        /// API >> Get >> api/taxable/getptrangeenum
        /// </summary>
        [HttpGet]
        [Route("getptrangeenum")]
        public ResponseBodyModel GetProfessionaTaxRange()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var ptRange = Enum.GetValues(typeof(PTRangeConstants))
                            .Cast<PTRangeConstants>()
                            .Select(x => new
                            {
                                TypeId = (int)x,
                                TypeName = Enum.GetName(typeof(PTRangeConstants), x).Replace("_", " - "),
                                SingleValue = x != PTRangeConstants.Range_To_Range,
                            }).ToList();

                res.Message = "PT Range";
                res.Status = true;
                res.Data = ptRange;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Proffesinal Tax Range

        #region API To Add Proffesional Taxs Range

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Post >> api/taxable/addptrange
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addptrange")]
        public async Task<ResponseBodyModel> AddPTTaxRange(ProfessionalTaxRange model)
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
                    ProfessionalTaxRange obj = new ProfessionalTaxRange
                    {
                        PTGroupId = model.PTGroupId,
                        PTStateId = model.PTStateId,
                        From = model.From,
                        To = model.To,
                        TaxAmount = model.TaxAmount,
                        IsLastMonthDiffrent = model.IsLastMonthDiffrent,
                        LastMonthId = model.LastMonthId,
                        LastMonthTax = model.LastMonthTax,
                        LastMonthName = model.LastMonthId == 0 ? null :
                                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.LastMonthId),
                        RangeStatus = model.RangeStatus,
                        IsDiffrentForFemale = model.IsDiffrentForFemale,
                        GenderAmount = model.GenderAmount,

                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now,
                    };
                    _db.PTRange.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Professional Tax Range Added";
                    res.Status = true;
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

        #endregion API To Add Proffesional Taxs Range

        #region API To Get Month List On PT Range

        /// <summary>
        /// Created By Harshit Mitra On 05-09-2022
        /// API >> Post >> api/taxable/monthlistonpt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("monthlistonpt")]
        public ResponseBodyModel GetMonthListOn()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            MonthId = i + 1,
                                            MonthName = m
                                        }).ToList();
                res.Message = "Month List";
                res.Status = true;
                res.Data = months;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Month List On PT Range

        #region API To Get Professional Range
        /// <summary>
        /// Created By Harshit Mitra on 08-09-2022
        /// API >> Get >> api/taxable/getptrange
        /// </summary>
        [HttpGet]
        [Route("getptrange")]
        public async Task<ResponseBodyModel> GetPTRange()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var ptRange = await (from g in _db.PTGroup
                                     join c in _db.Country on g.CountryId equals c.CountryId
                                     join s in _db.PTState on g.PTGroupId equals s.PTGroupId
                                     join st in _db.State on s.StateId equals st.StateId
                                     join r in _db.PTRange on s.PTStateId equals r.PTStateId
                                     where g.IsActive && !g.IsDeleted && s.IsActive &&
                                     !s.IsDeleted && r.IsActive && !r.IsDeleted
                                     select new
                                     {
                                         g.PTGroupId,
                                         s.PTStateId,
                                         g.GroupTitle,
                                         c.CountryName,
                                         Duration = s.Duration.ToString(),
                                         st.StateName,
                                         r.PTRangeId,
                                         r.From,
                                         r.To,
                                         RangeStatus = r.RangeStatus.ToString().Replace("_", " - "),
                                         Range = r.RangeStatus == PTRangeConstants.Range_To_Range ? r.From + " to " + r.To
                                            : r.RangeStatus == PTRangeConstants.Zero_To_Range ? "Below " + r.To : "Above " + r.From,
                                         Detail = r.IsLastMonthDiffrent ? (r.IsDiffrentForFemale ?
                                            r.TaxAmount + " for 11 month and " + r.LastMonthTax + " for " +
                                            r.LastMonthName +
                                            " and " + r.GenderAmount + " for Female"
                                            :
                                            r.TaxAmount + " for 11 month and " + r.LastMonthTax + " for " +
                                            r.LastMonthName)
                                            :
                                            r.TaxAmount.ToString(),
                                     }).ToListAsync();
                if (ptRange.Count > 0)
                {
                    res.Message = "PT Range";
                    res.Status = true;
                    res.Data = ptRange;
                }
                else
                {
                    res.Message = "PT Range Not Found";
                    res.Status = false;
                    res.Data = ptRange;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class MonthHelperClass
        {
            public int MonthId { get; set; }
            public string MonthName { get; set; }
        }
        #endregion

        #endregion Profes   sional Tax Range
    }
}