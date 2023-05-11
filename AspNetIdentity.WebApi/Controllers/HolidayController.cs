using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/Holiday")]
    public class HolidayController : ApiController
    {
        // GET: LeaveBalanceCrud
        private ApplicationDbContext db = new ApplicationDbContext();

        public static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        [HttpGet]
        [Authorize]
        [Route("GetAllHolidays")]
        public async Task<ResponseBodyModel> GetAllHolidays()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var HolidayDataList = await db.Holidays
                    .Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted)
                    .ToListAsync();
                if (HolidayDataList.Count > 0)
                {
                    foreach (var Holiday in HolidayDataList)
                    {
                        if (Holiday.HolidayDate.Date < DateTime.Now.Date)
                            Holiday.IsActive = false;
                    }
                    res.Data = HolidayDataList.OrderBy(x => x.HolidayDate).ToList();
                    res.Message = "Holiday list found!";
                    res.Status = true;
                }
                else
                {
                    res.Data = HolidayDataList;
                    res.Message = "Holiday list not found!";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        [HttpGet]
        [Authorize]
        [Route("GetRecentHolidays")]
        public async Task<ResponseBodyModel> GetRecentHolidays()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                // Access claimvar m = indianTime.Month;

                var m = indianTime.Month;
                var y = indianTime.Year;
                var d = indianTime.Day;

                int n = 0;
                if (m <= 11)
                {
                    n = m + 1;
                }
                else
                {
                    n = 1;
                }
                var holiday = await db.Holidays.Where(x => (x.HolidayDate.Month == m || x.HolidayDate.Month == n) && x.HolidayDate.Year >= y).ToListAsync();
                var newholiday = holiday.Select(x => new
                {
                    x.HolidayId,
                    x.HolidayName,
                    x.ImageUrl,
                    x.HolidayDate,
                    x.Description,
                    Day = x.HolidayDate.ToString("dddd"),
                    Date = x.HolidayDate.ToString("dd"),
                    Month = x.HolidayDate.ToString("MMMM"),
                }).ToList();

                if (newholiday.Count > 0)
                {
                    //var day = holiday.ToString("@ddd");
                    res.Message = "Successfull";
                    res.Status = true;
                    res.Data = newholiday;
                }
                else
                {
                    res.Message = "No Attendance Available";
                    res.Status = false;
                    res.Data = newholiday;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        [Route("AddHolidays")]
        [HttpPost]
        [Authorize]
        public async Task<ResponseBodyModel> AddHolidays(Holiday HolidayBalanceData)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Holiday HolidayData = new Holiday();
                HolidayData.HolidayId = HolidayBalanceData.HolidayId;
                HolidayData.HolidayName = HolidayBalanceData.HolidayName;
                HolidayData.HolidayDate = HolidayBalanceData.HolidayDate;
                HolidayData.IsFloaterOptional = HolidayBalanceData.IsFloaterOptional;
                HolidayData.Description = HolidayBalanceData.Description;
                HolidayData.ImageUrl = HolidayBalanceData.ImageUrl;
                HolidayData.TextColor = HolidayBalanceData.TextColor;
                HolidayData.CreatedOn = DateTime.Now;
                HolidayData.UpdatedOn = DateTime.Now;
                HolidayData.CreatedBy = claims.employeeId;
                HolidayData.UpdatedBy = claims.employeeId;
                HolidayData.CompanyId = claims.companyId;
                HolidayData.IsActive = true;
                HolidayData.IsDeleted = false;
                db.Holidays.Add(HolidayData);
                await db.SaveChangesAsync();
                res.Message = "Holiday Data added";
                res.Status = true;
                res.Data = HolidayData;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false; res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        [Route("UpdateHolidays")]
        [HttpPut]
        [Authorize]

        public async Task<ResponseBodyModel> UpdateHolidays(Holiday updateHolidays)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            {
                try
                {
                    if (updateHolidays.HolidayId > 0)
                    {
                        var HolidayData = db.Holidays.Where(z => z.HolidayId == updateHolidays.HolidayId).FirstOrDefault();
                        if (HolidayData != null)
                        {
                            HolidayData.HolidayId = updateHolidays.HolidayId;
                            HolidayData.HolidayName = updateHolidays.HolidayName;
                            HolidayData.Description = updateHolidays.Description;
                            HolidayData.HolidayDate = updateHolidays.HolidayDate;
                            HolidayData.IsFloaterOptional = updateHolidays.IsFloaterOptional;
                            HolidayData.ImageUrl = updateHolidays.ImageUrl;
                            HolidayData.TextColor = updateHolidays.TextColor;
                            db.Entry(HolidayData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            res.Message = "Holiday Updated Successfully";
                            res.Status = true;
                            res.Data = HolidayData;
                        }
                        else
                        {
                            res.Message = "No Attendance Available";
                            res.Status = false;
                            res.Data = HolidayData;
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
        }

        [Route("DeleteHolidays")]
        [HttpDelete]
        [Authorize]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteLeave(int HolidayId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                Holiday HolidayData = new Holiday();
                //var GoalDelete = db.Goals.Where(x => x.GoalsID == GoalsID).FirstOrDefault();
                var HolidayDelete = db.Holidays.Where(x => x.IsDeleted == false && x.HolidayId == HolidayId).FirstOrDefault();
                if (HolidayDelete != null)
                {
                    HolidayDelete.IsDeleted = true;
                    HolidayDelete.IsActive = false;
                    db.Holidays.Remove(HolidayDelete);
                    db.SaveChanges();

                    res.Message = "Record Delete Successfully";

                    res.Status = true;
                    res.Data = HolidayDelete;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Please select the record";
                    res.Data = HolidayDelete;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        [HttpGet]
        [Authorize]
        [Route("GetHolidayByID")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetHolidayByID(int HolidayId)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<Holiday> HolidayList = new List<Holiday>();
                var employeeData = (from ad in db.Holidays
                                    where ad.HolidayId == HolidayId
                                    where ad.IsActive == true
                                    select new
                                    {
                                        ad.HolidayId,
                                        ad.HolidayName,
                                        ad.IsFloaterOptional,
                                        ad.Description,
                                    }).ToList();
                foreach (var item in employeeData)
                {
                    Holiday data = new Holiday();
                    data.HolidayId = item.HolidayId;
                    data.HolidayName = item.HolidayName;
                    data.IsFloaterOptional = item.IsFloaterOptional;
                    data.Description = item.Description;
                    HolidayList.Add(data);

                    res.Message = "GetHoliday Successfully";

                    res.Status = true;
                    res.Data = employeeData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #region Api To Update holiday Profile Image

        /// <summary>
        /// Created By Ankit On 18-05-2022
        /// API >> Post >> api/Holiday/uploadholidayimage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadholidayimage")]
        public async Task<UploadImageResponseHoliday> UploadHolidayImages()
        {
            UploadImageResponseHoliday result = new UploadImageResponseHoliday();
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
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/holidayimages/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\holidayimages\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
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

        #endregion Api To Update holiday Profile Image
    }

    public class UploadImageResponseHoliday
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string URL { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public string ExtensionType { get; set; }
    }

    public class HolidayData
    {
        public long Holidayid { get; set; }
        public string HolidayName { get; set; }
        public string Description { get; set; }
        public bool IsFloaterOptional { get; set; }
    }

    public class HolidayRes
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public List<Holiday> Holiday { get; set; }
    }
}
//#region
//for duplicate API
//public IHttpActionResult GetAllHolidays()
//{
//    try
//    {
//        var identity = User.Identity as ClaimsIdentity;
//        int userid = 0;
//        int compid = 0;
//        int orgid = 0;
//        // Access claims

//        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
//            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
//        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
//            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
//        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
//            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

//        List<Holiday> HolidayDataList = new List<Holiday>();
//        var employeeData = (from ad in db.Holiday
//                            select new
//                            {
//                                ad.HolidayId,
//                                ad.HolidayName,
//                                ad.HolidayDate,
//                                ad.Description,
//                                ad.IsFloaterOptional,
//                            }).ToList();
//        foreach (var item in employeeData)
//        {
//            Holiday data = new Holiday();
//            data.HolidayId = item.HolidayId;
//            data.HolidayName = item.HolidayName;
//            data.HolidayDate =item.HolidayDate;
//            data.Description = item.Description;
//            data.IsFloaterOptional = item.IsFloaterOptional;
//            HolidayDataList.Add(data);
//        }

//        #endregion Api To Update holiday Profile Image
//    }
//    #region Helper Class
//    public class UploadImageResponseHoliday
//    {
//        public string Message { get; set; }
//        public bool Status { get; set; }
//        public string URL { get; set; }
//        public string Path { get; set; }
//        public string Extension { get; set; }
//        public string ExtensionType { get; set; }
//    }

//    public class HolidayData
//    {
//        public long Holidayid { get; set; }
//        public string HolidayName { get; set; }
//        public string Description { get; set; }
//        public bool IsFloaterOptional { get; set; }
//    }

//    public class HolidayRes
//    {
//        public string Message { get; set; }
//        public bool Status { get; set; }
//        public List<Holiday> Holiday { get; set; }
//    }
//    #endregion

//    #region
//    //for duplicate API
//    //public IHttpActionResult GetAllHolidays()
//    //{
//    //    try
//    //    {
//    //        var identity = User.Identity as ClaimsIdentity;
//    //        int userid = 0;
//    //        int compid = 0;
//    //        int orgid = 0;
//    //        // Access claims

//    //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
//    //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
//    //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
//    //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
//    //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
//    //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

//    //        List<Holiday> HolidayDataList = new List<Holiday>();
//    //        var employeeData = (from ad in db.Holiday
//    //                            select new
//    //                            {
//    //                                ad.HolidayId,
//    //                                ad.HolidayName,
//    //                                ad.HolidayDate,
//    //                                ad.Description,
//    //                                ad.IsFloaterOptional,
//    //                            }).ToList();
//    //        foreach (var item in employeeData)
//    //        {
//    //            Holiday data = new Holiday();
//    //            data.HolidayId = item.HolidayId;
//    //            data.HolidayName = item.HolidayName;
//    //            data.HolidayDate =item.HolidayDate;
//    //            data.Description = item.Description;
//    //            data.IsFloaterOptional = item.IsFloaterOptional;
//    //            HolidayDataList.Add(data);
//    //        }

//    //        return Ok(HolidayDataList);
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        return BadRequest(ex.Message);
//    //   }
//    //}
//    #endregion
