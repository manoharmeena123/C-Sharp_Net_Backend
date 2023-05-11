using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.Payment;
using AspNetIdentity.WebApi.Models;
using LinqKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Payment
{
    [Authorize]
    [RoutePrefix("api/projectpayment")]
    public class PaymentController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Project Payment Apis  

        #region Api To Add Project Payment
        /// <summary>
        /// Created By Mayank Prajapati On 06/01/2023
        /// API >> Post >>api/projectpayment/addpayment
        /// </summary>
        /// <returns></returns>
        [Route("addpayment")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddPayment(Paymenthelper model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    PaymentModel pay = new PaymentModel
                    {
                        ProjectId = model.ProjectId,
                        ProjectName = model.ProjectName,
                        PaymentMode = model.PaymentMode,
                        PaymentModeName = Enum.GetName(typeof(PaymentModeConstants), model.PaymentMode),
                        TransactionType = model.TransactionType,
                        TransactionName = Enum.GetName(typeof(TransactionTypeConstants), model.TransactionType),
                        ChequeNo = model.ChequeNo,
                        Comment = model.Comment,
                        TransactionNo = model.TransactionNo,
                        Amount = model.Amount,
                        Date = model.Date,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = DateTime.Now,

                    };
                    _db.PaymentModels.Add(pay);
                    await _db.SaveChangesAsync();

                    res.Message = "Payment added successfully";
                    res.Status = true;
                    res.Data = pay;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class Paymenthelper
        {
            public string ProjectName { get; set; } = string.Empty;
            public int ProjectId { get; set; } = 0;
            public int ChequeNo { get; set; } = 0;
            public int TransactionNo { get; set; } = 0;
            public DateTime Date { get; set; }
            public string Comment { get; set; } = string.Empty;
            public double Amount { get; set; }
            public TransactionTypeConstants TransactionType { get; set; }
            public PaymentModeConstants PaymentMode { get; set; }
        }
        #endregion Add Project Payment

        #region This Api Use Get Project Payment all Data
        /// <summary>
        /// API >> Get >>api/projectpayment/getprojectpayment
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectpayment")]
        public async Task<IHttpActionResult> GetProjectPayment()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var payment = await _db.PaymentModels.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();

                if (payment.Count > 0)
                {

                    res.Message = "Get Payment successfully  !";
                    res.Status = true;
                    res.Data = payment;
                }
                else
                {
                    res.Message = " Unable to Payment Details";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        #endregion Get all Project Payment Detail

        #region This Api Use Get By Project Name Search
        /// <summary>
        /// API >> Get >>api/projectpayment/getbyprojectnamesearch
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyprojectnamesearch")]
        public async Task<IHttpActionResult> GetByProjectName(string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var payment = (from a in _db.PaymentModels
                               where (a.IsActive && !a.IsDeleted && (a.ProjectName.ToLower().Contains(search.ToLower())))
                               select new ProjectPaymentResponse
                               {

                                   ProjectName = a.ProjectName,
                               }).ToList();
                if (payment.Count > 0)
                {
                    res.Message = "Get Payment successfully  !";
                    res.Status = true;
                    res.Data = payment;
                }
                else
                {
                    res.Message = " Unable to Payment Details";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class ProjectPaymentResponse
        {
            public string ProjectName { get; set; }
        }
        #endregion Get all Project Payment Detail

        #region Api to get all payments by month
        /// <summary>
        /// API >> Get >>api/projectpayment/getpaymentbymonth
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaymentbymonth")]
        public async Task<IHttpActionResult> GetPaymentByMonth()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetPaymentHelper response = new GetPaymentHelper();
            try
            {
                var monthsData = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                         .TakeWhile(m => m != String.Empty)
                                         .Select((m, i) => new
                                         {
                                             Month = i + 1,
                                             MonthName = m
                                         }).ToList();
                var currentYear = DateTime.Now.Year;
                var paymentHistory = await _db.PaymentModels.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.CompanyId == tokenData.companyId).ToListAsync();
                var monthNamesData = monthsData.ConvertAll(x => x.MonthName);

                var listDynamic = new List<dynamic>();
                var List1 = new List<double>();
                var data1 = new
                {
                    label = "Debit",
                    data = List1,
                };
                var List2 = new List<double>();
                var data2 = new
                {
                    label = "Credit",
                    data = List2,
                };
                foreach (var item in monthsData)
                {
                    List1.Add(paymentHistory.Where(x => x.Date.Month == item.Month &&
                    x.TransactionType == TransactionTypeConstants.Debit).Select(x => x.Amount).Sum());
                    List2.Add(paymentHistory.Where(x => x.Date.Month == item.Month &&
                    x.TransactionType == TransactionTypeConstants.Credit).Select(x => x.Amount).Sum());
                }
                listDynamic.Add(data1);
                listDynamic.Add(data2);


                response.label = monthNamesData;
                response.graph = listDynamic;

                res.Message = "Get Succesfully !";
                res.Status = true;
                res.Data = response;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        /// <summary>
        /// Created By Mayank Prajapati on 03-04-2022
        /// </summary>
        public class GetPaymentHelper
        {
            public Object label { get; set; }
            public Object graph { get; set; }
        }

        /// <summary>
        /// Created By  on 11-04-2022
        /// </summary>
        public class GetPaymentBarModel
        {
            public List<string> Name { get; set; }
            public List<double> DabitValue { get; set; }
            public List<double> CreditValue { get; set; }
        }
        #endregion Get all Project Payment Detail

        #region This is used for get Project Name By Id
        /// <summary>
        ///  Created by Mayank Prajappati on 06/01/2023
        /// API >> Get >>api/projectpayment/getprojectnamebyid
        /// </summary>
        /// <param name="projectPaymentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectnamebyid")]
        public async Task<IHttpActionResult> GetProjectNameById()
        {

            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getproject = await _db.ProjectLists
                     .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                     .Select(x => new
                     {

                         x.ProjectName,
                     })
                     .ToListAsync();
                if (getproject != null)
                {
                    res.Message = "Employee Data Found";
                    res.Status = true;
                    res.Data = getproject;
                }
                else
                {
                    res.Message = " Data not found ";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }

        #endregion This is used for get Payment By Id

        #region Api for Task Filter
        /// <summary>
        /// API>POST>>api/projectpayment/Paymentfillters
        /// </summary>
        /// <param name="ThisMonth"></param>
        /// <param name="ThisYear"></param>
        /// <param name="customRange"></param>
        /// <returns></returns> [HttpPost]
        [Route("Paymentfillters")]
        [HttpPost]
        public async Task<IHttpActionResult> GetByDate(FillertResponse model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            CustomDateResponce response = new CustomDateResponce();

            try
            {

                var dateData = await (from t in _db.PaymentModels
                                      where t.IsActive && !t.IsDeleted && t.CompanyId == tokenData.companyId
                                      select new CustomDateResponce
                                      {
                                          ProjectName = t.ProjectName,
                                          Date = t.Date,
                                          Amount = t.Amount,
                                          PaymentModeName = t.PaymentModeName,
                                          //ClientName = t.ClientName,
                                          TransactionName = t.TransactionName,
                                          TransactionNo = t.TransactionNo,
                                          Comment = t.Comment,
                                          ChequeNo = t.ChequeNo,

                                      }).ToListAsync();
                if (dateData.Count > 0)
                {
                    var predicate = PredicateBuilder.New<CustomDateResponce>(x => x.IsActive && !x.IsDeleted);
                    if (model.ThisMonth != 0)
                    {
                        dateData = (dateData.Where(x => x.Date.Month == model.ThisMonth)).ToList();
                    }
                    if (model.ThisYear != 0)
                    {
                        dateData = (dateData.Where(x => x.Date.Year == model.ThisYear)).ToList();
                    }
                    if (model.StartDate != null || model.EndDate != null)
                    {
                        dateData = (dateData.Where(x => x.Date >= model.StartDate && x.Date <= model.EndDate)).ToList();
                    }
                    res.Message = "Payment List Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = dateData;
                }
                else
                {
                    res.Message = "Payment List Found";
                    res.Status = false;

                }

            }
            catch (Exception ex)
            {
                logger.Error("api/taskcreation/taskfillters", ex.Message, model);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class CustomDateResponce
        {
            public string ProjectName { get; set; }
            public DateTime Date { get; set; }
            public double Amount { get; set; }
            public string PaymentModeName { get; set; }
            //public string ClientName { get; set; }
            public string TransactionName { get; set; }
            public int ChequeNo { get; set; }
            public int TransactionNo { get; set; }
            public string Comment { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }

        }
        public class FillertResponse
        {
            public int ThisMonth { get; set; }
            public int ThisYear { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }
        }

        #endregion

        #region This Api Use To Get Payment Type Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 09/01/2023
        ///// Api >> Get >> api/projectpayment/getallpayment
        /////
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("getallpayment")]
        [HttpGet]
        public IHttpActionResult GetAllPayment()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = Enum.GetValues(typeof(PaymentModeConstants))
                    .Cast<PaymentModeConstants>()
                    .Select(x => new ProjectPaymentTypeModel
                    {
                        PaymnetTypeId = (int)x,
                        PaymnetTypeName = Enum.GetName(typeof(PaymentModeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Payment Type Exist";
                res.Status = true;
                res.Data = payment;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class ProjectPaymentTypeModel
        {
            public int PaymnetTypeId { get; set; }
            public string PaymnetTypeName { get; set; }
        }
        #endregion

        #region This Api Use To Get Transaction Type Enum Api
        ///// <summary>
        ///// created by  Mayank Prajapati On 09/01/2023
        ///// Api >> Get >> api/projectpayment/gettransactiontype
        /////
        ///// <param name="model"></param>
        ///// <returns></returns>
        [Route("gettransactiontype")]
        [HttpGet]
        public IHttpActionResult GetTransactionType()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = Enum.GetValues(typeof(TransactionTypeConstants))
                    .Cast<TransactionTypeConstants>()
                    .Select(x => new TransactionTypeModel
                    {
                        TransactionTypeId = (int)x,
                        TransactionTypeName = Enum.GetName(typeof(TransactionTypeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Payment Type Exist";
                res.Status = true;
                res.Data = payment;
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        public class TransactionTypeModel
        {
            public int TransactionTypeId { get; set; }
            public string TransactionTypeName { get; set; }
        }
        #endregion

        #region This is used for get  Payment By Id
        /// <summary>
        ///  Created by Mayank Prajappati on 06/01/2023
        /// API >> Get >> api/projectpayment/getpaymnetbyid
        /// </summary>
        /// <param name="projectPaymentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaymnetbyid")]
        public async Task<IHttpActionResult> GetPaymentById(Guid projectPaymentId)
        {

            ResponseStatusCode res = new ResponseStatusCode();

            try
            {
                var payment = _db.PaymentModels.Where(x => x.ProjectPaymentId == projectPaymentId && x.IsActive && !x.IsDeleted).FirstOrDefault();


                if (payment != null)
                {

                    res.Status = true;
                    res.Message = "Payment List Found";
                    res.Data = payment;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Payment List Not Found";

                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }

        #endregion This is used for get Payment By Id

        #region Update Project Payment Data
        /// <summary>
        /// Created by Mayank Prajapati On 06/01/2023
        /// API >> Put >>api/projectpayment/updatepayment
        /// </summary>
        /// use to update the Update client
        /// <param name="updateclient"></param>
        /// <returns></returns>

        [Route("updatepayment")]
        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> updatePayment(PaymentModel model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    PaymentModel Paymnet = new PaymentModel
                    {
                        ProjectId = model.ProjectId,
                        ProjectName = model.ProjectName,
                        PaymentMode = model.PaymentMode,
                        PaymentModeName = Enum.GetName(typeof(PaymentModeConstants), model.PaymentMode),
                        TransactionType = model.TransactionType,
                        TransactionName = Enum.GetName(typeof(TransactionTypeConstants), model.TransactionType),
                        ChequeNo = model.ChequeNo,
                        Comment = model.Comment,
                        TransactionNo = model.TransactionNo,
                        Amount = model.Amount,
                        Date = model.Date,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        UpdatedBy = tokenData.employeeId,
                        UpdatedOn = DateTime.Now,
                    };
                    _db.Entry(Paymnet).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Updated Successfully!";
                    res.Data = Paymnet;
                }
                else
                {
                    res.Message = " Payment Not Updated ";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Deleted Project Payment
        /// <summary>
        /// API >> Delete >>api/projectpayment/removeprojectpayment
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("removeprojectpayment")]
        public async Task<IHttpActionResult> ProjectPaymentDetailsDelete(Guid projectPaymentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = await _db.PaymentModels.FirstOrDefaultAsync(x =>
                    x.ProjectPaymentId == projectPaymentId && x.CompanyId == tokenData.companyId);
                if (payment != null)
                {
                    payment.IsDeleted = true;
                    payment.IsActive = false;
                    payment.DeletedBy = tokenData.employeeId;
                    payment.DeletedOn = DateTime.Now;

                    _db.Entry(payment).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = payment;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion

        #region Project Payment Import File
        /// <summary>
        /// Api >> Post >>api/projectpayment/addprojectpaymentbyexcelimport
        /// created by Mayank Prajapati on 20/1/2023
        /// </summary>
        [HttpPost]
        [Route("addprojectpaymentbyexcelimport")]
        public async Task<ResponseBodyModel> ProjectPaymentImport(List<PaymentModel> Item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<PaymentTypeResponse> paymentImportItem = new List<PaymentTypeResponse>();
            // long successfullImported = 0;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                if (Item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    res.Data = paymentImportItem;
                    return res;
                }
                else
                {
                    foreach (var model in Item)
                    {
                        var pamentData = _db.PaymentModels.Where
                               (x => x.IsActive && !x.IsDeleted && x.ProjectName == model.ProjectName).ToList();
                        if (pamentData == null)
                        {
                            PaymentModel post = new PaymentModel
                            {
                                ProjectId = model.ProjectId,
                                ProjectName = model.ProjectName,
                                //ClientName = model.ClientName,
                                PaymentModeName = Enum.GetName(typeof(PaymentModeConstants), model.PaymentMode),
                                TransactionName = Enum.GetName(typeof(TransactionTypeConstants), model.TransactionType),
                                Amount = model.Amount,
                                Comment = model.Comment,
                                ChequeNo = model.ChequeNo,
                                TransactionNo = model.TransactionNo,
                                Date = DateTime.Now,
                                CompanyId = tokenData.companyId,
                                OrgId = tokenData.orgId,
                                CreatedBy = tokenData.employeeId,
                                CreatedOn = DateTime.Now,
                            };
                            _db.PaymentModels.Add(post);
                            await _db.SaveChangesAsync();

                            res.Message = "Employee Imported Succesfull ";
                            res.Status = true;
                            res.Data = pamentData;
                        }
                        else
                        {
                            res.Message = "Duplicated Data";
                            res.Status = false;
                            res.Data = pamentData;
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
        public class PaymentTypeResponse
        {

            public List<PaymentModel> PaymentModels { get; set; }
        }

        #endregion

        #region Getall Payment export

        /// <summary>
        ///  Created by 
        ///  API >> POST >>api/projectpayment/getprojectpaymentexport
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("getprojectpaymentexport")]
        public async Task<ResponseBodyModel> GetAllPaymentDataExport()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var paymentobj = await (from a in _db.PaymentModels
                                        where (a.IsActive && !a.IsDeleted && a.CompanyId == claims.companyId)
                                        select new GetAllPaymentExportModel
                                        {
                                            ProjectName = a.ProjectName,
                                            Amount = a.Amount,
                                            //ClientName = a.ClientName,
                                            PaymentModeName = a.PaymentModeName.ToString(),
                                            PaymentMode = a.PaymentMode,
                                            TransactionName = a.TransactionName.ToString(),
                                            TransactionType = a.TransactionType,
                                            Comment = a.Comment,
                                            ChequeNo = a.ChequeNo,
                                            TransactionNo = a.TransactionNo,
                                            Date = a.Date

                                        }).ToListAsync();
                res.Message = "Data Download";
                res.Status = true;
                res.Data = paymentobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class GetAllPaymentExportModel
        {
            public Guid ProjectPaymentId { get; set; } = Guid.NewGuid();
            public string ProjectName { get; set; }
            public int ProjectId { get; set; }
            public int ChequeNo { get; set; }
            public int TransactionNo { get; set; }
            public DateTime Date { get; set; }
            public string Comment { get; set; }
            public double Amount { get; set; }
            public TransactionTypeConstants TransactionType { get; set; }
            public string TransactionName { get; set; }
            // public string ClientName { get; set; }
            public PaymentModeConstants PaymentMode { get; set; }
            public string PaymentModeName { get; set; }

        }
        #endregion

        #endregion

        #region  Project Feedback Apis 

        #region Api To Add Project Feedback
        /// <summary>
        /// Created By Mayank Prajapati On 06/01/2023
        /// API >> Post >>api/projectpayment/addprojectfeedback
        /// </summary>
        /// <returns></returns>
        [Route("addprojectfeedback")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddProjectFeedback(ProjectFeedback model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    ProjectFeedback feedback = new ProjectFeedback
                    {
                        CustomerName = model.CustomerName,
                        ProjectId = model.ProjectId,
                        ReviewType = model.ReviewType,
                        Rating = model.Rating,
                        FeedBack = model.FeedBack,
                        Project = model.Project,
                        Comments = model.Comments,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.ProjectFeedbacks.Add(feedback);
                    await _db.SaveChangesAsync();

                    res.Message = "added Project FeedBack successfully";
                    res.Status = true;
                    res.Data = feedback;
                }
                else
                {
                    res.Message = "Unable to added  Project FeedBack ";
                    res.Status = false;
                }
            }

            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        #endregion Add Project Payment

        #region This Api Use Get Project FeedBack all Data
        /// <summary>
        /// API >> Get >>api/projectpayment/getprojectfeedback
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectfeedback")]
        public async Task<IHttpActionResult> GetProjectFeedBack()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = _db.ProjectFeedbacks.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToList();
                if (payment != null)
                {
                    res.Message = "Get Project Feedback Successfully  !";
                    res.Status = true;
                    res.Data = payment;
                }
                else
                {
                    res.Message = " Project Feedback  not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion Get all  Project FeedBack

        #region This is used for get detail by  Project FeedBack Id
        /// <summary>
        ///  Created by Mayank Prajappati on 06/01/2023
        /// API >> Get >> api/projectpayment/getprojectfeedbackbyid
        /// </summary>
        /// <param name="projectFeedbackId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectfeedbackbyid")]
        public async Task<IHttpActionResult> GetProjectFeedbackBYId(Guid projectFeedbackId)
        {

            ResponseStatusCode res = new ResponseStatusCode();

            try
            {
                var pmo = _db.ProjectFeedbacks.Where(x => x.ProjectFeedbackId == projectFeedbackId && x.IsActive && !x.IsDeleted).FirstOrDefault();


                if (pmo != null)
                {
                    res.Status = true;
                    res.Message = "Project Feedback List Found";
                    res.Data = pmo;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Project Feedback List Found";

                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }

        #endregion This is used for get Project FeedBack By Id 

        #region Update Project Feedback
        /// <summary>
        /// Created by Mayank Prajapati On 06/01/2023
        /// API >> Put >>api/projectpayment/updateprojectfeedback
        /// </summary>
        /// use to update the Update client
        /// <param name="updateProjectFeedback"></param>
        /// <returns></returns>

        [Route("updateprojectfeedback")]
        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> updateProjectFeedBack(ProjectFeedback model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    ProjectFeedback feedback = new ProjectFeedback
                    {
                        CustomerName = model.CustomerName,
                        ProjectId = model.ProjectId,
                        ReviewType = model.ReviewType,
                        Rating = model.Rating,
                        FeedBack = model.FeedBack,
                        Project = model.Project,
                        Comments = model.Comments,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.ProjectFeedbacks.Add(feedback);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Updated Successfully!";
                    res.Data = feedback;
                }
                else
                {
                    res.Message = "Update request failed";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Deleted Project FeedBack
        /// <summary>
        /// API >> Delete >>api/projectpayment/removeprojectfeedback
        ///  Created by Mayank Prajapati On 06/01/2023
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("removeprojectfeedback")]
        public async Task<IHttpActionResult> ProjectFeedBackDelete(Guid projectFeedbackId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payment = await _db.ProjectFeedbacks.FirstOrDefaultAsync(x =>
                    x.ProjectFeedbackId == projectFeedbackId && x.CompanyId == tokenData.companyId);
                if (payment != null)
                {
                    payment.IsDeleted = true;
                    payment.IsActive = false;
                    payment.DeletedBy = tokenData.employeeId;
                    payment.DeletedOn = DateTime.Now;

                    _db.Entry(payment).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = payment;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion

        #endregion

        #region PMO Apis 

        #region Api To Add PMO
        /// <summary>
        /// Created By Mayank Prajapati On 07/01/2023
        /// API >> Post >>api/projectpayment/addpmo
        /// </summary>
        /// <returns></returns>
        [Route("addpmo")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddPMO(PMO model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    PMO project = new PMO
                    {
                        ProjectId = model.ProjectId,
                        EmployeeId = model.EmployeeId,
                        FeedBack = model.FeedBack,
                        DeletedOn = model.DeletedOn,
                        Rating = model.Rating,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.PMOs.Add(project);
                    await _db.SaveChangesAsync();

                    res.Message = "added PMO successfully";
                    res.Status = true;
                    res.Data = project;
                }
                else
                {
                    res.Message = "Unable to added  PMO ";
                    res.Status = false;
                }
            }

            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }

        #endregion Add PMO

        #region This Api Use Get PMO all Data
        /// <summary>
        /// API >> Get >>api/projectpayment/getpmo
        ///  Created by Mayank Prajapati On 07/01/2023
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpmo")]
        public async Task<IHttpActionResult> GetPMO()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var project = _db.PMOs.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToList();
                if (project != null)
                {
                    res.Message = "Get PMO Successfully  !";
                    res.Status = true;
                    res.Data = project;
                }
                else
                {
                    res.Message = "PMO not found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion Get all PMO

        #region This is used for get detail by  PMO Id

        /// <summary>
        ///  Created by Mayank Prajappati on 07/01/2023
        /// API >> Get >> api/projectpayment/getpmobyid
        /// </summary>
        /// <param name="pmoId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpmobyid")]
        public async Task<IHttpActionResult> GetPMOById(Guid pmoId)
        {

            ResponseStatusCode res = new ResponseStatusCode();

            try
            {
                var pmo = _db.PMOs.Where(x => x.PMOId == pmoId && x.IsActive && !x.IsDeleted).FirstOrDefault();


                if (pmo != null)
                {
                    res.Status = true;
                    res.Message = "PMO List Found";
                    res.Data = pmo;
                }
                else
                {
                    res.Status = false;
                    res.Message = "PMO List Not Found";

                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }

        #endregion This is used for get by PMO By Id

        #region Update PMO
        /// <summary>
        /// Created by Mayank Prajapati On 07/01/2023
        /// API >> Put >>api/projectpayment/updatepmo
        /// </summary>
        /// use to update the Update PMO
        /// <returns></returns>

        [Route("updatepmo")]
        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> updatePMO(PMO model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    PMO pmoproject = new PMO
                    {
                        ProjectId = model.ProjectId,
                        EmployeeId = model.EmployeeId,
                        FeedBack = model.FeedBack,
                        DeletedOn = model.DeletedOn,
                        Rating = model.Rating,
                        CompanyId = tokenData.companyId,
                        OrgId = tokenData.orgId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = DateTime.Now,
                    };
                    _db.PMOs.Add(pmoproject);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Updated PMO Successfully!";
                    res.Data = pmoproject;
                }
                else
                {
                    res.Message = " PMO Update failed";
                    res.Status = false;
                };
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return Ok(res);
        }
        #endregion

        #region This Api Use Deleted PMO
        /// <summary>
        /// API >> Delete >>api/projectpayment/removePMO
        ///  Created by Mayank Prajapati On 07/01/2023
        /// </summary>
        /// <returns></returns>

        [HttpDelete]
        [Route("removePMO")]
        public async Task<IHttpActionResult> PMODelete(Guid pmoId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var project = await _db.PMOs.FirstOrDefaultAsync(x =>
                    x.PMOId == pmoId && x.CompanyId == tokenData.companyId);
                if (project != null)
                {
                    project.IsDeleted = true;
                    project.IsActive = false;
                    project.DeletedBy = tokenData.employeeId;
                    project.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(project).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully  !";
                    res.Status = true;
                    res.Data = project;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        #endregion

        #endregion
    }
}
