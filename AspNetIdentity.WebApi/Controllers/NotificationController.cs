using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra On 19-08-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/notification")]
    public class NotificationController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region API To Add And Update
        /// <summary>
        /// Created By Harshit Mitra On 19-08-2022
        /// API >> Post >> api/notification/checkfcm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkfcm")]
        public async Task<bool> CheckFCMAsync(FireBase model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var firebase = await _db.FireBases.FirstOrDefaultAsync(x =>
                x.EmployeeId == claims.employeeId && x.CompanyId == claims.companyId);
            if (firebase != null)
            {
                firebase.EmployeeId = claims.employeeId;
                firebase.CompanyId = claims.companyId;
                firebase.IMEI = model.IMEI;
                firebase.AndroidVersion = model.AndroidVersion;
                firebase.SIMOperator = model.SIMOperator;
                firebase.SIMNumber = model.SIMNumber;
                firebase.SIMState = model.SIMState;
                firebase.SIMCountry = model.SIMCountry;
                firebase.FCMToken = model.FCMToken;
                firebase.Brand = model.Brand;
                firebase.PhoneModel = model.PhoneModel;
                firebase.Manufacture = model.Manufacture;
                firebase.PCFCMToken = model.PCFCMToken;

                _db.Entry(firebase).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();
                _db.Dispose();
            }
            else
            {
                FireBase obj = new FireBase
                {
                    EmployeeId = claims.employeeId,
                    CompanyId = claims.companyId,
                    IMEI = model.IMEI,
                    AndroidVersion = model.AndroidVersion,
                    SIMOperator = model.SIMOperator,
                    SIMNumber = model.SIMNumber,
                    SIMState = model.SIMState,
                    SIMCountry = model.SIMCountry,
                    FCMToken = model.FCMToken,
                    Brand = model.Brand,
                    PhoneModel = model.PhoneModel,
                    Manufacture = model.Manufacture,
                    PCFCMToken = model.PCFCMToken,
                };
                _db.FireBases.Add(obj);
                await _db.SaveChangesAsync();
                _db.Dispose();
            }
            return true;
        }
        #endregion

        #region This Api Use To Get Notification Data
        /// <summary>
        /// Created By Ankit Date-19/08/2022
        /// Api >> Get >> api/notification/getbyid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyid")]
        public async Task<ResponseBodyModel> GetById(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notificationdata = await _db.NotificationDb.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == claims.companyId);
                if (notificationdata != null)
                {
                    notificationdata.IsNew = false;
                    _db.Entry(notificationdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Get Notificaton Data";
                    res.Status = true;
                    res.Data = notificationdata;
                }
                else
                {
                    res.Message = "No Notificaton Data";
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

        #region This Api Use To Get Notification All Data
        /// <summary>
        /// Created By Ankit Date-19/08/2022
        /// Api >> Get >> api/notification/getalldata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalldata")]
        public async Task<ResponseBodyModel> GetAllData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notificationdata = await _db.NotificationDb.Where(x => x.CompanyId == claims.companyId &&
                        x.EmployeeId == claims.employeeId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
                if (notificationdata.Count > 0)
                {
                    res.Message = "Get Notificaton Data";
                    res.Status = true;
                    res.Data = notificationdata.OrderByDescending(x => x.Id);
                }
                else
                {
                    res.Message = "No Notificaton Data";
                    res.Status = false;
                    res.Data = notificationdata;
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


        #region This Api Use To Remove Notification Data
        /// <summary>
        /// Created By Ankit Date-19/08/2022
        /// Api >> Delete >> api/notification/removedata
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("removedata")]
        public async Task<ResponseBodyModel> RemoveDate(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notificationdata = await _db.NotificationDb.Where(x => x.CompanyId == claims.companyId && x.Id == id).FirstOrDefaultAsync();
                if (notificationdata != null)
                {
                    notificationdata.IsDeleted = true;
                    notificationdata.IsActive = false;
                    notificationdata.DeleteDate = DateTime.Now;

                    _db.Entry(notificationdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Message = "Remove Notificaton Data";
                    res.Status = true;
                    res.Data = notificationdata;
                }
                else
                {
                    res.Message = "Not Notificaton Data";
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



        #region This Api Use to Add Notification Data
        /// <summary>
        /// Created By ankit On 19-08-2022
        /// API >> Post >> api/notification/addnotificationdata
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addnotificationdata")]
        public async Task<bool> AddNotification(Notification model)
        {
            bool response = false;
            try
            {
                Notification obj = new Notification
                {
                    EmployeeId = model.EmployeeId,
                    CompanyId = model.CompanyId,
                    Title = model.Title,
                    Message = model.Message,
                    CreateDate = DateTime.Now,
                    IsNew = true,
                    IsActive = true,
                    IsDeleted = false,
                };
                _db.NotificationDb.Add(obj);
                await _db.SaveChangesAsync();
                var FcmData = _db.FireBases.FirstOrDefault(f =>
                    f.CompanyId == model.CompanyId && f.EmployeeId == model.EmployeeId);

                if (FcmData != null)
                {
                    if (model.ForPC)
                        response = SendPcNotification(obj.Title, obj.Message, FcmData.PCFCMToken);
                }
                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Firebase Notification
        public bool SendPcNotification(string title, string message, string PCFCMToken)
        {
            bool st;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                string _title = title;
                string _msg = message;
                string _fcmToken = PCFCMToken;
                //object _data = "http://hrms.moreyeahs.in/assets/images/More1.jpg";
                object _data = "";
                // Vendor
                //string _serverKey = "AAAA9FpQCaE:APA91bGS4o7OyzptToC4JDZbxXifd3b5DYs19UfzDLcRWWrGUmN6Zk2W6PKGCPITbZLplz1vkpdXM6RjljZFt7uCHUfcd_g3E04fB7S_StTek10Yvs6noCv1scsqFBFq_5DCqYUukL6h";
                //string _senderId = "1049487215009";
                // Client
                string _serverKey = "AAAA2iMCt_U:APA91bEfI8K05SqFRsTDygWB0QSnd7n_q8MfCVGXuwMqUIvXjNNJQOTV-GzW0VfDRtuCtNMnJh37X58VqWXur1Tav_BCu5rSH4r5Y-QTJbwPwi78MsS7L69W7CqezdkXJlyeL9n6CWoD";
                string _senderId = "936890251253";
                var sts = ExcutePushNotification(_title, _msg, _fcmToken, _data, _serverKey, _senderId);
                st = true;
            }
            catch (Exception ex)
            {
                st = false;
                throw;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return st;
        }
        #endregion

        #region Excute Push Notifiction
        /// <summary>
        /// Excute Push Notification
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="fcmToken"></param>
        /// <param name="data"></param>
        /// <param name="serverKey"></param>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public string ExcutePushNotification(string title, string msg, string fcmToken, object data, string serverKey, string senderId)
        {
            var result = "-1";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                httpWebRequest.Method = "POST";
                var payload = new
                {
                    notification = new
                    {
                        title = title,
                        body = msg,
                        sound = "default",
                        image = data,
                        //image = "http://103.15.67.124:8081/" + data
                    },
                    data = new
                    {
                        title = title,
                        body = msg,
                        sound = "default",
                        image = data,
                        //info = data
                        LinkUrl = "https://www.google.com/",
                    },
                    //webpush = new
                    //{
                    //    fcm_options = new
                    //    {
                    //        link = "https://www.google.com/",
                    //    },
                    //},
                    to = fcmToken,
                    priority = "high",
                    content_available = true,
                };
                var serializer = new JavaScriptSerializer();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = serializer.Serialize(payload);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {

                result = "failed exp : " + ex;
                return result;
            }
        }
        #endregion

        #region This Api Use To get Notification Count
        /// <summary>
        /// Created By Shagun Date-20/08/2022
        /// Api >> Get >> api/notification/getcount
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcount")]
        public async Task<ResponseBodyModel> GetCount()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notificationdata = await _db.NotificationDb.Where(x => x.CompanyId == claims.companyId &&
                    x.EmployeeId == claims.employeeId && x.IsActive && !x.IsDeleted && x.IsNew).ToListAsync();

                res.Message = "Get Notification Count";
                res.Status = notificationdata.Count != 0;
                res.Data = notificationdata.Count.ToString();

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion



        #region This Api Use To Remove All Notification
        /// <summary>
        /// Created By Shagun Date-20/08/2022
        /// Api >> Delete >> api/notification/removeallnotification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("removeallnotification")]
        public async Task<ResponseBodyModel> Removedate()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var notificationdata = await _db.NotificationDb.Where(x => x.CompanyId == claims.companyId && x.EmployeeId == claims.employeeId && x.IsActive && !x.IsDeleted).ToListAsync();
                if (notificationdata != null)
                {
                    foreach (var item in notificationdata)
                    {
                        item.IsDeleted = true;
                        item.IsActive = false;
                        item.IsNew = false;
                        item.DeleteDate = DateTime.Now;

                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "All Notification Cleared";
                    res.Status = true;
                    res.Data = notificationdata;
                }
                else
                {
                    res.Message = "Not Notificaton Found";
                    res.Status = false;
                    res.Data = notificationdata;
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


    }
}