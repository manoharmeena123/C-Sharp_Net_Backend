using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra On 03-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/sourcemaster")]
    public class SourceMasterController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api For Adding New Job Source

        /// <summary>
        /// API >> Post >> api/sourcemaster/addsource
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addsource")]
        public async Task<ResponseBodyModel> AddSource(Source model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                Source obj = new Source()
                {
                    SourceName = model.SourceName,
                    IsActive = true,
                    IsDelete = false,
                    CreateDate = DateTime.Now,
                };
                _db.Sources.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Source Added";
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

        #endregion Api For Adding New Job Source

        #region Api For Getting All Source List

        /// <summary>
        /// API >> Get >> api/sourcemaster/getallsource
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallsource")]
        public async Task<ResponseBodyModel> GetAllSourceList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var source = await _db.Sources.ToListAsync();
                if (source.Count > 0)
                {
                    res.Message = "Sources List";
                    res.Status = true;
                    res.Data = source;
                }
                else
                {
                    res.Message = "Source List Is Empty";
                    res.Status = false;
                    res.Data = source;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Source List

        #region Api For Getting All Active Source List Order By Name

        /// <summary>
        /// API >> Get >> api/sourcemaster/activesourceorder
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        [HttpGet]
        [Route("activesourceorder2")]
        public async Task<ResponseBodyModel> ActiveSourceOrderByName2()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var source = await (from s in _db.Sources
                                    where s.IsActive && !s.IsDelete
                                    select new SourceListOrderBy()
                                    {
                                        SourceId = s.SourceId,
                                        SourceName = Enum.GetName(typeof(JobHiringSourceConstants), s.SourceName),
                                    }).OrderBy(X => X.SourceName).ToListAsync();
                if (source.Count > 0)
                {
                    res.Message = "Source List";
                    res.Status = true;
                    res.Data = source;
                }
                else
                {
                    res.Message = "Source List is Empty";
                    res.Status = false;
                    res.Data = source;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Getting All Active Source List Order By Name

        #region Api To Get Blood Group

        /// <summary>
        /// Created By Harshit Mitra on 26-04-2022
        /// API >> Get >> api/sourcemaster/activesourceorder
        /// </summary>
        [HttpGet]
        [Route("activesourceorder")]
        public ResponseBodyModel ActiveSourceOrderByName()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var bloodGroup = Enum.GetValues(typeof(JobHiringSourceConstants))
                                .Cast<JobHiringSourceConstants>()
                                .Select(x => new SourceListOrderBy
                                {
                                    SourceId = (int)x,
                                    SourceName = Enum.GetName(typeof(JobHiringSourceConstants), x).Replace("_", " "),
                                }).OrderBy(X => X.SourceName == "Others").ThenBy(x => x.SourceName).ToList();

                res.Message = "Blood Group List";
                res.Status = true;
                res.Data = bloodGroup;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Blood Group

        #region Api To Get Source By Id

        /// <summary>
        /// API >> Get >> api/sourcemaster/sourcebyid
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <param name="SourceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sourcebyid")]
        public async Task<ResponseBodyModel> SourceById(int sourceId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var source = await _db.Sources.FirstOrDefaultAsync(x => x.SourceId == sourceId);
                if (source != null)
                {
                    res.Message = "Source By Id";
                    res.Status = true;
                    res.Data = source;
                }
                else
                {
                    res.Message = "Source Not Found";
                    res.Status = false;
                    res.Data = source;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Source By Id

        #region Api For Edit Source

        /// <summary>
        /// API >> Put >> api/sourcemaster/editsource
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editsource")]
        public async Task<ResponseBodyModel> EditSource(Source model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var source = await _db.Sources.FirstOrDefaultAsync(x => x.SourceId == model.SourceId);
                if (source != null)
                {
                    source.SourceName = model.SourceName;
                    source.UpdateDate = DateTime.Now;
                    _db.Entry(source).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Source Edited";
                    res.Status = true;
                    res.Data = source;
                }
                else
                {
                    res.Message = "Source Not Found";
                    res.Status = false;
                    res.Data = source;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Edit Source

        #region Api For Delete Source

        /// <summary>
        /// API >> Put >> api/sourcemaster/deletesource
        /// Created By Harshit Mitra on 04-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletesource")]
        public async Task<ResponseBodyModel> DeleteSource(int sourceId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var source = await _db.Sources.FirstOrDefaultAsync(x => x.SourceId == sourceId);
                if (source != null)
                {
                    source.IsDelete = true;
                    source.IsActive = false;
                    source.DeleteDate = DateTime.Now;
                    _db.Entry(source).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Source Deleted";
                    res.Status = true;
                    res.Data = source;
                }
                else
                {
                    res.Message = "Source Not Found";
                    res.Status = false;
                    res.Data = source;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api For Delete Source

        #region Helper Model Classes

        public class SourceListOrderBy
        {
            public int SourceId { get; set; }
            public string SourceName { get; set; }
        }

        #endregion Helper Model Classes
    }
}