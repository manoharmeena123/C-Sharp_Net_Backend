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
    /// Created By Ankit Jain on 22-09-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/frequentlyaskquestions")]
    public class FrequentlyAskedQuestionsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();


        #region This Are Use For Crud In Faq Categories and Question And Ans Super Admin

        #region This Api Use To Add Frequently Asked Questions Categories
        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addfaqcategories
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addfaqcategories")]
        public async Task<ResponseBodyModel> AddFaqCategories(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                FAQCategories faqobj = new FAQCategories
                {
                    CategoriesName = model.CategoriesName,
                    CategoriesDescription = model.CategoriesDescription,
                    CreatedBy = 0,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now
                };
                _db.FAQCategoriess.Add(faqobj);
                await _db.SaveChangesAsync();

                res.Message = "FAQ Categories Added";
                res.Status = true;
                res.Data = faqobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region This Api Use To Get All FAQ Categories

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfapcategories
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfapcategories")]
        public async Task<ResponseBodyModel> GetAllFaqCategories()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x =>
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == 0 && x.OrgId == 0).ToListAsync();
                if (faqcategoriesdata.Count != 0)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapcategories
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapcategories")]
        public async Task<ResponseBodyModel> UpdateFaqcategories(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FAQCategoriesId == model.FAQCategoriesId && x.CompanyId == 0 && x.OrgId == 0)
                        .FirstOrDefaultAsync();
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.CategoriesName = model.CategoriesName;
                    faqcategoriesdata.CategoriesDescription = model.CategoriesDescription;
                    faqcategoriesdata.UpdatedBy = 0;
                    faqcategoriesdata.UpdatedOn = DateTime.Now;
                    faqcategoriesdata.IsActive = true;
                    faqcategoriesdata.IsDeleted = false;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Updated";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Remove FAQ Categories
        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcategories
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcategories")]
        public async Task<ResponseBodyModel> DeleteFaqCategories(int faqCategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.FirstOrDefaultAsync(x =>
                        x.FAQCategoriesId == faqCategoriesId && x.IsDeleted == false && x.IsActive == true && x.CompanyId == 0 && x.OrgId == 0);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Add Frequently Asked Questions and Answer
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addquesionsans
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addquesionsans")]
        public async Task<ResponseBodyModel> AddQuestions(AddQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {

                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                {
                    FAQCategoriesQAns Faqobj = new FAQCategoriesQAns
                    {
                        FAQCategoriesId = model.FAQCategoriesId,
                        FaqQuesions = model.FaqQuesions,
                        FaqAns = model.FaqAns,
                        FaqTags = model.FaqTags,
                        IsPublish = model.IsPublish,
                        Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft,
                        CreatedBy = 0,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                        //FaqImage = imageUrls,
                    };

                    _db.FAQCategoriesQAnss.Add(Faqobj);
                    await _db.SaveChangesAsync();

                    res.Message = "Added Questions And Ans";
                    res.Status = true;
                    res.Data = Faqobj;
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

        #region This Api Use To Get Frequently Questions and Answer

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsans
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsans")]
        public async Task<ResponseBodyModel> GetAllQuesionsAns()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //List<CategoriesImages> List = new List<CategoriesImages>();
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x => x.IsActive
                && !x.IsDeleted && x.CompanyId == 0 && x.OrgId == 0);
                if (fAQCategoriesdata != null)
                {
                    var response = new AddQuesionsAns
                    {
                        FAQCategoriesId = fAQCategoriesdata.FAQCategoriesId,
                        FaqQuesions = fAQCategoriesdata.FaqQuesions,
                        FaqAns = fAQCategoriesdata.FaqAns,
                        FaqTags = fAQCategoriesdata.FaqTags,
                        IsPublish = fAQCategoriesdata.IsPublish,
                        //FaqImages = fAQCategoriesdata.FaqImage.Split(',')
                        //        .Select(x => new CategoriesImages
                        //        {
                        //            FaqImage = x,
                        //        }).ToList(),
                    };
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = response;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region This Api Use To Get Frequently Questions and Answer By Id

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsansbyId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsansbyId")]
        public async Task<ResponseBodyModel> GetAllQuestionsAnsById(int faqcategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.FAQCategoriesId == faqcategoriesId
                && x.IsActive && !x.IsDeleted && x.Status == AboutUsStatusConstants.Publish && x.CompanyId == 0 && x.OrgId == 0).ToListAsync();

                if (fAQCategoriesdata != null)
                {
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = fAQCategoriesdata;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region Api's For Get FAQ Questions and Ans Draft Data
        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getalldraftdata
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getalldraftdata")]
        public async Task<ResponseBodyModel> GetAllDraftData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqDraft = await _db.FAQCategoriesQAnss.Where(x => x.IsActive == true && x.IsDeleted == false
                               && x.Status == AboutUsStatusConstants.Draft && x.CompanyId == 0 && x.OrgId == 0).ToListAsync();

                if (faqDraft.Count > 0)
                {
                    res.Message = "FAQ Questions And Ans Data Found !";
                    res.Status = true;
                    res.Data = faqDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = faqDraft;
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

        #region This Api Use To Remove FAQ Categories Questions And Answer
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcquesionsans
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcquesionsans")]
        public async Task<ResponseBodyModel> DeleteFaqQuesionsans(int faqcategoriesQAnsId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x => x.FaqCategoriesQAnsId == faqcategoriesQAnsId
                && x.IsDeleted == false && x.IsActive == true && x.Status == AboutUsStatusConstants.Publish && x.CompanyId == 0 && x.OrgId == 0);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions and Ans Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Questions and Ans Not Found";
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

        #endregion

        #region This Api Use To Remove FAQ Categories Questions And Answer Draft
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcquesionsansdraft
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcquesionsansdraft")]
        public async Task<ResponseBodyModel> DeleteFaqQuesionsansDraft(int faqcategoriesQAnsId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x => x.FaqCategoriesQAnsId == faqcategoriesQAnsId
                && x.IsDeleted == false && x.IsActive == true && x.Status == AboutUsStatusConstants.Draft && x.CompanyId == 0 && x.OrgId == 0);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions and Ans Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Questions and Ans Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories Questions and Ans

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapquesionsans
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapquesionsans")]
        public async Task<ResponseBodyModel> UpdateFaqQuesionAns(UpdateQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FaqCategoriesQAnsId == model.FaqCategoriesQAnsId && x.CompanyId == 0 && x.OrgId == 0)
                        .FirstOrDefaultAsync();
                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.FAQCategoriesId = model.FAQCategoriesId;
                    faqcategoriesdata.FaqQuesions = model.FaqQuesions;
                    faqcategoriesdata.FaqAns = model.FaqAns;
                    faqcategoriesdata.FaqTags = model.FaqTags;
                    faqcategoriesdata.IsPublish = model.IsPublish;
                    faqcategoriesdata.Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft;
                    faqcategoriesdata.UpdatedBy = 0;
                    faqcategoriesdata.UpdatedOn = DateTime.Now;
                    faqcategoriesdata.IsActive = true;
                    faqcategoriesdata.IsDeleted = false;
                    //faqcategoriesdata.FaqImage = imageUrls;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions And Ans Updated";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Quesions And Ans Not Found";
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

        #endregion

        #endregion Super Admin

        #region This Api Use For Admin

        #region This Api Use To Add Frequently Asked Questions Categories in Admin
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addfaqcategoriesadmin
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addfaqcategoriesadmin")]
        public async Task<ResponseBodyModel> AddFaqCategoriesAdmin(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                FAQCategories faqobj = new FAQCategories
                {
                    CategoriesName = model.CategoriesName,
                    CategoriesDescription = model.CategoriesDescription,
                    CreatedBy = claims.employeeId,
                    CreatedByName = claims.displayName,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CompanyId = claims.companyId
                };
                _db.FAQCategoriess.Add(faqobj);
                await _db.SaveChangesAsync();

                res.Message = "FAQ Categories Added";
                res.Status = true;
                res.Data = faqobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region This Api Use To Get All FAQ Categories admin

        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfapcategoriesadmin
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfapcategoriesadmin")]
        public async Task<ResponseBodyModel> GetAllFaqCategoriesAdmin()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x =>
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == 0).ToListAsync();
                if (faqcategoriesdata.Count != 0)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Get All FAQ Categories admin By Id

        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfapcategoriesadminbyid
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfapcategoriesadminbyid")]
        public async Task<ResponseBodyModel> GetAllFaqCategoriesAdminById(int faqcategoriesId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x => x.FAQCategoriesId == faqcategoriesId &&
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId).ToListAsync();
                if (faqcategoriesdata != null)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories admin

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapcategoriesadmin
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapcategoriesadmin")]
        public async Task<ResponseBodyModel> UpdateFaqcategoriesadmin(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FAQCategoriesId == model.FAQCategoriesId && x.CompanyId == claims.companyId)
                        .FirstOrDefaultAsync();
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.CategoriesName = model.CategoriesName;
                    faqcategoriesdata.CategoriesDescription = model.CategoriesDescription;
                    faqcategoriesdata.UpdatedBy = claims.employeeId;
                    faqcategoriesdata.UpdateByName = claims.displayName;
                    faqcategoriesdata.UpdatedOn = DateTime.Now;
                    faqcategoriesdata.IsActive = true;
                    faqcategoriesdata.IsDeleted = false;
                    faqcategoriesdata.CompanyId = claims.companyId;
                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Updated";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Remove FAQ Categories admin
        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcategoriesadmin
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcategoriesadmin")]
        public async Task<ResponseBodyModel> DeleteFaqCategoriesAdmin(int faqCategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.FirstOrDefaultAsync(x => x.FAQCategoriesId == faqCategoriesId &&
                x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Add Frequently Asked Questions and Answer admin
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addquesionsansadmin
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addquesionsansadmin")]
        public async Task<ResponseBodyModel> AddQuesionsAdmin(AddQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model.HrAdded == true)
                {
                    FAQCategoriesQAns Faqhraddeed = new FAQCategoriesQAns
                    {
                        FAQCategoriesId = model.FAQCategoriesId,
                        FaqQuesions = model.FaqQuesions,
                        FaqAns = model.FaqAns,
                        FaqTags = model.FaqTags,
                        //FaqImage = imageUrls,
                        Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft,
                        IsPublish = model.IsPublish,
                        HrAdded = model.HrAdded,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        CreatedBy = claims.employeeId,
                        CreatedByName = claims.displayName,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                    };
                    _db.FAQCategoriesQAnss.Add(Faqhraddeed);
                    await _db.SaveChangesAsync();

                    res.Message = "Added Questions And Ans";
                    res.Status = true;
                    res.Data = Faqhraddeed;
                }
                else
                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                {
                    FAQCategoriesQAns Faqobj = new FAQCategoriesQAns
                    {
                        FAQCategoriesId = model.FAQCategoriesId,
                        FaqQuesions = model.FaqQuesions,
                        FaqAns = model.FaqAns,
                        FaqTags = model.FaqTags,
                        //FaqImage = imageUrls,
                        Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft,
                        IsPublish = model.IsPublish,
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                    };

                    _db.FAQCategoriesQAnss.Add(Faqobj);
                    await _db.SaveChangesAsync();

                    res.Message = "Added Questions And Ans";
                    res.Status = true;
                    res.Data = Faqobj;
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

        #region This Api Use To Get Frequently Questions and Answer Admin

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsansadmin
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsansadmin")]
        public async Task<ResponseBodyModel> GetAllDocumentAdmin()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //List<CategoriesImages> List = new List<CategoriesImages>();
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId);
                if (fAQCategoriesdata != null)
                {
                    var response = new AddQuesionsAns
                    {
                        FAQCategoriesId = fAQCategoriesdata.FAQCategoriesId,
                        FaqQuesions = fAQCategoriesdata.FaqQuesions,
                        FaqAns = fAQCategoriesdata.FaqAns,
                        FaqTags = fAQCategoriesdata.FaqTags,
                        IsPublish = fAQCategoriesdata.IsPublish,
                        //FaqImages = fAQCategoriesdata.FaqImage.Split(',')
                        //        .Select(x => new CategoriesImages
                        //        {
                        //            FaqImage = x,
                        //        }).ToList(),
                    };
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = response;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region This Api Use To Get Frequently Questions and Answer By Id admin

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsansadminbyId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsansadminbyId")]
        public async Task<ResponseBodyModel> GetAllQuestionsAnsAdminById(int faqcategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.FAQCategoriesId == faqcategoriesId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.Status == AboutUsStatusConstants.Publish).ToListAsync();

                if (fAQCategoriesdata != null)
                {
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = fAQCategoriesdata;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region Api's For Get FAQ Questions and Ans Draft Data Admin
        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getalladmindraftdata
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getalladmindraftdata")]
        public async Task<ResponseBodyModel> GetallAdminDraftData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqDraft = await _db.FAQCategoriesQAnss.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                x.Status == AboutUsStatusConstants.Draft && x.CompanyId == claims.companyId && x.OrgId == 0).ToListAsync();

                if (faqDraft.Count > 0)
                {
                    res.Message = "FAQ Quesions And Ans Data Found !";
                    res.Status = true;
                    res.Data = faqDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = faqDraft;
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

        #region This Api Use To Remove FAQ Categories Questions And Answer admin
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcquesionsansadmin
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcquesionsansadmin")]
        public async Task<ResponseBodyModel> DeleteFaqQuesionsansAdmin(int faqcategoriesQAnsId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x =>
                        x.FaqCategoriesQAnsId == faqcategoriesQAnsId && x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions and Ans Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Questions and Ans Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories Admin

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapquesionsansadmin
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapquesionsansadmin")]
        public async Task<ResponseBodyModel> UpdateFaqQuesionAnsAdmin(UpdateQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FaqCategoriesQAnsId == model.FaqCategoriesQAnsId && x.CompanyId == claims.companyId)
                        .FirstOrDefaultAsync();
                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                if (faqcategoriesdata != null)
                {
                    if (model.HrAdded == true)
                    {
                        faqcategoriesdata.FAQCategoriesId = model.FAQCategoriesId;
                        faqcategoriesdata.FaqQuesions = model.FaqQuesions;
                        faqcategoriesdata.FaqAns = model.FaqAns;
                        faqcategoriesdata.FaqTags = model.FaqTags;
                        faqcategoriesdata.IsPublish = model.IsPublish;
                        faqcategoriesdata.HrAdded = model.HrAdded;
                        faqcategoriesdata.Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft;
                        faqcategoriesdata.CompanyId = claims.companyId;
                        faqcategoriesdata.UpdatedBy = claims.employeeId;
                        faqcategoriesdata.UpdatedOn = DateTime.Now;
                        faqcategoriesdata.OrgId = claims.orgId;
                        faqcategoriesdata.UpdatedByname = claims.displayName;
                        faqcategoriesdata.IsActive = true;
                        faqcategoriesdata.IsDeleted = false;
                        //faqcategoriesdata.FaqImage = imageUrls;

                        _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "FAQ Categories Questions And Ans Updated";
                        res.Status = true;
                        res.Data = faqcategoriesdata;
                    }
                    else
                    {
                        faqcategoriesdata.FAQCategoriesId = model.FAQCategoriesId;
                        faqcategoriesdata.FaqQuesions = model.FaqQuesions;
                        faqcategoriesdata.FaqAns = model.FaqAns;
                        faqcategoriesdata.FaqTags = model.FaqTags;
                        faqcategoriesdata.IsPublish = model.IsPublish;
                        faqcategoriesdata.Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft;
                        faqcategoriesdata.CompanyId = claims.companyId;
                        faqcategoriesdata.UpdatedBy = claims.employeeId;
                        faqcategoriesdata.UpdatedByname = claims.displayName;
                        faqcategoriesdata.UpdatedOn = DateTime.Now;
                        faqcategoriesdata.IsActive = true;
                        faqcategoriesdata.IsDeleted = false;
                        //faqcategoriesdata.FaqImage = imageUrls;

                        _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "FAQ Categories Questions And Ans Updated";
                        res.Status = true;
                        res.Data = faqcategoriesdata;
                    }
                }
                else
                {
                    res.Message = "FAQ Categories Questions And Ans Not Found";
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

        #endregion

        #region Api's For Get FAQ Questions and Ans Draft Data HR && Admin
        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getalladmindraftdatahradmin
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getalladmindraftdatahradmin")]
        public async Task<ResponseBodyModel> GetallAdminDraftDataHRAdmin()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqDraft = await _db.FAQCategoriesQAnss.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                x.Status == AboutUsStatusConstants.Draft && x.CompanyId == claims.companyId /*&& x.HrAdded == true*/).ToListAsync();

                if (faqDraft.Count > 0)
                {
                    res.Message = "FAQ Questions And Ans Data Found !";
                    res.Status = true;
                    res.Data = faqDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = faqDraft;
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

        //#region This api use for upload documents for faq document admin

        ///// <summary>
        /////Created By Ankit On 26-09-2022
        ///// </summary>route api/frequentlyaskquestions/uploadfaqdocumentsadmin
        ///// <returns></returns>
        //[HttpPost]
        //[Route("uploadfaqdocumentsadmin")]
        //public async Task<UploadFaqDoc> UploadPreboardDocmentsAdmin()
        //{
        //    UploadFaqDoc result = new UploadFaqDoc();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
        //        var data = Request.Content.IsMimeMultipartContent();
        //        if (Request.Content.IsMimeMultipartContent())
        //        {
        //            //fileList f = new fileList();
        //            var provider = new MultipartMemoryStreamProvider();
        //            await Request.Content.ReadAsMultipartAsync(provider);
        //            if (provider.Contents.Count > 0)
        //            {
        //                var filefromreq = provider.Contents[0];
        //                Stream _id = filefromreq.ReadAsStreamAsync().Result;
        //                StreamReader reader = new StreamReader(_id);
        //                string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

        //                string extemtionType = Helper.MimeType.GetContentType(filename).Split('/').First();

        //                string extension = System.IO.Path.GetExtension(filename);
        //                string Fileresult = filename.Substring(0, filename.Length - extension.Length);
        //                byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
        //                //f.byteArray = buffer;
        //                string mime = filefromreq.Headers.ContentType.ToString();
        //                Stream stream = new MemoryStream(buffer);
        //                var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Faqcategoriesdocument/" + claims.employeeid), dates + '.' + filename);
        //                string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeid + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeid;

        //                //for create new Folder
        //                DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
        //                if (!objDirectory.Exists)
        //                {
        //                    Directory.CreateDirectory(DirectoryURL);
        //                }
        //                //string path = "UploadImages\\" + compid + "\\" + filename;

        //                string path = "uploadimage\\Faqcategoriesdocument\\" + claims.employeeid + "\\" + dates + '.' + Fileresult + extension;

        //                File.WriteAllBytes(FileUrl, buffer.ToArray());
        //                result.Message = "Successful";
        //                result.Status = true;
        //                result.URL = FileUrl;
        //                result.Path = path;
        //                result.Extension = extension;
        //                result.ExtensionType = extemtionType;
        //            }
        //            else
        //            {
        //                result.Message = "You Pass 0 Content";
        //                result.Status = false;
        //            }
        //        }
        //        else
        //        {
        //            result.Message = "Error";
        //            result.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.Status = false;
        //    }
        //    return result;
        //}

        //#endregion This api use for upload documents for preaboarding document

        #endregion Admin In FAQ

        #region This Are use for crud in Faq Categories And Questionand Ans Hr

        #region This Api Use To Add Frequently Asked Questions Categories in Hr
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addfaqcategorieshr
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addfaqcategorieshr")]
        public async Task<ResponseBodyModel> AddFaqCategoriesHr(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                FAQCategories faqobj = new FAQCategories
                {
                    CategoriesName = model.CategoriesName,
                    CategoriesDescription = model.CategoriesDescription,
                    CreatedBy = claims.employeeId,
                    CreatedByName = claims.displayName,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId
                };
                _db.FAQCategoriess.Add(faqobj);
                await _db.SaveChangesAsync();

                res.Message = "FAQ Categories Added";
                res.Status = true;
                res.Data = faqobj;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region This Api Use To Get All FAQ Categories HR

        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfapcategorieshr
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfapcategorieshr")]
        public async Task<ResponseBodyModel> GetAllFaqCategoriesHr()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x =>
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (faqcategoriesdata.Count != 0)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories HR

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapcategorieshr
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapcategorieshr")]
        public async Task<ResponseBodyModel> UpdateFaqcategorieshr(AddCategories model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FAQCategoriesId == model.FAQCategoriesId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .FirstOrDefaultAsync();
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.CategoriesName = model.CategoriesName;
                    faqcategoriesdata.CategoriesDescription = model.CategoriesDescription;
                    faqcategoriesdata.UpdatedBy = claims.employeeId;
                    faqcategoriesdata.UpdateByName = claims.displayName;
                    faqcategoriesdata.UpdatedOn = DateTime.Now;
                    faqcategoriesdata.IsActive = true;
                    faqcategoriesdata.IsDeleted = false;
                    faqcategoriesdata.CompanyId = claims.companyId;
                    faqcategoriesdata.OrgId = claims.orgId;
                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Updated";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Remove FAQ Categories HR
        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcategorieshr
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcategorieshr")]
        public async Task<ResponseBodyModel> DeleteFaqCategoriesHr(int faqCategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.FirstOrDefaultAsync(x =>
                        x.FAQCategoriesId == faqCategoriesId && x.IsDeleted == false && x.OrgId == claims.orgId
                        && x.IsActive == true && x.CompanyId == claims.companyId);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Add Frequently Asked Questions and Answer HR
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Post >> api/frequentlyaskquestions/addquesionsanshr
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [Route("addquesionsanshr")]
        public async Task<ResponseBodyModel> AddQuesionsHR(AddQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                {
                    FAQCategoriesQAns Faqobj = new FAQCategoriesQAns
                    {
                        FAQCategoriesId = model.FAQCategoriesId,
                        FaqQuesions = model.FaqQuesions,
                        FaqAns = model.FaqAns,
                        FaqTags = model.FaqTags,
                        Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft,
                        //FaqImage = imageUrls,
                        IsPublish = model.IsPublish,
                        CompanyId = claims.companyId,
                        CreatedBy = claims.employeeId,
                        CreatedByName = claims.displayName,
                        OrgId = claims.orgId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                    };

                    _db.FAQCategoriesQAnss.Add(Faqobj);
                    await _db.SaveChangesAsync();

                    res.Message = "Added Questions And Ans";
                    res.Status = true;
                    res.Data = Faqobj;
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

        #region This Api Use To Get Frequently Questions and Answer HR

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsanshr
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsanshr")]
        public async Task<ResponseBodyModel> GetAllDataHr()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //List<CategoriesImages> List = new List<CategoriesImages>();
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted
                && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (fAQCategoriesdata != null)
                {
                    var response = new AddQuesionsAns
                    {
                        FAQCategoriesId = fAQCategoriesdata.FAQCategoriesId,
                        FaqQuesions = fAQCategoriesdata.FaqQuesions,
                        FaqAns = fAQCategoriesdata.FaqAns,
                        FaqTags = fAQCategoriesdata.FaqTags,
                        IsPublish = fAQCategoriesdata.IsPublish,
                        //FaqImages = fAQCategoriesdata.FaqImage.Split(',')
                        //        .Select(x => new CategoriesImages
                        //        {
                        //            FaqImage = x,
                        //        }).ToList(),
                    };
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = response;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region This Api Use To Get Frequently Questions and Answer By Id HR

        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getallquestionsanshrbyId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallquestionsanshrbyId")]
        public async Task<ResponseBodyModel> GetAllQuestionsAnsHrById(int faqcategoriesId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var fAQCategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.FAQCategoriesId == faqcategoriesId
                && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.OrgId == claims.orgId && x.IsPublish == true).ToListAsync();

                if (fAQCategoriesdata != null)
                {
                    res.Message = "FAQ Categories Questions and Ans Found";
                    res.Status = true;
                    res.Data = fAQCategoriesdata;
                }

                else
                {
                    res.Message = "No FAQ Categories Questions and Ans Found";
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

        #region Api's For Get FAQ Questions and Ans Draft Data HR
        /// <summary>
        /// Create By Ankit Jain Date-26-09-2022
        /// Api >> Get >> api/frequentlyaskquestions/getalladmindraftdatahr
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getalladmindraftdatahr")]
        public async Task<ResponseBodyModel> GetallAdminDraftDataHR()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqDraft = await _db.FAQCategoriesQAnss.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                x.Status == AboutUsStatusConstants.Draft && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();

                if (faqDraft.Count > 0)
                {
                    res.Message = "FAQ Questions And Ans Data Found !";
                    res.Status = true;
                    res.Data = faqDraft;
                }
                else
                {
                    res.Message = "No Data Found ! ";
                    res.Status = false;
                    res.Data = faqDraft;
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

        #region This Api Use To Remove FAQ Categories Questions And Answer HR
        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Delete >> api/frequentlyaskquestions/deletefaqcquesionsanshr
        /// </summary>
        /// <param name="model"></param>

        [HttpDelete]
        [Route("deletefaqcquesionsanshr")]
        public async Task<ResponseBodyModel> DeleteFaqQuesionsansHr(int faqcategoriesQAnsId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.FirstOrDefaultAsync(x =>
                        x.FaqCategoriesQAnsId == faqcategoriesQAnsId && x.IsDeleted == false && x.IsActive == true
                        && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.IsActive = false;
                    faqcategoriesdata.IsDeleted = true;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions and Ans Remove";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Questions and Ans Not Found";
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

        #endregion

        #region This Api Use To Edit FAQ Categories HR

        /// <summary>
        /// Crated By Ankit Jain On 22-09-2022
        /// API >> Put >> api/frequentlyaskquestions/editfapquesionsanshr
        /// </summary>
        /// <param name="model"></param>
        [HttpPut]
        [Route("editfapquesionsanshr")]
        public async Task<ResponseBodyModel> UpdateFaqQuesionAnsHr(UpdateQuesionsAns model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.IsDeleted == false &&
                        x.IsActive == true && x.FaqCategoriesQAnsId == model.FaqCategoriesQAnsId && x.CompanyId == claims.companyId && x.OrgId == claims.orgId)
                        .FirstOrDefaultAsync();
                //var imageUrls = model.FaqImages.Count > 0 ? String.Join(",", model.FaqImages.Select(x => x.FaqImage).ToList()) : null;
                if (faqcategoriesdata != null)
                {
                    faqcategoriesdata.FAQCategoriesId = model.FAQCategoriesId;
                    faqcategoriesdata.FaqQuesions = model.FaqQuesions;
                    faqcategoriesdata.FaqAns = model.FaqAns;
                    faqcategoriesdata.FaqTags = model.FaqTags;
                    faqcategoriesdata.IsPublish = model.IsPublish;
                    faqcategoriesdata.Status = model.IsPublish == true ? AboutUsStatusConstants.Publish : AboutUsStatusConstants.Draft;
                    faqcategoriesdata.CompanyId = claims.companyId;
                    faqcategoriesdata.OrgId = claims.orgId;
                    faqcategoriesdata.UpdatedBy = claims.employeeId;
                    faqcategoriesdata.UpdatedByname = claims.displayName;
                    faqcategoriesdata.UpdatedOn = DateTime.Now;
                    faqcategoriesdata.IsActive = true;
                    faqcategoriesdata.IsDeleted = false;
                    //faqcategoriesdata.FaqImage = imageUrls;

                    _db.Entry(faqcategoriesdata).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "FAQ Categories Questions And Ans Updated";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Questions And Ans Not Found";
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

        #endregion

        #endregion

        #region This Api Use For FAQ Question And ans Get Read And Send Response

        #region This Api Use To Get All FAQ Categories admin And Hr Get By User

        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfapcategoriesuser
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfapcategoriesuser")]
        public async Task<ResponseBodyModel> GetAllFaqCategoriesUser()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriess.Where(x =>
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId).ToListAsync();
                if (faqcategoriesdata.Count != 0)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #region This Api Use To Get All FAQ Categories Questions And Ans admin And Hr Get By User

        /// <summary>
        /// Crated By Ankit Jain On 26-09-2022
        /// API >> Get >> api/frequentlyaskquestions/getfaqQuestionsAnsUser
        /// </summary>
        /// <param name="model"></param>
        [HttpGet]
        [Route("getfaqQuestionsAnsUser")]
        public async Task<ResponseBodyModel> GetFapQuestionsAnsUser(int faqcategoriesId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var faqcategoriesdata = await _db.FAQCategoriesQAnss.Where(x => x.FAQCategoriesId == faqcategoriesId &&
                         x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId && x.Status == AboutUsStatusConstants.Publish).ToListAsync();
                if (faqcategoriesdata.Count != 0)
                {
                    res.Message = "FAQ Categories list Found";
                    res.Status = true;
                    res.Data = faqcategoriesdata;
                }
                else
                {
                    res.Message = "FAQ Categories Not Found";
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

        #endregion

        #endregion

        #region Helper Model Class

        /// <summary>
        /// Created By Ankit on 26-09-2022
        /// </summary>
        public class AddCategories
        {
            public int FAQCategoriesId { get; set; }
            public string CategoriesName { get; set; }
            public string CategoriesDescription { get; set; }
        }


        /// <summary>
        /// Created By Ankit on 26-09-2022
        /// </summary>
        public class AddCategoriesadmin
        {
            public int FAQCategoriesId { get; set; }
            public string CategoriesName { get; set; }
            public string CategoriesDescription { get; set; }
            public string FaqQuesions { get; set; }
            public string FaqAns { get; set; }
            public bool IsPublish { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 26-09-2022
        /// </summary>
        public class AddQuesionsAns
        {
            public int FAQCategoriesId { get; set; }
            public string FaqQuesions { get; set; }
            public string FaqAns { get; set; }
            //public List<CategoriesImages> FaqImages { get; set; }
            public string FaqTags { get; set; }
            public bool IsPublish { get; set; }
            public bool HrAdded { get; set; }
        }

        /// <summary>
        /// Created By Ankit on 26-09-2022
        /// </summary>
        public class UpdateQuesionsAns
        {
            public int FaqCategoriesQAnsId { get; set; }
            public int FAQCategoriesId { get; set; }
            public string FaqQuesions { get; set; }
            public string FaqAns { get; set; }
            //public List<CategoriesImages> FaqImages { get; set; }
            public string FaqTags { get; set; }
            public bool IsPublish { get; set; }
            public bool HrAdded { get; set; }
        }




        ///// <summary>
        ///// Created By Ankit on 26-09-2022
        ///// </summary>

        //public class UploadFaqDoc
        //{
        //    public string Message { get; set; }
        //    public bool Status { get; set; }
        //    public string URL { get; set; }
        //    public string Path { get; set; }
        //    public string Extension { get; set; }
        //    public string ExtensionType { get; set; }
        //}
        #endregion
    }
}
