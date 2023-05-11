using System;

namespace AngularJSAuthentication.API.Controllers
{
    //[RoutePrefix("api/WarehouseCategory")]
    //public class WarehoseCategoryController : ApiController
    //{
    //    //private static Logger logger = LogManager.GetCurrentClassLogger();
    //    [Authorize]
    //    [Route("")]
    //    public IEnumerable<WarehouseSubsubCategoryHindmt> Get()
    //    {
    //        // logger.Info("start Category: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            List<WarehouseSubsubCategoryHindmt> ass = new List<WarehouseSubsubCategoryHindmt>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid, Warehouse_id);
    //                if (Warehouse_id > 0)
    //                {
    //                    ass = context.AllWarehouseCategoryWid(compid, Warehouse_id, orgid).ToList();
    //                    // logger.Info("End  WarehouseCategory: ");
    //                    return ass;
    //                }
    //                else
    //                {
    //                    ass = context.AllWarehouseCategory(compid).ToList();
    //                    // logger.Info("End  WarehouseCategory: ");
    //                    return ass;
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                //  logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #region Get warehouse sub  Category
    //    [Route("WarehouseSubCategory")]
    //    public IEnumerable<WarehouseSubCategoryHindmt> Getsubcatdata(int WarehouseId)
    //    {
    //        //logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            List<WarehouseSubCategoryHindmt> subcat = new List<WarehouseSubCategoryHindmt>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid, Warehouse_id);
    //                var list = db.DbWarehouseSubCategory.Where(x => x.WarehouseId == WarehouseId).ToList();

    //                return list;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseSubCategory " + ex.Message);
    //                // logger.Info("End  WarehouseSubCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Get warehouse  Category
    //    [Route("WarehouseCategory")]
    //    public IEnumerable<WarehouseCategoryHindmt> Getalldata(int WarehouseId)
    //    {
    //        //  logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            List<WarehouseCategoryHindmt> ass = new List<WarehouseCategoryHindmt>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                //  logger.Info("User ID : {0} , Company Id : {1}", compid, userid, Warehouse_id);
    //                var list = db.DbWarehouseCategory.Where(x => x.WarehouseId == WarehouseId).ToList();

    //                return list;
    //            }
    //            catch (Exception ex)
    //            {
    //                //  logger.Error("Error in WarehouseCategory " + ex.Message);
    //                //  logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Get warehouse Base Category
    //    [Route("WHBaseCategory")]
    //    public IEnumerable<WarehouseBaseCategory> GetBasedata(int WarehouseId)
    //    {
    //        //logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            List<WarehouseBaseCategory> ass = new List<WarehouseBaseCategory>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid, Warehouse_id);
    //                var list = db.WarehouseBaseCategoryDB.Where(x => x.WarehouseId == WarehouseId).ToList();

    //                return list;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                // logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Get warehouse Sub Category
    //    [Route("sscategory")]
    //    [HttpGet]
    //    public IEnumerable<WarehouseSubsubCategoryHindmt> GetSubSubdata(int WarehouseId)
    //    {
    //        // logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            List<WarehouseSubsubCategoryHindmt> ass = new List<WarehouseSubsubCategoryHindmt>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                //  logger.Info("User ID : {0} , Company Id : {1}", compid, userid, Warehouse_id);
    //                var list = db.DbWarehousesubsubcats.Where(x => x.WarehouseId == WarehouseId).ToList();

    //                return list;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                // logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }

    //    #endregion

    //    #region Category Activated and Deactivated
    //    [Route("WHCatAct")]
    //    [AcceptVerbs("PUT")]
    //    public WarehouseCategoryHindmt ActPut(WarehouseCategoryHindmt item)
    //    {
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                WarehouseCategoryHindmt act = db.DbWarehouseCategory.Where(x => x.WhCategoryid == item.WhCategoryid && x.Deleted == false).FirstOrDefault();
    //                if (act != null)
    //                {
    //                    act.IsActive = item.IsActive;
    //                    db.Entry(act).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();

    //                }

    //                //If category is Active or Deactive so all the child relation is affected and active or deactive
    //                //Updated by Praveen Goswami on 21-Feb-2019

    //                List<WarehouseSubCategoryHindmt> listsub = new List<WarehouseSubCategoryHindmt>();
    //                listsub = db.DbWarehouseSubCategory.Where(x => x.Categoryid == item.WhCategoryid && x.Deleted == false).ToList();
    //                foreach (var x in listsub.ToList())
    //                {
    //                    x.IsActive = item.IsActive;
    //                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }

    //                List<WarehouseSubsubCategoryHindmt> listsubsub = new List<WarehouseSubsubCategoryHindmt>();
    //                listsubsub = db.DbWarehousesubsubcats.Where(x => x.Categoryid == item.WhCategoryid && x.Deleted == false).ToList();
    //                foreach (var y in listsubsub.ToList())
    //                {
    //                    y.IsActive = item.IsActive;
    //                    db.Entry(y).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //                return item;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Base Category Activated and Deactivated
    //    [Route("WHBaseCatAct")]
    //    [AcceptVerbs("PUT")]
    //    public WarehouseBaseCategory BaseActPut(WarehouseBaseCategory item)
    //    {
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                WarehouseBaseCategory act = db.WarehouseBaseCategoryDB.Where(x => x.id == item.id && x.Deleted == false).FirstOrDefault();
    //                if (act != null)
    //                {
    //                    act.IsActive = item.IsActive;
    //                    db.Entry(act).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();

    //                }

    //                //If base category is Active or Deactive so all the child relation is affected and active or deactive
    //                //Updated by Praveen Goswami on 21-Feb-2019

    //                List<WarehouseCategoryHindmt> listcat = new List<WarehouseCategoryHindmt>();
    //                listcat = db.DbWarehouseCategory.Where(x => x.BaseCategoryid == item.id && x.Deleted == false).ToList();
    //                foreach (var z in listcat.ToList())
    //                {
    //                    z.IsActive = item.IsActive;
    //                    db.Entry(z).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }

    //                List<WarehouseSubCategoryHindmt> listsub = new List<WarehouseSubCategoryHindmt>();
    //                listsub = db.DbWarehouseSubCategory.Where(x => x.BaseCategoryid == item.id && x.Deleted == false).ToList();
    //                foreach (var x in listsub.ToList())
    //                {
    //                    x.IsActive = item.IsActive;
    //                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }

    //                List<WarehouseSubsubCategoryHindmt> listsubsub = new List<WarehouseSubsubCategoryHindmt>();
    //                listsubsub = db.DbWarehousesubsubcats.Where(x => x.BaseCategoryId == item.id && x.Deleted == false).ToList();
    //                foreach (var y in listsubsub.ToList())
    //                {
    //                    y.IsActive = item.IsActive;
    //                    db.Entry(y).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //                return item;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Sub Category Activated and Deactivated
    //    [Route("WHSubCatAct")]
    //    [AcceptVerbs("PUT")]
    //    public WarehouseSubCategoryHindmt SubActPut(WarehouseSubCategoryHindmt item)
    //    {
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                //logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                WarehouseSubCategoryHindmt act = db.DbWarehouseSubCategory.Where(x => x.WhSubCategoryId == item.WhSubCategoryId && x.Deleted == false).FirstOrDefault();
    //                if (act != null)
    //                {
    //                    act.IsActive = item.IsActive;
    //                    db.Entry(act).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();

    //                }

    //                //If Subcategory is Active or Deactive so all the child relation is affected and active or deactive
    //                //Updated by Praveen Goswami on 21-Feb-2019

    //                List<WarehouseSubsubCategoryHindmt> listsubsub = new List<WarehouseSubsubCategoryHindmt>();
    //                listsubsub = db.DbWarehousesubsubcats.Where(x => x.SubCategoryId == item.WhSubCategoryId && x.Deleted == false).ToList();
    //                foreach (var y in listsubsub.ToList())
    //                {
    //                    y.IsActive = item.IsActive;
    //                    db.Entry(y).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();
    //                }
    //                return item;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Sub Sub Category Activated and Deactivated
    //    [Route("WHSubSubCatAct")]
    //    [AcceptVerbs("PUT")]
    //    public WarehouseSubsubCategoryHindmt SubsubActPut(WarehouseSubsubCategoryHindmt item)
    //    {
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                //   logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                WarehouseSubsubCategoryHindmt act = db.DbWarehousesubsubcats.Where(x => x.WhSubsubCategoryid == item.WhSubsubCategoryid && x.Deleted == false).FirstOrDefault();
    //                if (act != null)
    //                {
    //                    act.IsActive = item.IsActive;
    //                    db.Entry(act).State = System.Data.Entity.EntityState.Modified;
    //                    db.SaveChanges();

    //                }
    //                return item;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Mapp warehouse Base Category
    //    [Route("BaseCategory")]
    //    [HttpGet]
    //    public IEnumerable<WHBaseCategoryDTO> BasecategoryGet(int WarehouseId)
    //    {
    //        //logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                List<WHBaseCategoryDTO> NotMappeDataBase = new List<WHBaseCategoryDTO>();
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                var Cat = db.BaseCategoryDb.Where(x => x.Deleted == false).ToList();
    //                var Whdata = db.WarehousesHind.Where(x => x.WarehouseId == WarehouseId).SingleOrDefault();
    //                var MappedData = db.WarehouseBaseCategoryDB.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId).ToList();

    //                foreach (var Warehouse in Cat)
    //                {
    //                    if (MappedData.Any(x => x.BaseCategoryId == Warehouse.BaseCategoryId))
    //                    {
    //                    }
    //                    else
    //                    {
    //                        WHBaseCategoryDTO BData = new WHBaseCategoryDTO()
    //                        {
    //                            BaseCategoryId = Warehouse.BaseCategoryId,
    //                            BaseCategoryName = Warehouse.BaseCategoryName,
    //                            Code = Warehouse.Code,
    //                            LogoUrl = Warehouse.LogoUrl,
    //                            WarehouseId = Whdata.WarehouseId,
    //                            WarehouseName = Whdata.WarehouseName,
    //                            CreatedDate = Warehouse.CreatedDate,
    //                            UpdatedDate = Warehouse.UpdatedDate,
    //                            Deleted = Warehouse.Deleted,
    //                            CompanyId = compid,
    //                            CreatedBy = Warehouse.CreatedBy,
    //                            UpdateBy = Warehouse.UpdateBy,
    //                            IsActive = Warehouse.IsActive,
    //                            HindiName = Warehouse.HindiName

    //                        };
    //                        //MappeData.Add(Warehouse.Categoryid);

    //                        NotMappeDataBase.Add(BData);
    //                    }

    //                }

    //                //logger.Info("End  WarehouseCategory: ");

    //                return NotMappeDataBase;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                // logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region Mapp warehouse Category
    //    [Route("NotMapped")]
    //    [HttpGet]
    //    public IEnumerable<WarehouseCategoryDTO> NotMappedGet(int WarehouseId)
    //    {
    //        //  logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                List<WarehouseCategoryDTO> NotMappeData = new List<WarehouseCategoryDTO>();
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                var Cat = db.CategorysHindmt.Where(x => x.Deleted == false).ToList();
    //                var Whdata = db.WarehousesHind.Where(x => x.WarehouseId == WarehouseId).SingleOrDefault();
    //                var MappedData = db.DbWarehouseCategory.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId).ToList();

    //                List<WarehouseCategoryDTO> MappeData = new List<WarehouseCategoryDTO>();
    //                foreach (var Warehouse in Cat)
    //                {
    //                    if (MappedData.Any(x => x.Categoryid == Warehouse.Categoryid))
    //                    {
    //                    }
    //                    else
    //                    {
    //                        try
    //                        {
    //                            var MappedBasecatData = db.WarehouseBaseCategoryDB.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId && x.BaseCategoryId == Warehouse.BaseCategoryId).SingleOrDefault();
    //                            if (MappedBasecatData != null)
    //                            {
    //                                WarehouseCategoryDTO MData = new WarehouseCategoryDTO()
    //                                {
    //                                    Categoryid = Warehouse.Categoryid,
    //                                    CategoryName = Warehouse.CategoryName,
    //                                    BaseCategoryID = MappedBasecatData.id,
    //                                    Code = Warehouse.Code,
    //                                    LogoUrl = Warehouse.LogoUrl,
    //                                    CompanyId = compid,
    //                                    WarehouseId = Whdata.WarehouseId,
    //                                    WarehouseName = Whdata.WarehouseName,
    //                                    CreatedDate = Warehouse.CreatedDate,
    //                                    UpdatedDate = Warehouse.UpdatedDate,
    //                                    Deleted = Warehouse.Deleted,
    //                                    CreatedBy = Warehouse.CreatedBy,
    //                                    UpdateBy = Warehouse.UpdateBy,
    //                                    IsActive = Warehouse.IsActive

    //                                };
    //                                //MappeData.Add(Warehouse.Categoryid);

    //                                NotMappeData.Add(MData);
    //                            }
    //                        }
    //                        catch (Exception tt)
    //                        {
    //                            //  logger.Error("Error in WarehouseCategory " + tt.Message);
    //                        }
    //                    }

    //                }

    //                //logger.Info("End  WarehouseCategory: ");

    //                return NotMappeData;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                //  logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }

    //    #endregion
    //    #region Mapp warehouse Sub Category
    //    [Route("WhSubcategory")]
    //    [HttpGet]
    //    public IEnumerable<WarehouseSubCategoryDTO> NotMappedGetsubcat(int WarehouseId)
    //    {
    //        //logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                List<WarehouseSubCategoryDTO> NotMappeData = new List<WarehouseSubCategoryDTO>();
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                var Cat = db.SubCategorysHindmt.Where(x => x.Deleted == false).ToList();
    //                var Whdata = db.WarehousesHind.Where(x => x.WarehouseId == WarehouseId).SingleOrDefault();
    //                var MappedData = db.DbWarehouseSubCategory.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId).ToList();
    //                List<WarehouseSubCategoryDTO> MappeData = new List<WarehouseSubCategoryDTO>();
    //                foreach (var Warehouse in Cat)
    //                {
    //                    if (MappedData.Any(x => x.SubCategoryId == Warehouse.SubCategoryId))
    //                    {
    //                    }
    //                    else
    //                    {
    //                        var MappedBaseSubcatData = db.DbWarehouseCategory.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId && x.Categoryid == Warehouse.Categoryid).SingleOrDefault();
    //                        if (MappedBaseSubcatData != null)
    //                        {
    //                            WarehouseSubCategoryDTO MData = new WarehouseSubCategoryDTO()
    //                            {
    //                                BaseCategoryID = MappedBaseSubcatData.BaseCategoryid,
    //                                SubCategoryId = Warehouse.SubCategoryId,
    //                                SubcategoryName = Warehouse.SubcategoryName,
    //                                Categoryid = Warehouse.Categoryid,
    //                                CategoryName = Warehouse.CategoryName,
    //                                HindiName = Warehouse.HindiName,
    //                                SortOrder = Warehouse.SortOrder,
    //                                IsPramotional = Warehouse.IsPramotional,
    //                                Code = Warehouse.Code,
    //                                LogoUrl = Warehouse.LogoUrl,
    //                                CompanyId = compid,
    //                                WarehouseId = Whdata.WarehouseId,
    //                                WarehouseName = Whdata.WarehouseName,
    //                                CreatedDate = Warehouse.CreatedDate,
    //                                UpdatedDate = Warehouse.UpdatedDate,
    //                                Deleted = Warehouse.Deleted,
    //                                CreatedBy = Warehouse.CreatedBy,
    //                                UpdateBy = Warehouse.UpdateBy,
    //                                IsActive = Warehouse.IsActive

    //                            };

    //                            NotMappeData.Add(MData);
    //                        }
    //                    }

    //                }

    //                //logger.Info("End  WarehouseCategory: ");

    //                return NotMappeData;
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in WarehouseCategory " + ex.Message);
    //                //logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }

    //    #endregion
    //    #region Mapp warehouse Sub Sub Category
    //    [Route("WhSubSubcategory")]
    //    [HttpGet]
    //    public IEnumerable<WarehouseSubsubCategoryDTO> NotMappedGetsubsubcat(int WarehouseId)
    //    {
    //        //logger.Info("start Category: ");
    //        using (ApplicationDbContext db = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                List<WarehouseSubsubCategoryDTO> NotMappeData = new List<WarehouseSubsubCategoryDTO>();
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                var Cat = db.SubsubCategorys.Where(x => x.Deleted == false).ToList();
    //                var Whdata = db.WarehousesHind.Where(x => x.WarehouseId == WarehouseId).SingleOrDefault();
    //                var MappedData = db.DbWarehousesubsubcats.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId).ToList();
    //                List<WarehouseSubsubCategoryDTO> MappeData = new List<WarehouseSubsubCategoryDTO>();
    //                foreach (var Warehouse in Cat)
    //                {
    //                    if (MappedData.Any(x => x.SubsubCategoryid == Warehouse.SubsubCategoryid))
    //                    {
    //                    }
    //                    else
    //                    {
    //                        var MappedBaseSubSubcatData = db.DbWarehouseSubCategory.Where(x => x.Deleted == false && x.WarehouseId == WarehouseId && x.SubCategoryId == Warehouse.SubCategoryId).SingleOrDefault();
    //                        if (MappedBaseSubSubcatData != null)
    //                        {
    //                            WarehouseSubsubCategoryDTO MData = new WarehouseSubsubCategoryDTO()
    //                            {
    //                                BaseCategoryId = MappedBaseSubSubcatData.BaseCategoryid,
    //                                SubCategoryId = Warehouse.SubCategoryId,
    //                                SubcategoryName = Warehouse.SubcategoryName,
    //                                SubsubCategoryid = Warehouse.SubsubCategoryid,
    //                                SubsubcategoryName = Warehouse.SubsubcategoryName,
    //                                Categoryid = Warehouse.Categoryid,
    //                                //BaseCategoryId = Warehouse.BaseCategoryId,
    //                                CategoryName = Warehouse.CategoryName,
    //                                HindiName = Warehouse.HindiName,
    //                                SortOrder = Warehouse.SortOrder,
    //                                IsPramotional = Warehouse.IsPramotional,
    //                                Code = Warehouse.Code,
    //                                LogoUrl = Warehouse.LogoUrl,
    //                                CompanyId = compid,
    //                                WarehouseId = Whdata.WarehouseId,
    //                                WarehouseName = Whdata.WarehouseName,
    //                                CreatedDate = Warehouse.CreatedDate,
    //                                UpdatedDate = Warehouse.UpdatedDate,
    //                                Deleted = Warehouse.Deleted,
    //                                CreatedBy = Warehouse.CreatedBy,
    //                                UpdateBy = Warehouse.UpdateBy,
    //                                IsActive = Warehouse.IsActive,
    //                                Type = Warehouse.Type,
    //                                CommisionPercent = Warehouse.CommisionPercent,
    //                                IsExclusive = Warehouse.IsExclusive
    //                            };

    //                            NotMappeData.Add(MData);
    //                        }
    //                    }

    //                }

    //                //  logger.Info("End  WarehouseCategory: ");

    //                return NotMappeData;
    //            }
    //            catch (Exception ex)
    //            {
    //                //  logger.Error("Error in WarehouseCategory " + ex.Message);
    //                //  logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }

    //    #endregion

    //    [ResponseType(typeof(WarehouseCategoryHindmt))]
    //    [Route("")]
    //    public IEnumerable<WarehouseCategoryHindmt> GetAllCategory(string i)
    //    {
    //        // logger.Info("start Category: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            List<WarehouseCategoryHindmt> warehouseCategory = new List<WarehouseCategoryHindmt>();
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                warehouseCategory = context.AllWhCategory().ToList();
    //                // logger.Info("End  WarehouseCategory: ");
    //                return warehouseCategory;
    //            }
    //            catch (Exception ex)
    //            {
    //                //logger.Error("Error in WarehouseCategory " + ex.Message);
    //                // logger.Info("End  WarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    //[ResponseType(typeof(WarehouseCategoryHindmt))]
    //    //[Route("")]
    //    //[AcceptVerbs("POST")]
    //    //public List<WarehouseCategoryHindmt> add(List<WarehouseCategoryHindmt> item)
    //    //{
    //    //    // logger.Info("start addWarehouseCategory: ");
    //    //    using (ApplicationDbContext context = new ApplicationDbContext())
    //    //    {
    //    //        try
    //    //        {
    //    //            int Warehouse_id = 0;
    //    //            var identity = User.Identity as ClaimsIdentity;
    //    //            int userid = 0;
    //    //            int compid = 0;
    //    //            int orgid = 0;

    //    //            // Access claims

    //    //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //    //                userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //    //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //    //                compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //    //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //    //                orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //    //            context.AddToWarehousesCategorys(item);
    //    //            return item;

    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            //logger.Error("Error in addWarehouseCategory " + ex.Message);
    //    //            //logger.Info("End  addWarehouseCategory: ");
    //    //            return null;
    //    //        }
    //    //    }
    //    //}
    //    [ResponseType(typeof(WarehouseBaseCategory))]
    //    [Route("BaseCategory")]
    //    [AcceptVerbs("POST")]
    //    public List<WarehouseBaseCategory> add(List<WarehouseBaseCategory> item)
    //    {
    //        //logger.Info("start addWarehouseCategory: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                context.AddToWarehousesBaseCategorys(item);
    //                return item;

    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in addWarehouseCategory " + ex.Message);
    //                // logger.Info("End  addWarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #region mapped data save to Sub Category table
    //    [ResponseType(typeof(WarehouseSubCategoryHindmt))]
    //    [Route("SubCategory")]
    //    [AcceptVerbs("POST")]
    //    public List<WarehouseSubCategoryHindmt> add(List<WarehouseSubCategoryHindmt> item)
    //    {
    //        //  logger.Info("start addWarehouseCategory: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                context.AddToWarehousesSubCategorys(item);
    //                return item;

    //            }
    //            catch (Exception ex)
    //            {
    //                //  logger.Error("Error in addWarehouseCategory " + ex.Message);
    //                //  logger.Info("End  addWarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }

    //    #endregion
    //    #region mapped data save to Sub Sub Category table
    //    [ResponseType(typeof(WarehouseSubsubCategoryHindmt))]
    //    [Route("SubSubCategory")]
    //    [AcceptVerbs("POST")]
    //    public List<WarehouseSubsubCategoryHindmt> addsubsub(List<WarehouseSubsubCategoryHindmt> item)
    //    {
    //        // logger.Info("start addWarehouseCategory: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                context.AddToWarehousesSubSubCategorys(item);
    //                return item;

    //            }
    //            catch (Exception ex)
    //            {
    //                //logger.Error("Error in addWarehouseCategory " + ex.Message);
    //                // logger.Info("End  addWarehouseCategory: ");
    //                return null;
    //            }
    //        }
    //    }
    //    #endregion

    //    [ResponseType(typeof(WarehouseSubsubCategoryHindmt))]
    //    [Route("")]
    //    [AcceptVerbs("PUT")]
    //    public List<WarehouseSubsubCategoryHindmt> Put(List<WarehouseSubsubCategoryHindmt> item)
    //    {
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                return context.PutWarehouseCategory(item, compid, orgid);
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }

    //    [ResponseType(typeof(WarehouseSubsubCategoryHindmt))]
    //    [Route("")]
    //    [AcceptVerbs("Delete")]
    //    public void Remove(int id)
    //    {
    //        //logger.Info("start del WarehouseCategory: ");
    //        using (ApplicationDbContext context = new ApplicationDbContext())
    //        {
    //            try
    //            {
    //                int Warehouse_id = 0;
    //                var identity = User.Identity as ClaimsIdentity;
    //                int userid = 0;
    //                int compid = 0;
    //                int orgid = 0;

    //                // Access claims

    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
    //                    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
    //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
    //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
    //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

    //                // logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
    //                context.DeleteWarehouseCategory(id, compid, orgid);
    //                //   logger.Info("End  delete WarehouseCategory: ");
    //            }
    //            catch (Exception ex)
    //            {
    //                // logger.Error("Error in del WarehouseCategory " + ex.Message);

    //            }
    //        }
    //    }

    //}
    public class WarehouseCategoryDTO
    {
        public int BaseCategoryID { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Code { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public bool Deleted { get; set; }
    }

    public class WHBaseCategoryDTO
    {
        public int BaseCategoryId { get; set; }
        public string BaseCategoryName { get; set; }
        public string HindiName { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public string LogoUrl { get; set; }
        public bool Deleted { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
    }

    public class WarehouseSubCategoryDTO
    {
        public int BaseCategoryID { get; set; }
        public int SubCategoryId { get; set; }
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string HindiName { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPramotional { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }

    public class WarehouseSubsubCategoryDTO
    {
        public int SubsubCategoryid { get; set; }
        public string SubsubcategoryName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string HindiName { get; set; }
        public int BaseCategoryId { get; set; }
        public int Categoryid { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPramotional { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        public double? CommisionPercent { get; set; }
        public int CompanyId { get; set; }

        public bool? IsExclusive
        {
            get; set;
        }
    }
}