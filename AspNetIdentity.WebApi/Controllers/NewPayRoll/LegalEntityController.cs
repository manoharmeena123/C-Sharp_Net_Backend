using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Payroll
{
    /// <summary>
    /// Created By Harshit Mitra on 21-02-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/legalentity")]
    public class LegalEntityController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE LEAGAL ENTITY OF COMPANY
        /// <summary>
        /// API >> Post >> api/legalentity/addlegalentity
        /// Created By Harshit Mitra on 16/12/2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addlegalentity")]
        public async Task<IHttpActionResult> AddCompanyLegalEntity(CreateLegalEntity model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                CompanyLegalEntity obj = new CompanyLegalEntity
                {
                    EntityName = model.EntityName,
                    LegalNameOfCompany = model.LegalNameOfCompany,
                    CompanyIdentifyNumber = model.CompanyIdentifyNumber,
                    DateOfIncorporation = TimeZoneConvert.ConvertTimeToSelectedZone(model.DateOfIncorporation),
                    TypeOfBusinessId = model.TypeOfBusinessId,
                    SectorId = model.SectorId,
                    NatureOfBusinessId = model.NatureOfBusinessId,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    CityId = model.CityId,
                    StateId = model.StateId,
                    ZipCode = model.ZipCode,
                    SignatoryUrl = model.SignatoryUrl,
                    CountryId = model.CountryId,
                    Logo = model.Logo,

                    CreatedBy = tokenData.employeeId,
                    CompanyId = tokenData.companyId,
                };
                _db.CompanyLegalEntities.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Company Legal Entity Created";
                res.StatusCode = HttpStatusCode.Created;
                res.Status = true;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/legalentity/addlegalentity | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion 

        #region API TO GET ALL LEGAL ENTITY OF COMPANY
        /// <summary>
        /// Created By Harshit Mitra on 16/12/2022
        /// API >> Get >> api/legalentity/legalentitylist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("legalentitylist")]
        public async Task<IHttpActionResult> GetLegalEntityList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var legalEntityList = await _db.CompanyLegalEntities
                    .Where(x => x.IsActive && !x.IsDeleted && tokenData.companyId == x.CompanyId)
                    .Select(x => new
                    {
                        x.LegalEntityId,
                        x.EntityName,
                    })
                    .ToListAsync();

                if (legalEntityList.Count == 0)
                {
                    res.Message = "No Legal Entity Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = legalEntityList;

                    return Ok(res);
                }
                res.Message = "Legal Entity Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = legalEntityList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/legalentity/legalentitylist | " +
                     //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion 

        #region API TO GET LEGAL ENTITY OF COMPANY BY ID
        /// <summary>
        /// Created By Harshit Mitra on 14-04-2022
        /// API >> Get >> api/legalentity/getlegalentitybyid
        /// </summary>
        /// <param name="legalEntityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getlegalentitybyid")]
        public async Task<IHttpActionResult> GetLegalEntityById(Guid legalEntityId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var legalEntity = await _db.CompanyLegalEntities
                    .FirstOrDefaultAsync(x => x.LegalEntityId == legalEntityId);
                if (legalEntity == null)
                {
                    res.Message = "Legal Entity Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                res.Message = "Legal Entity Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = legalEntity;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/legalentity/getlegalentitybyid | " +
                     "LegalEntityId : " + legalEntityId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion 

        #region Api To Update Legal Entity Signature Image

        /// <summary>
        /// Created By Harshit Mitra On 30-05-2022
        /// API >> Post >> api/legalentity/uploadentitysignature
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadentitysignature")]
        public async Task<UploadImageResponse> UploadEmployeeProfileImage()
        {
            UploadImageResponse result = new UploadImageResponse();
            try
            {
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
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/entitysignature"), filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { "\\" + filename }, StringSplitOptions.None).FirstOrDefault());

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            string path = "uploadimage\\entitysignature\\" + Fileresult + extension;

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

        #endregion Api To Update Legal Entity Signature Image


        #region REQUEST AND RESPONSE 

        public class CreateLegalEntity
        {
            public string EntityName { get; set; } = String.Empty;
            public string SignatoryUrl { get; set; } = String.Empty;
            public string LegalNameOfCompany { get; set; } = String.Empty;
            public string CompanyIdentifyNumber { get; set; } = String.Empty;
            public DateTime DateOfIncorporation { get; set; } = DateTime.UtcNow;
            public int TypeOfBusinessId { get; set; } = 0;
            public int SectorId { get; set; } = 0;
            public int NatureOfBusinessId { get; set; } = 0;
            public string AddressLine1 { get; set; } = String.Empty;
            public string AddressLine2 { get; set; } = String.Empty;
            public int CityId { get; set; } = 0;
            public int StateId { get; set; } = 0;
            public string ZipCode { get; set; } = String.Empty;
            public int CountryId { get; set; } = 0;
            public string Logo { get; set; } = String.Empty;
        }
        #endregion
    }
}