using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using LinqKit;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet.NewProject
{
    /// <summary>
    /// Created By Ravi Vyas On 16/01/2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/projectdocument")]
    public class ProjectDocumentController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Add And Update Document In Project
        /// <summary>
        /// API>>POST>>api/projectdocument/addupdateprojectdocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdateprojectdocument")]
        public async Task<IHttpActionResult> AddProjectDocument(DocumentRequestBody model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var checkData = _db.ProjectDocuments.Where(x => x.IsActive && !x.IsDeleted &&
                                     x.ProjectDocumentId == model.ProjectDocumentId)
                                     .FirstOrDefault();
                    if (checkData == null)
                    {
                        ProjectDocument obj = new ProjectDocument
                        {
                            ProjectId = model.ProjectId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };

                        _db.ProjectDocuments.Add(obj);
                        await _db.SaveChangesAsync();
                        checkData = obj;
                    }
                    else
                    {
                        checkData.UpdatedBy = tokenData.employeeId;
                        checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    }
                    checkData.DocumentTypeConstants = model.DocumentTypeConstants;
                    checkData.DocumentTitleName = model.DocumentName;
                    checkData.Description = model.Description;
                    checkData.Attachment = model.Attachment;
                    checkData.CompanyId = tokenData.companyId;

                    _db.Entry(checkData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Document Added Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectdocument/addupdateprojectdocument | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Get All Project Document 
        /// <summary>
        /// Created By Ravi Vyas On 16/01/2023
        /// API>>GET>>api/projectdocument/getallprojectdocument
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectdocument")]
        public async Task<IHttpActionResult> GetAllDocument(int projectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ProjectDocuments.Where(x => x.IsActive && !x.IsDeleted && x.ProjectId == projectId)
                    .Select(x => new DocumentResponeBody
                    {
                        ProjectDocumentId = x.ProjectDocumentId,
                        DocumentTitleName = x.DocumentTitleName,
                        Description = x.Description,
                        Attachment = x.Attachment,
                        Extension = "",
                        CreatedOn = x.CreatedOn,
                        CreatedByName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                        DocumentCategory = x.DocumentTypeConstants.ToString(),
                        DocumentTypeId = x.DocumentTypeConstants
                    })
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();
                if (getData.Count > 0)
                {
                    getData.ForEach(y =>
                    {
                        y.Extension = Path.GetExtension(y.Attachment);
                    });

                    res.Message = "Document  Succesfully Get !";
                    res.Status = true;

                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = getData.Count,
                            Counts = (int)count,
                            List = getData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        return Ok(res);
                    }
                    else
                    {
                        res.Data = getData;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Data Not Found!";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectdocument/getallprojectdocument | " +
                   "Pay Group Id : " + projectId + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class DocumentResponeBody
        {
            public Guid ProjectDocumentId { get; set; }
            public int ProjectId { get; set; }
            public string DocumentTitleName { get; set; }
            public string Description { get; set; }
            public string Attachment { get; set; }
            public string Extension { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public string CreatedByName { get; set; }
            public string DocumentCategory { get; set; }
            public bool IsActive { get; set; } = true;
            public bool IsDeleted { get; set; } = false;
            public DocumentTypeConstants DocumentTypeId { get; set; }
        }

        #endregion

        #region Api for Delete Project Documents 
        /// <summary>
        /// api/projectdocument/deleteprojectdocuments
        /// </summary>
        /// <param name="projectDocumentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteprojectdocuments")]
        public async Task<IHttpActionResult> DeleteProjectDocumwnt(Guid projectDocumentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.ProjectDocuments.Where(x => x.ProjectDocumentId == projectDocumentId
                                                           && x.IsActive && !x.IsDeleted)
                                                           .FirstOrDefaultAsync();
                if (checkData != null)
                {
                    checkData.IsActive = false;
                    checkData.IsDeleted = true;
                    checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone); ;
                    checkData.UpdatedBy = tokenData.employeeId;
                    _db.Entry(checkData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Delete Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/projectdocument/getallprojectdocument | " +
                   "Pay Group Id : " + projectDocumentId + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Get Project Document  By ProjectDocumentId
        /// <summary>
        /// Created By Ravi Vyas On 16/01/2023
        /// API>>GET>>api/projectdocument/getallprojectdocumentbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectdocumentbyid")]
        public async Task<IHttpActionResult> GetAllDocumentById(Guid projectDocumentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ProjectDocuments.Where(x => x.IsActive && !x.IsDeleted && x.ProjectDocumentId == projectDocumentId)
                    .Select(x => new
                    {
                        ProjectDocumentId = x.ProjectDocumentId,
                        DocumentTitleName = x.DocumentTitleName,
                        Description = x.Description,
                        Attachment = x.Attachment,
                        CreatedOn = x.CreatedOn,
                        DocumentTypeConstants = x.DocumentTypeConstants,
                        CreatedByName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                    })
                    .FirstOrDefaultAsync();
                if (getData != null)
                {
                    res.Message = "Data Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found!";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectdocument/getallprojectdocumentbyid | " +
                   "Pay Group Id : " + projectDocumentId + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api To Upload Attached Task File 
        /// <summary>
        /// Created By Ravi Vyas On 06-02-2023
        /// API>>Post>>api/projectdocument/uploadprojectattechment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadprojectattechment")]
        public async Task<UploadImageResponse> UploadTaskAttechment()
        {
            UploadImageResponse result = new UploadImageResponse();
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
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        //if (extemtionType == "image")
                        //{
                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/ProjectDocument/"), dates + '.' + filename).Replace(" ", "");
                        string DirectoryURL = (FileUrl.Split(new string[] { "ProjectDocument" + "\\" }, StringSplitOptions.None).FirstOrDefault()) + "ProjectDocument";

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\ProjectDocument\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successful";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path.Replace(" ", "");
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                        //}
                        //else
                        //{
                        //    result.Message = "Only Select Image Format";
                        //    result.Status = false;
                        //}
                    }
                    else
                    {
                        result.Message = "No content Passed ";
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

        #endregion Api To Upload Navigation Logo

        #region Get All Project Document Type   // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 13-02-2023
        /// API >> Get >>api/projectdocument/getprojecttitletypeenum
        /// </summary>
        [Route("getprojecttitletypeenum")]
        [HttpGet]
        public ResponseBodyModel TaskStatus()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getStatus = Enum.GetValues(typeof(DocumentTypeConstants))
                    .Cast<DocumentTypeConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(DocumentTypeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Title Get Succesfully";
                res.Status = true;
                res.Data = getStatus;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Project Document Type  // dropdown

        #region Api for Serach  Document
        /// <summary>
        /// API>>GET>>api/projectdocument/fillterdocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("fillterdocument")]
        public async Task<IHttpActionResult> FillterDocument(DocumentFillterResquest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DateTimeOffset checkDate = new DateTimeOffset();
            DateTimeOffset checkDate1 = new DateTimeOffset();

            try
            {
                if (model.DocumentStartDate.HasValue)
                    checkDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.DocumentStartDate.Value, tokenData.TimeZone);
                if (model.DocumentEndDate.HasValue)
                    checkDate1 = TimeZoneConvert.ConvertTimeToSelectedZone(model.DocumentEndDate.Value, tokenData.TimeZone);


                var getData = await _db.ProjectDocuments.
                              Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId &&
                              x.ProjectId == model.ProjectId)
                              .Select(x => new DocumentResponeBody
                              {
                                  DocumentCategory = x.DocumentTypeConstants.ToString(),
                                  DocumentTitleName = x.DocumentTitleName,
                                  Description = x.Description,
                                  Attachment = x.Attachment,
                                  CreatedOn = x.CreatedOn,
                                  CreatedByName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                  Extension = "",
                                  DocumentTypeId = x.DocumentTypeConstants
                              }).ToListAsync();
                if (getData.Count > 0)
                {
                    getData.ForEach(y =>
                    {
                        y.Extension = Path.GetExtension(y.Attachment);
                    });
                    var predicate = PredicateBuilder.New<DocumentResponeBody>(x => x.IsActive && !x.IsDeleted);
                    if (model.DocumentType.Count > 0)
                    {
                        getData = (getData.Where(x => model.DocumentType.Contains(x.DocumentTypeId))).ToList();
                    }
                    if (model.DocumentStartDate.HasValue && model.DocumentEndDate.HasValue)
                    {
                        getData = (getData.Where(x => x.CreatedOn.Date >= checkDate.Date && x.CreatedOn.Date <= checkDate1.Date)).ToList();
                    }
                    if (model.Serach != null)
                    {
                        getData = (getData.Where(x => x.DocumentTitleName.ToUpper().StartsWith(model.Serach.ToUpper())
                        || x.CreatedByName.ToUpper().StartsWith(model.Serach.ToUpper())
                        || x.Description.ToUpper().StartsWith(model.Serach.ToUpper())
                        || x.DocumentCategory.ToUpper().StartsWith(model.Serach.ToUpper()))).ToList();
                    }
                    res.Message = "Task list Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    if (model.page.HasValue && model.count.HasValue)
                    {

                        res.Data = new PaginationData
                        {
                            TotalData = getData.Count,
                            Counts = (int)model.count,
                            List = getData.Skip(((int)model.page - 1) * (int)model.count).Take((int)model.count).ToList(),
                        };
                        return Ok(res);
                    }
                    else
                    {
                        res.Data = getData;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/projectdocument/fillterdocument", ex.Message, model);
                return BadRequest("Failed");
            }
        }

        public class DocumentFillterResquest
        {
            public List<DocumentTypeConstants> DocumentType { get; set; } = new List<DocumentTypeConstants>();
            public DateTime? DocumentStartDate { get; set; }
            public DateTime? DocumentEndDate { get; set; }
            public string Serach { get; set; }
            public int ProjectId { get; set; }
            public int? page { get; set; } = null;
            public int? count { get; set; } = null;
        }
        public class DocumentFillterResponse
        {
            public string Category { get; set; }
            public string Title { get; set; }
            public DateTimeOffset DocumentUploadDate { get; set; }
            public string UploadBy { get; set; }
            public string Document { get; set; }
        }
        #endregion

        #region Helper Model
        public class AddDocumentRequestBodyModel
        {
            public List<DocumentRequestBody> documentRequestBodys { get; set; }
        }
        public class DocumentRequestBody
        {
            public Guid ProjectDocumentId { get; set; } = Guid.NewGuid();
            public int ProjectId { get; set; }
            public string DocumentName { get; set; }
            public string Attachment { get; set; }
            public string Description { get; set; }
            public DocumentTypeConstants DocumentTypeConstants { get; set; }
        }
        #endregion

    }
}
