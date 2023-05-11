using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
    [RoutePrefix("api/Endrosement")]
    public class EndorsementController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region This Api Used By Post Endorsement

        /// <summary>
        /// Created By Ankit 05/06/2022
        /// Api >> Post >> api/Endrosement/PostEndorsement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostEndorsement")]
        [Authorize]
        public async Task<ResponseBodyModel> PostEndorsementType(Endorsement model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    Endorsement obj = new Endorsement
                    {
                        BadgeId = model.BadgeId,
                        EmployeeId = model.EmployeeId,
                        Comments = model.Comments,
                        DesignationName = model.DesignationName,
                        EndorsementTypeId = model.EndorsementTypeId,
                        RoleId = model.EmployeeId,
                        EndorsementsName = model.EndorsementsName,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.Endorsements.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "Endorsements Added Successfully";
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

        #endregion This Api Used By Post Endorsement

        #region API to Get All the Endorsement List

        /// <summary>
        /// Created By Ankit 05/06/2022
        /// Modify By Harshit Mitra on 05-08-2022
        /// Api >> Get >> api/Endrosement/GetAllEndorsementList
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllEndorsementList")]
        public async Task<ResponseBodyModel> GetAllEndorsementList(int page, int count)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                List<EndorsementData> EndorsementDataList = new List<EndorsementData>();

                var endorsementdata = (from ad in _db.Endorsements
                                       join bg in _db.EmpBadges on ad.BadgeId equals bg.BadgeId
                                       join em in _db.Employee on ad.EmployeeId equals em.EmployeeId
                                       where em.CompanyId == claims.companyId && em.OrgId == claims.orgId
                                       select new
                                       {
                                           ad.EndorsementId,
                                           em.EmployeeId,
                                           ad.EndorsementsName,
                                           n = em.DisplayName,
                                           ad.EndorsementTypeId,
                                           ad.CreatedOn,
                                           ad.UpdatedOn,
                                           bg.BadgeId,
                                           bg.Title,
                                           bg.BadgeName,
                                           ad.DesignationName
                                       }).ToList();
                foreach (var item in endorsementdata)
                {
                    EndorsementData data = new EndorsementData
                    {
                        EndorsementId = item.EndorsementId,
                        EmployeeId = item.EmployeeId,
                        FullName = item.n,
                        EndorsementsName = item.EndorsementsName,
                        DesignationName = item.DesignationName,
                        EndorsementTypeId = item.EndorsementTypeId,
                        Title = item.Title,
                        CreatedDate = item.CreatedOn,
                        BadgeId = item.BadgeId,
                        BadgeName = item.BadgeName
                    };
                    EndorsementDataList.Add(data);
                    await _db.SaveChangesAsync();
                }
                if (EndorsementDataList.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Data Found";
                    res.Data = new PaginationData
                    {
                        TotalData = EndorsementDataList.Count,
                        Counts = (int)count,
                        List = EndorsementDataList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                    };
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found";
                    res.Data = new PaginationData
                    {
                        TotalData = EndorsementDataList.Count,
                        Counts = (int)count,
                        List = EndorsementDataList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API to Get All the Endorsement List

        #region This Api used To Add Badegs

        /// <summary>
        /// Created By ankit 05/06/2022
        /// Api >> Post >> api/Endrosement/GetAllEndorsementList
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddedBadegs")]
        [Authorize]
        public async Task<ResponseBodyModel> AddBadegs(EmployeeBadges model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Invalid";
                    res.Status = false;
                }
                else
                {
                    EmployeeBadges obj = new EmployeeBadges
                    {
                        BadgeId = model.BadgeId,
                        // EmployeeId = model.EmployeeId,
                        BadgeName = model.BadgeName,
                        ImageUrl = model.ImageUrl,
                        BadgeType = model.BadgeType,
                        Description = model.Description,
                        Title = model.Title,
                        CreatedBy = claims.userId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _db.EmpBadges.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "Badges Added Successfully";
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

        #endregion This Api used To Add Badegs

        #region This Api Is Used to Get Endorsement filter

        /// <summary>
        /// Created By Ankit 04/05/2022
        /// Api >> Post >> api/Endrosement/GetEndorsementFilter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetEndorsementFilter")]
        public async Task<ResponseBodyModel> GetEndorsementFilter(EndorsementDirectory model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var EndorsementList = await (from e in _db.Endorsements
                                             join b in _db.EmpBadges on e.BadgeId equals b.BadgeId
                                             join en in _db.Employee on e.EmployeeId equals en.EmployeeId
                                             join ds in _db.Designation on en.DesignationId equals ds.DesignationId
                                             where e.IsActive == true && e.IsDeleted == false
                                             select new EndorsementDirectoryHelperClassAdmin
                                             {
                                                 EmployeeId = en.EmployeeId,
                                                 fullName = en.FirstName + "" + en.LastName,
                                                 DesignationId = en.DesignationId,
                                                 EndorsementId = e.EndorsementId,
                                                 EndorsementsName = e.EndorsementsName,
                                                 DesignationName = ds.DesignationName,
                                                 Title = b.Title,
                                                 CreatedDate = e.CreatedOn,
                                             }).ToListAsync();
                if (model.EndorsementId.Count == 0)
                {
                    if (model.DesignationId.Count == 0)
                    {
                        if (model.EmployeeId.Count == 0)
                        {
                            EndorsementList = EndorsementList.ToList();
                        }
                        else
                        {
                            EndorsementList = EndorsementList.Where(x => model.EmployeeId.Contains(x.EmployeeId)).ToList();
                        }
                    }
                    else
                    {
                        if (model.EmployeeId.Count == 0)
                        {
                            EndorsementList = EndorsementList.Where(x => model.DesignationId.Contains(x.DesignationId)).ToList();
                        }
                        else
                        {
                            EndorsementList = EndorsementList.Where(x => model.DesignationId.Contains(x.DesignationId) &&
                                    model.EmployeeId.Contains(x.EmployeeId)).ToList();
                        }
                    }
                }
                else
                {
                    if (model.DesignationId.Count == 0)
                    {
                        if (model.EmployeeId.Count == 0)
                        {
                            EndorsementList = EndorsementList.Where(x => model.EndorsementId.Contains(x.EndorsementId)).ToList();
                        }
                        else
                        {
                            EndorsementList = EndorsementList.Where(x => model.EndorsementId.Contains(x.EndorsementId) &&
                                model.EmployeeId.Contains(x.EmployeeId)).ToList();
                        }
                    }
                    else
                    {
                        if (model.EmployeeId.Count == 0)
                        {
                            EndorsementList = EndorsementList.Where(x => model.EndorsementId.Contains(x.EndorsementId) &&
                                model.DesignationId.Contains(x.DesignationId)).ToList();
                        }
                        else
                        {
                            EndorsementList = EndorsementList.Where(x => model.EndorsementId.Contains(x.EndorsementId) &&
                                model.DesignationId.Contains(x.DesignationId) && model.EmployeeId.Contains(x.EmployeeId)).ToList();
                        }
                    }
                }
                if (EndorsementList.Count > 0)
                {
                    res.Message = "Endorsement List";
                    res.Status = true;
                    res.Data = EndorsementList;
                }
                else
                {
                    res.Message = "Endorsement List Is Empty";
                    res.Status = false;
                    res.Data = EndorsementList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Is Used to Get Endorsement filter

        #region Create A Api use Get Badegs

        /// <summary>
        /// Created By Ankit 05/05/2022
        /// Api >> Get >> api/Endrosement/GetEmpBadegs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEmpBadegs")]
        [Authorize]
        public async Task<ResponseBodyModel> GetEmpBadegs()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var EmpBadgesData = await _db.EmpBadges.Where(x => x.IsActive == true && x.IsDeleted == false).ToListAsync();

                if (EmpBadgesData.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Data Found";
                    res.Data = EmpBadgesData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Create A Api use Get Badegs

        #region Api To Upload Badges Profile Image

        /// <summary>
        /// Created By Ankit On 18-05-2022
        /// API >> Post >> api/Endrosement/uploadbadgeimage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadbadgeimage")]
        public async Task<UploadImageResponseEndosment> UploadbadgeImages()
        {
            UploadImageResponseEndosment result = new UploadImageResponseEndosment();
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
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/badgesimages/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\badgesimages\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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

        #endregion Api To Upload Badges Profile Image

        #region Get badges directly

        [HttpGet]
        [Route("getbadges")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> GetBadgesDirectly()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            List<BadgesResponseModel> ListBadges = new List<BadgesResponseModel>();
            ResponseBodyModel obj = new ResponseBodyModel();
            try
            {
                var Badgesobj = new List<Badges>()
                {
                    new Badges()
                    {
                        Title = "A Mentoring Star",
                        Description = "Recognisation for Mentorship",
                        ImageUrl = "uploadimage/badges/AMentoringStar.png"
                    },
                    new Badges()
                    {
                        Title = "Amazing Team Player",
                        Description = "For outstanding team work",
                        ImageUrl = "uploadimage/badges/AmazingTeamPlayer.png"
                    },
                    new Badges()
                    {
                        Title = "Exeeded Expectations!",
                        Description = "Went above and Beyond to Deliver",
                        ImageUrl = "uploadimage/badges/ExeededExpectations!.png"
                    },
                    new Badges()
                    {
                        Title = "Good Work!",
                        Description = "Recognizing Good Work.",
                        ImageUrl = "uploadimage/badges/GoodWork!.png"
                    },
                    new Badges()
                    {
                        Title = "No.1 Contributer",
                        Description = "Top Contributer in a goal",
                        ImageUrl = "uploadimage/badges/Contributer.png"
                    },
                    new Badges()
                    {
                        Title = "OutStanding Work!",
                        Description = "Recognizing Outstanding Delivery",
                        ImageUrl = "uploadimage/badges/OutStandingWork!.png"
                    },
                    new Badges()
                    {
                        Title = "Top Performer",
                        Description = "For top performing people",
                        ImageUrl = "uploadimage/badges/TopPerformer.png"
                    },
                    new Badges()
                    {
                        Title = "Well done!",
                        Description = "Recognisation for Mentorship",
                        ImageUrl = "uploadimage/badges/Welldone.png"
                    },
                    new Badges()
                    {
                        Title = "you’re a rockstar",
                        Description = "you stand out from the crowd",
                        ImageUrl = "uploadimage/badges/youreastar.png"
                    },
                    new Badges()
                    {
                        Title = "you’re a star!",
                        Description = "Exellent work",
                        ImageUrl = "uploadimage/badges/yourearockstar.png"
                    }
                };

                foreach (var Badges in Badgesobj)
                {
                    BadgesResponseModel badges = new BadgesResponseModel();
                    badges.Title = Badges.Title;
                    badges.Description = Badges.Description;
                    badges.ImageUrl = Badges.ImageUrl;
                    ListBadges.Add(badges);
                }

                obj.Status = true;
                obj.Message = "Badges list found";
                obj.Data = ListBadges;
            }
            catch (Exception ex)
            {
                obj.Status = false;
                obj.Message = ex.Message;
            }
            return obj;
        }

        #endregion Get badges directly

        #region This are Used For ResponseHelper Class

        public class EndorsementDirectory
        {
            public List<int> EndorsementId { get; set; }
            public List<int> DesignationId { get; set; }
            public List<int> EmployeeId { get; set; }
        }

        public class UploadImageResponseEndosment
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        public class EndorsementDirectoryHelperClassAdmin
        {
            public int EmployeeId { get; set; }
            public string fullName { get; set; }
            public string DesignationName { get; set; }
            public int DesignationId { get; set; }
            public int EndorsementId { get; set; }
            public string EndorsementsName { get; set; }
            public string Title { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class DataEndorsement
        {
            public int EndorsementId { get; set; }
            public List<int> EmployeeId { get; set; }

            public int EmployeeTypeID { get; set; }
            public string FullName { get; set; }
            public int BadgeId { get; set; }
            public int EndorsementTypeId { get; set; }
            public string EndorsementsType { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public int RoleId { get; set; }

            public string RoleType { get; set; }
            public string BadgeName { get; set; }
        }

        public class EndorsementData
        {
            public int EndorsementId { get; set; }
            public int EmployeeId { get; set; }

            public int EmployeeTypeID { get; set; }
            public string FullName { get; set; }
            public int BadgeId { get; set; }
            public int EndorsementTypeId { get; set; }
            public DateTime UpdatedDate { get; set; }
            public DateTime CreatedDate { get; set; }

            //  public int RoleId { get; set; }
            public string Endorsements { get; set; }

            public DateTime Date { get; set; }
            public string RoleType { get; set; }
            public string BadgeName { get; set; }
            public string ImageUrl { get; set; }
            public string Comments { get; set; }
            public int BadgeType { get; set; }
            public string DesignationName { get; set; }
            public string Title { get; set; }
            public string EndorsementsName { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// Created by SurajBundel on 10-06-2022
        /// Model created for static json
        /// </summary>
        public class BadgesResponseModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
        }

        #endregion This are Used For ResponseHelper Class
    }
}