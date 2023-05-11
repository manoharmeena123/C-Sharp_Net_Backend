using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra on 10-02-2022
    /// </summary>
    //[Authorize]
    [RoutePrefix("api/docmaster")]
    public class DocumentMastersController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region Api To Add Document Types

        /// <summary>
        /// API >> Post >> api/docmaster/adddoctype
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("adddoctype")]
        public async Task<ResponseBodyModel> AdddDocType(DocTypeMaster model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                DocTypeMaster docType = new DocTypeMaster();
                docType.DocName = model.DocName;
                docType.DocType = model.DocType;
                docType.IsActive = true;
                docType.IsDelete = false;
                docType.CreateDate = DateTime.Now;
                _db.DocumentTypes.Add(docType);
                await _db.SaveChangesAsync();

                res.Message = "Document Type Added";
                res.Status = true;
                res.Data = docType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Document Types

        #region Api To Get Document Type List

        /// <summary>
        /// API >> Get >> api/docmaster/alldoctypelist
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("alldoctypelist")]
        public async Task<ResponseBodyModel> GetAllDocumentType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var doclist = await _db.DocumentTypes.ToListAsync();
                if (doclist.Count > 0)
                {
                    res.Message = "Document List";
                    res.Status = true;
                    res.Data = doclist;
                }
                else
                {
                    res.Message = "Document List Is Empty";
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

        #endregion Api To Get Document Type List

        #region Api To Get All Active Document Type List

        /// <summary>
        /// API >> Get >> api/docmaster/getactivedoctype
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getactivedoctype")]
        public async Task<ResponseBodyModel> GetAllActiveDocumentTypes()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var docType = await _db.DocumentTypes.Where(x => x.IsDelete == false && x.IsActive == true).ToListAsync();
                if (docType.Count > 0)
                {
                    res.Message = "Active Document List";
                    res.Status = true;
                    res.Data = docType;
                }
                else
                {
                    res.Message = "Document List Is Empty";
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

        #endregion Api To Get All Active Document Type List

        #region Api To Get Document Type By Id

        /// <summary>
        /// API >> Get >> api/docmaster/getdoctypebyid
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <param name="docTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdoctypebyid")]
        public async Task<ResponseBodyModel> GetDocumentTypeById(int docTypeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var doctype = await _db.DocumentTypes.FirstOrDefaultAsync(x => x.DocTypeId == docTypeId);
                if (doctype != null)
                {
                    res.Message = "Document Type By Id";
                    res.Status = true;
                    res.Data = doctype;
                }
                else
                {
                    res.Message = "Document Type Not Found";
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

        #endregion Api To Get Document Type By Id

        #region Api To Edit Document Type

        /// <summary>
        /// API >> Put >> api/docmaster/editdoctype
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editdoctype")]
        public async Task<ResponseBodyModel> EditDocumentType(DocTypeMaster model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var docType = await _db.DocumentTypes.FirstOrDefaultAsync(x => x.DocTypeId == model.DocTypeId);
                if (docType != null)
                {
                    docType.DocName = model.DocName;
                    docType.DocType = model.DocType;
                    docType.UpdateDate = DateTime.Now;

                    _db.Entry(docType).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Document Type Edited";
                    res.Status = true;
                    res.Data = docType;
                }
                else
                {
                    res.Message = "Document Type Not Found";
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

        #endregion Api To Edit Document Type

        #region Api To Add Required Document For Per Candidate

        /// <summary>
        /// API >>  >> api/docmaster/addrecdocument
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addrecdocument")]
        public async Task<ResponseBodyModel> AddRequiredDocumentForCandidate(GetAddRequiredDocumentModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId);
                if (candidate != null)
                {
                    var requiredDocMaster = _db.RequiredDocMaster.FirstOrDefault(x => x.CandidateId == candidate.CandidateId);
                    if (requiredDocMaster == null)
                    {
                        RequiredDocMaster reqs = new RequiredDocMaster()
                        {
                            CandidateId = candidate.CandidateId,
                            IsActive = true,
                            IsDelete = false,
                            CreateDate = DateTime.Now,
                        };
                        _db.RequiredDocMaster.Add(reqs);
                        _db.SaveChanges();
                        requiredDocMaster = reqs;
                    }
                    var count = _db.RequiredDocs.Where(x => x.ReqDocMasterId == requiredDocMaster.ReqDocMasterId).ToList().Count;
                    if (count == 0)
                    {
                        if (model.DocHead.Count > 0)
                        {
                            foreach (var doc in model.DocHead)
                            {
                                var i = (int)Enum.Parse(typeof(DocType), doc.Name.Replace(' ', '_'));
                                foreach (var d in doc.DocList)
                                {
                                    if (d.IsRequired)
                                    {
                                        var docType = _db.DocumentTypes.FirstOrDefault(x => x.DocTypeId == d.DocTypeId);
                                        if (docType != null)
                                        {
                                            RequiredDocuments recdoc = new RequiredDocuments();
                                            recdoc.CandidateId = candidate.CandidateId;
                                            recdoc.DocTypeId = docType.DocTypeId;
                                            recdoc.ReqDocMasterId = requiredDocMaster.ReqDocMasterId;
                                            recdoc.DocumentName = docType.DocName;
                                            recdoc.DocumentStatus = Enum.GetName(typeof(DocumentStatus), DocumentStatus.Not_Uploaded).Replace('_', ' ');
                                            recdoc.DocType = i;
                                            //recdoc.Reason = candidate.Reason;
                                            recdoc.IsActive = true;
                                            recdoc.IsDelete = false;
                                            recdoc.CreateDate = DateTime.Now;
                                            _db.RequiredDocs.Add(recdoc);
                                            await _db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            candidate.PrebordingStages = PreboardingStages.Verfiy_Info;
                            _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();

                            res.Message = "Added";
                            res.Status = true;
                            res.Data = model;
                        }
                        else
                        {
                            res.Message = "You Pass Empty List";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "You Already Addred Requirment";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api To Add Required Document For Per Candidate






        #region Api To Get Required DocumentType For Requirement

        /// <summary>
        /// API >> Get >> api/docmaster/getreqdoc
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getreqdoc")]
        public async Task<ResponseBodyModel> GetRequiredDocumentType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetAddRequiredDocumentModel obj = new GetAddRequiredDocumentModel();
            try
            {
                var docreqList = await _db.DocumentTypes.Where(x => x.IsDelete == false && x.IsActive == true).ToListAsync();
                var doclist = Enum.GetValues(typeof(DocType));

                List<DocTypeHead> docTypeHeadList = new List<DocTypeHead>();
                foreach (var d in doclist)
                {
                    var i = Convert.ToInt32(d);
                    DocTypeHead docHead = new DocTypeHead();
                    docHead.Name = Enum.GetName(typeof(DocType), d).Replace('_', ' ');

                    List<ReqDocList> reqDocList = new List<ReqDocList>();
                    foreach (var doc in docreqList)
                    {
                        if (doc.DocType == i)
                        {
                            ReqDocList resList = new ReqDocList()
                            {
                                DocTypeId = doc.DocTypeId,
                                DocTypeName = doc.DocName,
                                IsRequired = false,
                            };
                            reqDocList.Add(resList);
                        }
                    }
                    docHead.DocList = reqDocList;
                    docTypeHeadList.Add(docHead);
                }
                obj.CandidateId = 0;
                obj.DocHead = docTypeHeadList;

                res.Message = "Get Recuired Document Type";
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

        #endregion Api To Get Required DocumentType For Requirement

        #region Api To Add and Update Documents For Candidates

        /// <summary>
        /// API >> Get >> api/docmaster/addupdatecandicatedoc
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("addupdatecandicatedoc")]
        public async Task<ResponseBodyModel> AddUpdateCandidateDocument(CandidateDocuments model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (model != null)
                {
                    var candidate = _db.Candidates.FirstOrDefault(x => x.CandidateId == model.CandidateId);
                    if (candidate != null)
                    {
                        if (model.DocTypeId != 0)
                        {
                            var reqDocType = _db.RequiredDocs.FirstOrDefault(x => x.CandidateId == candidate.CandidateId && x.DocTypeId == model.DocTypeId);
                            if (reqDocType != null)
                            {
                                reqDocType.DocumentStatus = Enum.GetName(typeof(DocumentStatus), DocumentStatus.Uploaded).Replace("_", " ");
                                reqDocType.UpdateDate = DateTime.Now;

                                var candidateDocument = _db.CandidateDocuments.FirstOrDefault(x => x.CandidateId == model.CandidateId);
                                if (candidateDocument == null)
                                {
                                    CandidateDocuments doc = new CandidateDocuments();
                                    doc.CandidateId = model.CandidateId;
                                    doc.CreateDate = DateTime.Now;
                                    doc.IsActive = true;
                                    doc.IsDelete = false;

                                    _db.CandidateDocuments.Add(doc);
                                    await _db.SaveChangesAsync();
                                }
                                var docs = _db.CandidateDocuments.FirstOrDefault(x => x.CandidateId == model.CandidateId);
                                /// For Degree
                                docs.Branch = String.IsNullOrEmpty(model.Branch) ? docs.Branch : model.Branch;
                                docs.Degreee = String.IsNullOrEmpty(model.Degreee) ? docs.Degreee : model.Degreee;
                                docs.YearOfJoining = model.YearOfJoining == null ? docs.YearOfJoining : model.YearOfJoining;
                                docs.YearOfCompleation = model.YearOfCompleation == null ? docs.YearOfCompleation : model.YearOfCompleation;
                                docs.PerctOrCGPA = String.IsNullOrEmpty(model.PerctOrCGPA) ? docs.PerctOrCGPA : model.PerctOrCGPA;
                                docs.UniversityOrCollage = String.IsNullOrEmpty(docs.UniversityOrCollage) ? docs.UniversityOrCollage : model.UniversityOrCollage;
                                docs.DegreeeUpload = String.IsNullOrEmpty(model.DegreeeUpload) ? docs.DegreeeUpload : model.DegreeeUpload;
                                /// For Pan Card
                                docs.PanNumber = String.IsNullOrEmpty(model.PanNumber) ? docs.PanNumber : model.PanNumber;
                                docs.NameOnPan = String.IsNullOrEmpty(model.NameOnPan) ? docs.NameOnPan : model.NameOnPan;
                                docs.DateOfBirthDateOnPan = model.DateOfBirthDateOnPan == null ? docs.DateOfBirthDateOnPan : model.DateOfBirthDateOnPan;
                                docs.FatherNameOnPan = String.IsNullOrEmpty(model.FatherNameOnPan) ? docs.FatherNameOnPan : model.FatherNameOnPan;
                                docs.PanUpload = String.IsNullOrEmpty(model.PanUpload) ? docs.PanUpload : model.PanUpload;
                                /// For Aadhaar
                                docs.AadhaarCardNumber = String.IsNullOrEmpty(model.AadhaarCardNumber) ? docs.AadhaarCardNumber : model.AadhaarCardNumber;
                                docs.DateOfBirthOnAadhaar = model.DateOfBirthOnAadhaar == null ? docs.DateOfBirthOnAadhaar : model.DateOfBirthOnAadhaar;
                                docs.FatherHusbandNameOnAadhaar = String.IsNullOrEmpty(model.FatherHusbandNameOnAadhaar) ? docs.FatherHusbandNameOnAadhaar : model.FatherHusbandNameOnAadhaar;
                                docs.GenderOnAadhaar = String.IsNullOrEmpty(model.GenderOnAadhaar) ? docs.GenderOnAadhaar : model.GenderOnAadhaar;
                                docs.AddressOnAadhaar = String.IsNullOrEmpty(model.AddressOnAadhaar) ? docs.AddressOnAadhaar : model.AddressOnAadhaar;
                                docs.FrontAadhaarUpload = String.IsNullOrEmpty(model.FrontAadhaarUpload) ? docs.FrontAadhaarUpload : model.FrontAadhaarUpload;
                                docs.BackAadhaarUpload = String.IsNullOrEmpty(model.BackAadhaarUpload) ? docs.BackAadhaarUpload : model.BackAadhaarUpload;
                                /// For Voter Id
                                docs.VoterIdNumber = String.IsNullOrEmpty(model.VoterIdNumber) ? docs.VoterIdNumber : model.VoterIdNumber;
                                docs.DateOfBirthOnVoterId = model.DateOfBirthOnVoterId == null ? docs.DateOfBirthOnVoterId : model.DateOfBirthOnVoterId;
                                docs.NameOnVoterId = String.IsNullOrEmpty(model.NameOnVoterId) ? docs.NameOnVoterId : model.NameOnVoterId;
                                docs.FatherHusbandNameOnVoter = String.IsNullOrEmpty(model.FatherHusbandNameOnVoter) ? docs.FatherHusbandNameOnVoter : model.FatherHusbandNameOnVoter;
                                docs.AddressOnVoterId = String.IsNullOrEmpty(model.AddressOnVoterId) ? docs.AddressOnVoterId : model.AddressOnVoterId;
                                /// For Driving License
                                docs.Licensenumber = String.IsNullOrEmpty(model.Licensenumber) ? docs.Licensenumber : model.Licensenumber;
                                docs.DateOfBirthOnDriving = model.DateOfBirthOnDriving == null ? docs.DateOfBirthOnDriving : model.DateOfBirthOnDriving;
                                docs.NameOnDriving = String.IsNullOrEmpty(model.NameOnDriving) ? docs.NameOnDriving : model.NameOnDriving;
                                docs.FatherHusbandNameOnDriving = String.IsNullOrEmpty(model.FatherHusbandNameOnDriving) ? docs.FatherHusbandNameOnDriving : model.FatherHusbandNameOnDriving;
                                docs.ExpireOnLicense = model.ExpireOnLicense == null ? docs.ExpireOnLicense : model.ExpireOnLicense;
                                docs.DrivingLicenseUpload = String.IsNullOrEmpty(model.DrivingLicenseUpload) ? docs.DrivingLicenseUpload : model.DrivingLicenseUpload;
                                /// For Passport
                                docs.PassportName = String.IsNullOrEmpty(model.PassportName) ? docs.PassportName : model.PassportName;
                                docs.DateOfBirthOnPassport = model.DateOfBirthOnPassport == null ? docs.DateOfBirthOnPassport : model.DateOfBirthOnPassport;
                                docs.FullNameOnPassport = String.IsNullOrEmpty(model.FullNameOnPassport) ? docs.FullNameOnPassport : model.FullNameOnPassport;
                                docs.FatherNameOnPassport = String.IsNullOrEmpty(model.FatherNameOnPassport) ? docs.FatherNameOnPassport : model.FatherNameOnPassport;
                                docs.DateOfIssue = model.DateOfIssue == null ? docs.DateOfIssue : model.DateOfIssue;
                                docs.PlaceOfIssue = String.IsNullOrEmpty(model.PlaceOfIssue) ? docs.PlaceOfIssue : model.PlaceOfIssue;
                                docs.PlaceOfBirth = String.IsNullOrEmpty(model.PlaceOfBirth) ? docs.PlaceOfBirth : model.PlaceOfBirth;
                                docs.ExpireOnOnPassport = model.ExpireOnOnPassport == null ? docs.ExpireOnOnPassport : model.ExpireOnOnPassport;
                                docs.AddressOnPassport = String.IsNullOrEmpty(model.AddressOnPassport) ? docs.AddressOnPassport : model.AddressOnPassport;
                                docs.PassportUpload = String.IsNullOrEmpty(model.PassportUpload) ? docs.PassportUpload : model.PassportUpload;
                                /// For Previous Experience
                                docs.CompanyName = String.IsNullOrEmpty(model.CompanyName) ? docs.CompanyName : model.CompanyName;
                                docs.JobTitle = String.IsNullOrEmpty(model.JobTitle) ? docs.JobTitle : model.JobTitle;
                                docs.JoiningDateExperience = model.JoiningDateExperience == null ? docs.JoiningDateExperience : model.JoiningDateExperience;
                                docs.RelievoingDateExperience = model.RelievoingDateExperience == null ? docs.RelievoingDateExperience : model.RelievoingDateExperience;
                                docs.LocationExperience = String.IsNullOrEmpty(model.LocationExperience) ? docs.LocationExperience : model.LocationExperience;
                                docs.DescriptionExperience = String.IsNullOrEmpty(model.DescriptionExperience) ? docs.DescriptionExperience : model.DescriptionExperience;
                                docs.ExperienceUpload = String.IsNullOrEmpty(model.ExperienceUpload) ? docs.ExperienceUpload : model.ExperienceUpload;
                                /// For Pay Slip
                                docs.PaySlipsUpload = String.IsNullOrEmpty(model.PaySlipsUpload) ? docs.PaySlipsUpload : model.PaySlipsUpload;

                                docs.UpdateDate = DateTime.Now;
                                reqDocType.CandidateDocId = docs.CandidateDocId;

                                _db.Entry(reqDocType).State = System.Data.Entity.EntityState.Modified;
                                _db.Entry(docs).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();

                                var reqDocsCount = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId && x.DocumentStatus == "Not Uploaded").ToList().Count;
                                if (reqDocsCount == 0)
                                {
                                    candidate.UpdatedOn = DateTime.Now;
                                    candidate.PrebordingStages = PreboardingStages.Verfiy_Info;
                                    _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                                    _db.SaveChanges();
                                }

                                res.Message = "Updated";
                                res.Status = true;
                                res.Data = docs;
                            }
                            else
                            {
                                res.Message = "Required Doc Type Not Found";
                                res.Status = false;
                            }
                        }
                        else
                        {
                            res.Message = "Doc Type Id Is 0";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Candidate Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Model Is Empty";
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

        #endregion Api To Add and Update Documents For Candidates

        #region Api To Get Added Documents For Candidates

        /// <summary>
        /// API >> Get >> api/docmaster/getaddeddoc
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getaddeddoc")]
        public async Task<ResponseBodyModel> GetAddedDocuments(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            NewGetAddRequiredDocumentModel obj = new NewGetAddRequiredDocumentModel();
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == candidateId);
                if (candidate != null)
                {
                    var reqDocList = await _db.RequiredDocs.Where(x => x.IsActive == true && x.IsDelete == false && x.CandidateId == candidate.CandidateId).ToListAsync();
                    if (reqDocList != null)
                    {
                        var doclist = Enum.GetValues(typeof(DocType));
                        if (doclist.Length > 0)
                        {
                            List<NewDocTypeHead> list1 = new List<NewDocTypeHead>();
                            foreach (var d in doclist)
                            {
                                int i = Convert.ToInt32(d);
                                List<NewReqDocList> list2 = new List<NewReqDocList>();

                                NewDocTypeHead head = new NewDocTypeHead();
                                head.Name = Enum.GetName(typeof(DocType), d).Replace('_', ' ');
                                foreach (var rlis in reqDocList)
                                {
                                    if (rlis.DocType == i)
                                    {
                                        NewReqDocList reqDocList1 = new NewReqDocList();
                                        reqDocList1.DocTypeId = rlis.DocTypeId;
                                        reqDocList1.DocTypeName = rlis.DocumentName;
                                        reqDocList1.DocStatus = rlis.DocumentStatus;
                                        //reqDocList1.Data = _db.CandidateDocuments.FirstOrDefault(y => y.CandidateId == candidate.CandidateId);

                                        list2.Add(reqDocList1);
                                    }
                                }
                                head.DocList = list2;
                                if (list2.Count > 0)
                                    list1.Add(head);
                            }
                            if (list1.Count > 0)
                            {
                                var candidatDoc = _db.CandidateDocuments.FirstOrDefault(y => y.CandidateId == candidate.CandidateId);
                                GetCandidateDocTypeModel data = new GetCandidateDocTypeModel()
                                {
                                    PreboardingStage = Enum.GetName(typeof(PreboardingStages), candidate.PrebordingStages).Replace("_", " "),
                                    Data1 = list1,
                                    Data2 = candidatDoc,
                                };

                                res.Message = "Candidated Added Doc";
                                res.Status = true;
                                res.Data = data;
                            }
                            else
                            {
                                res.Message = "Document Requirement For This Candidate is not Selected";
                                res.Status = false;
                            }
                        }
                        else
                        {
                            res.Message = "Document Requirement For This Candidate is not Selected";
                            res.Status = false;
                        }
                    }
                    else
                    {
                        res.Message = "Document Requirement For This Candidate is not Selected";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api To Get Added Documents For Candidates

        #region Api To Make Candidate Document Approve or Reject

        /// <summary>
        /// API >> Get >> api/docmaster/docapproveorreject
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("docapproveorreject")]
        public async Task<ResponseBodyModel> MakeDocumentApproveOrReject(ApproveRejectModelClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId);
                if (candidate != null)
                {
                    var reqDocType = _db.RequiredDocs.FirstOrDefault(x => x.CandidateId == candidate.CandidateId && x.DocTypeId == model.DocTypeId);
                    if (reqDocType != null)
                    {
                        reqDocType.DocumentStatus = model.IsApproved ?
                            Enum.GetName(typeof(DocumentStatus), DocumentStatus.Approved).Replace("_", " ") :
                            Enum.GetName(typeof(DocumentStatus), DocumentStatus.Rejected).Replace("_", " ");
                        reqDocType.UpdateDate = DateTime.Now;

                        _db.Entry(reqDocType).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        var totalDocs = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId).ToList().Count;
                        var rejectDocs = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId && x.DocumentStatus == "Rejected").ToList().Count;
                        var approvDocs = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId && x.DocumentStatus == "Approved").ToList().Count;
                        if (rejectDocs > 0)
                            candidate.PrebordingStages = PreboardingStages.Archived;
                        if (totalDocs == approvDocs)
                            candidate.PrebordingStages = PreboardingStages.Release_Offer;

                        _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        res.Message = "Document " + (model.IsApproved ?
                            Enum.GetName(typeof(DocumentStatus), DocumentStatus.Approved).Replace("_", " ") :
                            Enum.GetName(typeof(DocumentStatus), DocumentStatus.Rejected).Replace("_", " "));
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Required Document Not Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api To Make Candidate Document Approve or Reject

        #region Api To Change Required Document For Per Candidate

        /// <summary>
        /// API >> Get >> api/docmaster/changereqdoc
        /// Created By Harshit Mitra on 12-02-2022
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("changereqdoc")]
        public async Task<ResponseBodyModel> ChangeRequiredDocument(GetAddRequiredDocumentModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId);
                if (candidate != null)
                {
                    var reqDoc = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId).Select(x => x.DocTypeId).ToList();
                    if (reqDoc.Count > 0)
                    {
                        var requiredDocMaster = _db.RequiredDocMaster.FirstOrDefault(x => x.CandidateId == candidate.CandidateId);
                        if (requiredDocMaster != null)
                        {
                            foreach (var doc in model.DocHead)
                            {
                                foreach (var d in doc.DocList)
                                {
                                    if (reqDoc.Contains(d.DocTypeId))
                                    {
                                        var requiredDoc = _db.RequiredDocs.FirstOrDefault(x => x.DocTypeId == d.DocTypeId);
                                        if (!d.IsRequired)
                                        {
                                            _db.Entry(requiredDoc).State = EntityState.Deleted;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (d.IsRequired)
                                        {
                                            var docType = _db.DocumentTypes.FirstOrDefault(x => x.DocTypeId == d.DocTypeId);
                                            RequiredDocuments recdoc = new RequiredDocuments();
                                            recdoc.CandidateId = candidate.CandidateId;
                                            recdoc.DocTypeId = docType.DocTypeId;
                                            recdoc.ReqDocMasterId = requiredDocMaster.ReqDocMasterId;
                                            recdoc.DocumentName = docType.DocName;
                                            recdoc.DocumentStatus = Enum.GetName(typeof(DocumentStatus), DocumentStatus.Not_Uploaded).Replace('_', ' ');
                                            recdoc.DocType = docType.DocType;
                                            recdoc.IsActive = true;
                                            recdoc.IsDelete = false;
                                            recdoc.CreateDate = DateTime.Now;
                                            _db.RequiredDocs.Add(recdoc);
                                            await _db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            res.Message = "Updated";
                            res.Status = true;
                            res.Data = model;
                        }
                    }
                    else
                    {
                        res.Message = "Required Document For This Candidater is Not Added";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Canddidate Not Found";
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

        #endregion Api To Change Required Document For Per Candidate

        #region Api To Archived Candidate on Preboarding

        /// <summary>
        /// API >> Patch >> api/docmaster/archivecandionpreboard
        /// Created By Harshit Mitra on 12-02-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("archivecandionpreboard")]
        public async Task<ResponseBodyModel> ArchiveCandidateOnPreBoarding(Candidate model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId && x.CompanyId == claims.companyId);
                if (candidate != null)
                {
                    var job = await _db.JobPosts.FirstOrDefaultAsync(x => x.JobPostId == candidate.JobId);
                    if (job != null)
                    {
                        candidate.PreboardingArchiveStage = candidate.PreboardingArchiveStage;
                        candidate.HiringArchiveStage = candidate.StageId;
                        candidate.StageType = StageFlowType.Archived;
                        StageStatus obj = new StageStatus
                        {
                            CandidateId = model.CandidateId,
                            StageId = (Guid)candidate.StageId,
                            EmployeeId = claims.employeeId,
                            CompanyId = claims.companyId,
                            Reason = model.Reason,
                            PrebordingStageId = candidate.PrebordingStages,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            JobId = candidate.JobId,
                            StageOrder = _db.StageStatuses.Count(x => x.JobId == candidate.JobId && x.CandidateId == candidate.CandidateId),
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId
                        };
                        _db.StageStatuses.Add(obj);
                        await _db.SaveChangesAsync();
                        candidate.StageId = _db.HiringStages.Include("Job").Where(x => x.StageType == StageFlowType.Archived && x.Job.JobPostId == job.JobPostId).Select(x => x.StageId).FirstOrDefault();
                        candidate.PrebordingStages = PreboardingStages.Archived;
                        candidate.UpdatedOn = DateTime.Now;
                        candidate.UpdatedBy = claims.employeeId;
                        _db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        res.Message = "Candidate Archived";
                        res.Status = true;
                    }
                    else
                    {
                        res.Message = "Candidate Not Archived";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api To Archived Candidate on Preboarding




        #region Api To Get All Check Document Before Update Candidate Requirement Document

        /// <summary>
        /// API >> Patch >> api/docmaster/candreqdocchecklists
        /// Created By Harshit Mitra on 12-02-2022
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("candreqdocchecklists")]
        public async Task<ResponseBodyModel> GetCandidateRequirementCheckList(int candidateId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetAddRequiredDocumentModel obj = new GetAddRequiredDocumentModel();
            try
            {
                var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == candidateId);
                if (candidate != null)
                {
                    var docreqList = await _db.DocumentTypes.Where(x => x.IsDelete == false && x.IsActive == true).ToListAsync();
                    var reqDocType = _db.RequiredDocs.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                    var doclist = Enum.GetValues(typeof(DocType));

                    List<DocTypeHead> docTypeHeadList = new List<DocTypeHead>();
                    foreach (var d in doclist)
                    {
                        var i = Convert.ToInt32(d);
                        DocTypeHead docHead = new DocTypeHead();
                        docHead.Name = Enum.GetName(typeof(DocType), d).Replace('_', ' ');

                        List<ReqDocList> reqDocList = new List<ReqDocList>();
                        foreach (var doc in docreqList)
                        {
                            if (doc.DocType == i)
                            {
                                var docs = reqDocType.Where(x => x.DocTypeId == doc.DocTypeId).FirstOrDefault();
                                ReqDocList resList = new ReqDocList()
                                {
                                    DocTypeId = doc.DocTypeId,
                                    DocTypeName = doc.DocName,
                                    IsRequired = docs != null,
                                };
                                reqDocList.Add(resList);
                            }
                        }
                        docHead.DocList = reqDocList;
                        docTypeHeadList.Add(docHead);
                    }
                    obj.CandidateId = candidateId;
                    obj.DocHead = docTypeHeadList;

                    res.Message = "Recuired Document For Candidate";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Candidate Not Found";
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

        #endregion Api To Get All Check Document Before Update Candidate Requirement Document

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        public class GetAddRequiredDocumentModel
        {
            public int CandidateId { get; set; }
            public List<DocTypeHead> DocHead { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        public class DocTypeHead
        {
            public string Name { get; set; }
            public List<ReqDocList> DocList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 10-02-2022
        /// </summary>
        public class ReqDocList
        {
            public int DocTypeId { get; set; }
            public string DocTypeName { get; set; }
            public bool IsRequired { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        public class NewGetAddRequiredDocumentModel
        {
            public int CandidateId { get; set; }
            public List<NewDocTypeHead> DocHead { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        public class NewDocTypeHead
        {
            public string Name { get; set; }
            public List<NewReqDocList> DocList { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        public class NewReqDocList
        {
            public int DocTypeId { get; set; }
            public string DocTypeName { get; set; }
            public string DocStatus { get; set; }
            public dynamic Data { get; set; }
        }

        /// <summary>
        // Created By Harshit Mitra on 11-02-2022
        /// </summary>
        public class ApproveRejectModelClass
        {
            public int CandidateId { get; set; }
            public int DocTypeId { get; set; }
            public bool IsApproved { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 11-02-2022
        /// </summary>
        public class GetCandidateDocTypeModel
        {
            public string PreboardingStage { get; set; }
            public object Data1 { get; set; }
            public object Data2 { get; set; }
        }

        #endregion Helper Model Class
    }
}