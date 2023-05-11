using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ExitsModel;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Exits
{
    /// <summary>
    /// Created By Harshit Mitra On 08-08-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/exitsettings")]
    public class ExitsSettingController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region API For Get Type

        /// <summary>
        /// Create By Ravi Vyas 08-08-2022
        /// api/exitsettings/gettype
        /// </summary>
        /// <returns></returns>

        [Route("getnoticeperiodtype")]
        [HttpGet]
        public ResponseBodyModel GetNoticeType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var noticeType = Enum.GetValues(typeof(NoticePeriodDurationConstants))
                              .Cast<NoticePeriodDurationConstants>()
                              .Select(x => new
                              {
                                  Id = (int)x,
                                  Type = Enum.GetName(typeof(NoticePeriodDurationConstants), x).Replace("_", " "),
                              }).ToList();

                res.Message = "Notification Type";
                res.Status = true;
                res.Data = noticeType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API For Get Type

        #region API To Add Notice Period Settings

        /// <summary>
        /// Create By Ravi Vyas 08-08-2022
        /// api/exitsettings/addexitsetting
        /// </summary>
        /// <returns></returns>
        [Route("addnoticeperiod")]
        [HttpPost]
        public async Task<ResponseBodyModel> AddNoticePeriod(NoticePeriodSetting model)
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
                    var checkAny = await _db.NoticePeriods.AnyAsync(x => x.CompanyId == claims.companyId);
                    NoticePeriodSetting obj = new NoticePeriodSetting()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Duration = model.Duration,
                        Type = model.Type,
                        IsDefault = !checkAny,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = claims.companyId,
                        OrgId = 0,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.NoticePeriods.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Notice Period Add";
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

        #endregion API To Add Notice Period Settings

        #region API To Get All Notice Period

        /// <summary>
        /// Create By Ravi Vyas 08-08-2022
        /// Modify By Harshit Mitra on 08-08-2022
        /// API >> Get >> api/exitsettings/getallnoticeperiod
        /// </summary>
        /// <returns></returns>

        [Route("getallnoticeperiod")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllNotice(int? page = null, int? count = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var noticePeriod = await _db.NoticePeriods.Where(x => x.IsActive &&
                        !x.IsDeleted && x.CompanyId == claims.companyId).ToListAsync();
                if (noticePeriod.Count > 0)
                {
                    noticePeriod.ForEach(x =>
                    {
                        x.CreatedByName = _db.GetEmployeeNameById(x.CreatedBy);
                        x.UpdatedByName = x.UpdatedBy.HasValue ? _db.GetEmployeeNameById((int)x.UpdatedBy) : null;
                        x.DeletedByName = x.DeletedBy.HasValue ? _db.GetEmployeeNameById((int)x.DeletedBy) : null;
                        x.TypeName = Enum.GetName(typeof(NoticePeriodDurationConstants), x.Type);
                    });
                    res.Message = "Get NoticePeriods ! ";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = noticePeriod.Count,
                            Counts = (int)count,
                            List = noticePeriod.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = noticePeriod;
                    }
                }
                else
                {
                    res.Message = "Notice Periods";
                    res.Status = false;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = noticePeriod.Count,
                            Counts = (int)count,
                            List = noticePeriod,
                        };
                    }
                    else
                    {
                        res.Data = noticePeriod;
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

        #endregion API To Get All Notice Period

        #region API To Get Notice Period By Id

        /// <summary>
        /// Created By Harshit Mitra on 08-08-2022
        /// API >> Get >> api/exitsettings/getnoticeperiodbyid
        /// </summary>
        /// <returns></returns>
        [Route("getnoticeperiodbyid")]
        [HttpGet]
        public async Task<ResponseBodyModel> GetAllNotice(int noticePeriodId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var noticePeriod = await _db.NoticePeriods.Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == claims.companyId && x.NoticePeriodId == noticePeriodId).FirstOrDefaultAsync();
                if (noticePeriod != null)
                {
                    noticePeriod.CreatedByName = _db.GetEmployeeNameById(noticePeriod.CreatedBy);
                    noticePeriod.UpdatedByName = noticePeriod.UpdatedBy.HasValue ? _db.GetEmployeeNameById((int)noticePeriod.UpdatedBy) : null;
                    noticePeriod.DeletedByName = noticePeriod.DeletedBy.HasValue ? _db.GetEmployeeNameById((int)noticePeriod.DeletedBy) : null;
                    res.Message = "Notice Period";
                    res.Status = true;
                    res.Data = noticePeriod;
                }
                else
                {
                    res.Message = "Notice Period";
                    res.Status = false;
                    res.Data = noticePeriod;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Notice Period By Id

        #region API To Edit Notice Period

        /// <summary>
        /// Created By Harshit Mitra On 08-08-2022
        /// API >> Put >> api/exitsettings/editnoticeperiod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editnoticeperiod")]
        public async Task<ResponseBodyModel> EditNoticePeriod(NoticePeriodSetting model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var noticePeriod = await _db.NoticePeriods.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                            x.NoticePeriodId == model.NoticePeriodId && x.CompanyId == claims.companyId);
                if (noticePeriod != null)
                {
                    noticePeriod.Name = model.Name;
                    noticePeriod.Description = model.Description;
                    noticePeriod.Type = model.Type;
                    noticePeriod.Duration = model.Duration;
                    noticePeriod.UpdatedBy = claims.employeeId;
                    noticePeriod.UpdatedOn = DateTime.Now;

                    _db.Entry(noticePeriod).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Notice Period Edited";
                    res.Status = true;
                    res.Data = noticePeriod;
                }
                else
                {
                    res.Message = "Notice Period Not Found";
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

        #endregion API To Edit Notice Period

        #region API To Delete Notice Period

        /// <summary>
        /// Created By Harshit Mitra On 09-08-2022
        /// API >> Put >> api/exitsettings/deletenoticeperiod
        /// </summary>
        /// <param name="noticePeriodId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletenoticeperiod")]
        public async Task<ResponseBodyModel> DeleteNoticePeriod(int noticePeriodId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var noticePeriod = await _db.NoticePeriods.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                            x.NoticePeriodId == noticePeriodId && x.CompanyId == claims.companyId);
                if (noticePeriod != null)
                {
                    noticePeriod.IsActive = false;
                    noticePeriod.IsDeleted = true;
                    noticePeriod.DeletedBy = claims.employeeId;
                    noticePeriod.DeletedOn = DateTime.Now;

                    _db.Entry(noticePeriod).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Notice Period Deleted";
                    res.Status = true;
                    res.Data = noticePeriod;
                }
                else
                {
                    res.Message = "Notice Period Not Found";
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

        #endregion API To Delete Notice Period

        #region API To Change Default Notice Period

        /// <summary>
        /// Created By Harshit Mitra On 08-08-2022
        /// API >> Put >> api/exitsettings/changedefault
        /// </summary>
        /// <param name="noticePeriodId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("changedefault")]
        public async Task<ResponseBodyModel> ChangeDefaultNoticePeriod(int noticePeriodId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var noticePeriods = await _db.NoticePeriods.Where(x => x.CompanyId == claims.companyId && x.IsActive &&
                            !x.IsDeleted && (x.IsDefault || x.NoticePeriodId == noticePeriodId)).ToListAsync();
                if (noticePeriods.Count != 0)
                {
                    foreach (var item in noticePeriods)
                    {
                        item.UpdatedBy = claims.employeeId;
                        item.UpdatedOn = DateTime.Now;
                        item.IsDefault = item.NoticePeriodId == noticePeriodId;

                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Default Setting Changed";
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

        #endregion API To Change Default Notice Period
    }
}