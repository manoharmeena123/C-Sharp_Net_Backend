using AspNetIdentity.WebApi.Infrastructure;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        // private static //logger //logger = LogManager.GetCurrentClass//logger();
        private ApplicationDbContext db = new ApplicationDbContext();

        //        [Authorize]
        //        [Route("WarehouseBased")]
        //        [HttpGet]
        //        public IEnumerable<WarehouseSubsubCategoryHindmt> Get(string recordtype, int whid)
        //        {
        //            //  using (var context = new AuthContext())
        //            {
        //                if (recordtype == "warehouse")
        //                {
        //                    // //logger.Info("start Category: ");
        //                    List<SubsubCategoryHindmt> sCategory = new List<SubsubCategoryHindmt>();
        //                    List<CategoryHindmt> Category = new List<CategoryHindmt>();
        //                    List<WarehouseHindmt> Warehouse = new List<WarehouseHindmt>();
        //                    List<WarehouseCategoryHindmt> WarehouseCategory = new List<WarehouseCategoryHindmt>();
        //                    List<WarehouseSubsubCategoryHindmt> sWarehouseCategory = new List<WarehouseSubsubCategoryHindmt>();
        //                    List<WarehouseSubsubCategoryHindmt> wareH = new List<WarehouseSubsubCategoryHindmt>();
        //                    List<string> Subcode = new List<string>();
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //                    try
        //                    {
        //                        var identity = User.Identity as ClaimsIdentity;
        //#pragma warning disable CS0219 // The variable 'userid' is assigned but its value is never used
        //                        int compid = 0, userid = 0;
        //#pragma warning restore CS0219 // The variable 'userid' is assigned but its value is never used
        //#pragma warning disable CS0219 // The variable 'Warehouse_id' is assigned but its value is never used
        //                        int Warehouse_id = 0;
        //#pragma warning restore CS0219 // The variable 'Warehouse_id' is assigned but its value is never used
        //                        // Access claims
        //                        //foreach (Claim claim in identity.Claims)
        //                        //{
        //                        //    if (claim.Type == "compid")
        //                        //    {
        //                        //        compid = int.Parse(claim.Value);
        //                        //    }
        //                        //    if (claim.Type == "userid")
        //                        //    {
        //                        //        userid = int.Parse(claim.Value);
        //                        //    }
        //                        //    if (claim.Type == "Warehouseid")
        //                        //    {
        //                        //        Warehouse_id = int.Parse(claim.Value);
        //                        //    }
        //                        // }

        //                        //  //logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
        //                        Category = db.AllCategory(compid).ToList();
        //                        sCategory = db.sAllCategory(compid).ToList();
        //                        Warehouse = db.AllWarehouse(compid).ToList();

        //                        wareH = db.AllWarehouseCategory(compid).ToList();

        //                        var cat = Category;
        //                        var scat = sCategory;
        //                        var war = (from c in Warehouse where c.WarehouseId.Equals(whid) && c.CompanyId == compid select c).SingleOrDefault();

        //                        for (int i = 0; i < scat.Count; i++)
        //                        {
        //                            List<WarehouseSubsubCategoryHindmt> wcat = (from c in wareH where c.WarehouseId == whid && c.Deleted == false && c.CompanyId == compid select c).ToList();

        //                            WarehouseSubsubCategoryHindmt wc = new WarehouseSubsubCategoryHindmt();
        //                            wc.SubsubCategoryid = scat[i].SubsubCategoryid;
        //                            wc.SubsubcategoryName = scat[i].SubcategoryName;
        //                            wc.SubsubCode = scat[i].Code;
        //                            wc.IsActive = true;
        //                            wc.LogoUrl = scat[i].LogoUrl;
        //                            wc.WarehouseId = whid;

        //                            foreach (var c in wcat)
        //                            {
        //                                if (c.SubsubCategoryid.Equals(scat[i].SubsubCategoryid))
        //                                {
        //                                    wc.SubsubCategoryid = c.SubsubCategoryid;
        //                                    wc.IsActive = true;
        //                                    wc.SortOrder = c.SortOrder;
        //                                }
        //                            }

        //                            if (wc != null && !Subcode.Any(x => x == wc.SubsubCode))
        //                            {
        //                                sWarehouseCategory.Add(wc);
        //                                Subcode.Add(wc.SubsubCode);
        //                            }
        //                        }
        //                        ////logger.Info("End  Category: ");
        //                        return sWarehouseCategory;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // //logger.Error("Error in Category " + ex.Message);
        //                        // //logger.Info("End  Category: ");
        //                        return null;
        //                    }
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //                }
        //            }
        //            return null;
        //        }

        //        [Authorize]
        //        [Route("getCategory")]
        //        [HttpGet]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> GetCategory(int baseCategoryId)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            // //logger.Info("Get Category");
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            CategoryHelperHindmt ch = new CategoryHelperHindmt();
        //            try
        //            {
        //                var pr = ch.AllCategory(db, baseCategoryId);
        //                if (pr != null)
        //                {
        //                    res.Message = "Category found successfully";
        //                    res.Status = true;
        //                    res.Data = pr;
        //                }
        //                else
        //                {
        //                    res.Message = "Category Not Found";
        //                    res.Status = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                //  //logger.Info("Error in getting category details" + e.Message);
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        [Authorize]
        //        [Route("getItem")]
        //        [HttpGet]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> GetItems(int CategoryId)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            // //logger.Info("Get Category");
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            CategoryHelperHindmt ch = new CategoryHelperHindmt();
        //            try
        //            {
        //                var item = ch.GetItem(db, CategoryId);

        //                if (item != null)
        //                {
        //                    res.Message = "Category successfully";
        //                    res.Status = true;
        //                    res.Data = item;
        //                }
        //                else
        //                {
        //                    res.Message = "Category Not Found";
        //                    res.Status = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                //  //logger.Info("Error in getting category details" + e.Message);
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        [Authorize]
        //        [Route("WarehouseCategorybased")]
        //        [HttpGet]
        //        public IEnumerable<WarehouseCategoryHindmt> Get(string recordtype, int whid, int whcatid)
        //        {
        //            // using (var context = new AuthContext())
        //            {
        //                if (recordtype == "warehouse")
        //                {
        //                    ////logger.Info("start Category: ");
        //                    List<CategoryHindmt> Category = new List<CategoryHindmt>();
        //                    List<WarehouseHindmt> Warehouse = new List<WarehouseHindmt>();
        //                    List<WarehouseCategoryHindmt> WarehouseCategory = new List<WarehouseCategoryHindmt>();
        //                    List<WarehouseCategoryHindmt> wareH = new List<WarehouseCategoryHindmt>();
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //                    try
        //                    {
        //                        var identity = User.Identity as ClaimsIdentity;
        //#pragma warning disable CS0219 // The variable 'userid' is assigned but its value is never used
        //                        int compid = 0, userid = 0;
        //#pragma warning restore CS0219 // The variable 'userid' is assigned but its value is never used
        //                        // Access claims
        //                        //foreach (Claim claim in identity.Claims)
        //                        //{
        //                        //    if (claim.Type == "compid")
        //                        //    {
        //                        //        compid = int.Parse(claim.Value);
        //                        //    }
        //                        //    if (claim.Type == "userid")
        //                        //    {
        //                        //        userid = int.Parse(claim.Value);
        //                        //    }
        //                        //}

        //                        //  //logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
        //                        Category = db.AllCategory(compid).ToList();
        //                        Warehouse = db.AllWarehouse(compid).ToList();

        //                        var cat = Category;
        //                        var war = (from c in Warehouse where c.WarehouseId.Equals(whid) && c.CompanyId == compid select c).SingleOrDefault();

        //                        List<WarehouseCategoryHindmt> wcat = (from c in wareH where c.WarehouseId == whid && c.CompanyId == compid select c).ToList();
        //                        foreach (var i in wcat)
        //                        {
        //                            if (i.IsActive)
        //                            {
        //                                WarehouseCategoryHindmt wc = new WarehouseCategoryHindmt();
        //                                wc.WhCategoryid = whcatid;
        //                                wc.Categoryid = i.Categoryid;
        //                                wc.CategoryName = i.CategoryName;
        //                                wc.WarehouseId = whid;
        //                                //wc.Stateid = war.Stateid;
        //                                //wc.State = war.StateName;
        //                                //wc.Cityid = war.Cityid;
        //                                //wc.City = war.CityName;
        //                                wc.IsActive = true;
        //                                wc.SortOrder = i.SortOrder;

        //                                WarehouseCategory.Add(wc);
        //                            }
        //                        }
        //                        // //logger.Info("End  Category: ");
        //                        return WarehouseCategory;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        //   //logger.Error("Error in Category " + ex.Message);
        //                        // //logger.Info("End  Category: ");
        //                        return null;
        //                    }
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //                }
        //            }
        //            return null;
        //        }

        //        #region get Category code

        //        public string convertnumber2String(int number)
        //        {
        //            string text = "";
        //            switch (number)
        //            {
        //                case 1:
        //                    {
        //                        text = "A";
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        text = "B";
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        text = "C";
        //                        break;
        //                    }
        //                case 4:
        //                    {
        //                        text = "D";
        //                        break;
        //                    }
        //                case 5:
        //                    {
        //                        text = "E";
        //                        break;
        //                    }
        //                case 6:
        //                    {
        //                        text = "F";
        //                        break;
        //                    }
        //                case 7:
        //                    {
        //                        text = "G";
        //                        break;
        //                    }
        //                case 8:
        //                    {
        //                        text = "H";
        //                        break;
        //                    }
        //                case 9:
        //                    {
        //                        text = "I";
        //                        break;
        //                    }
        //                case 10:
        //                    {
        //                        text = "J";
        //                        break;
        //                    }
        //                case 11:
        //                    {
        //                        text = "K";
        //                        break;
        //                    }
        //                case 12:
        //                    {
        //                        text = "L";
        //                        break;
        //                    }
        //                case 13:
        //                    {
        //                        text = "M";
        //                        break;
        //                    }
        //                case 14:
        //                    {
        //                        text = "N";
        //                        break;
        //                    }
        //                case 15:
        //                    {
        //                        text = "O";
        //                        break;
        //                    }
        //                case 16:
        //                    {
        //                        text = "P";
        //                        break;
        //                    }
        //                case 17:
        //                    {
        //                        text = "Q";
        //                        break;
        //                    }
        //                case 18:
        //                    {
        //                        text = "R";
        //                        break;
        //                    }
        //                case 19:
        //                    {
        //                        text = "S";
        //                        break;
        //                    }
        //                case 20:
        //                    {
        //                        text = "T";
        //                        break;
        //                    }
        //                case 21:
        //                    {
        //                        text = "U";
        //                        break;
        //                    }
        //                case 22:
        //                    {
        //                        text = "V";
        //                        break;
        //                    }
        //                case 23:
        //                    {
        //                        text = "W";
        //                        break;
        //                    }
        //                case 24:
        //                    {
        //                        text = "X";
        //                        break;
        //                    }
        //                case 25:
        //                    {
        //                        text = "Y";
        //                        break;
        //                    }
        //                case 26:
        //                    {
        //                        text = "Z";
        //                        break;
        //                    }
        //                default:
        //                    {
        //                        break;
        //                    }
        //            }
        //            return text;
        //        }

        //        public string GetCategoryCode()
        //        {
        //            int cat1 = int.Parse(WebConfigurationManager.AppSettings["cat1"]);
        //            int cat2 = int.Parse(WebConfigurationManager.AppSettings["cat2"]);

        //            if (cat2 > 26)
        //            {
        //                cat2 = 1;
        //                cat1 += 1;
        //            }
        //            else
        //            {
        //                cat2 += 1;
        //            }
        //            string newcat1 = convertnumber2String(cat1);
        //            string newcat2 = convertnumber2String(cat2);

        //            //Update Configure Cat1 and Cat2 value

        //            ////Helps to open the Root level web.config file.
        //            //Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

        //            ////Modifying the AppKey from AppValue to AppValue1

        //            //webConfigApp.AppSettings.Settings["cat1"].Value = Convert.ToString(cat1);
        //            //webConfigApp.AppSettings.Settings["cat2"].Value = Convert.ToString(cat2);
        //            ////Save the Modified settings of AppSettings.
        //            //webConfigApp.Save();

        //            var data = newcat1.Trim() + newcat2.Trim();
        //            data = data.Replace("\"", string.Empty).Trim();
        //            return data;
        //        }

        //        #endregion get Category code

        //        [ResponseType(typeof(CategoryHindmt))]
        //        [Authorize]
        //        [Route("AddCategory")]
        //        [HttpPost]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> add(CategoryHindmt item)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            //  using (var context = new AuthContext())
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            // //logger.Info("start addCategory: ");
        //            try
        //            {
        //                //int cat1 = int.Parse(WebConfigurationManager.AppSettings["cat1"]);
        //                //int cat2 = int.Parse(WebConfigurationManager.AppSettings["cat2"]);
        //                //Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
        //                //var identity = User.Identity as ClaimsIdentity;
        //#pragma warning disable CS0219 // The variable 'userid' is assigned but its value is never used
        //#pragma warning disable CS0219 // The variable 'compid' is assigned but its value is never used
        //                int compid = 0, userid = 0;
        //#pragma warning restore CS0219 // The variable 'compid' is assigned but its value is never used
        //#pragma warning restore CS0219 // The variable 'userid' is assigned but its value is never used
        //                // Access claims
        //                //foreach (Claim claim in identity.Claims)
        //                //{
        //                //    if (claim.Type == "compid")
        //                //    {
        //                //        compid = int.Parse(claim.Value);
        //                //    }
        //                //    if (claim.Type == "userid")
        //                //    {
        //                //        userid = int.Parse(claim.Value);
        //                //    }
        //                //}
        //                if (item != null)
        //                {
        //                    // throw new ArgumentNullException("item");

        //                    // //logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
        //                    db.AddCategory(item);
        //                    ////if (cat2 > 26)
        //                    //{
        //                    //    cat2 = 1;
        //                    //    cat1 += 1;
        //                    //}
        //                    //else
        //                    //{
        //                    //    cat2 += 1;
        //                    //}
        //                    // webConfigApp.AppSettings.Settings["cat1"].Value = Convert.ToString(cat1);
        //                    //webConfigApp.AppSettings.Settings["cat2"].Value = Convert.ToString(cat2);
        //                    //Save the Modified settings of AppSettings.
        //                    //webConfigApp.Save();
        //                    // //logger.Info("End  addCategory: ");
        //                    res.Message = "SubCategory added successfully";
        //                    res.Status = true;
        //                    res.Data = item;
        //                }
        //                else
        //                {
        //                    res.Message = "Category Not Found";
        //                    res.Status = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Message = ex.Message;
        //                res.Status = false;
        //                ////logger.Error("Error in addCategory " + ex.Message);
        //                ////logger.Info("End  addCategory: ");
        //            }
        //            return res;
        //        }

        //        [ResponseType(typeof(CategoryHindmt))]
        //        [Authorize]
        //        [Route("PUT")]
        //        [HttpPut]
        //#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        public async Task<ResponseBodyModel> categoryHindmt(CategoryHindmt objcat)
        //#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();

        //            try
        //            {
        //                var category = db.CategorysHindmt.Where(x => x.Categoryid == objcat.Categoryid && x.Deleted == false).FirstOrDefault();

        //                if (category != null)
        //                {
        //                    category.UpdatedDate = DateTime.Now;
        //                    category.CategoryName = objcat.CategoryName;
        //                    category.HindiName = objcat.HindiName;
        //                    category.Discription = objcat.Discription;
        //                    string logourl = objcat.LogoUrl.Trim('"');
        //                    category.LogoUrl = logourl;
        //                    category.Code = objcat.Code;
        //                    category.LogoUrl = objcat.LogoUrl;
        //                    category.ConsumerApp_LogoUrl = objcat.ConsumerApp_LogoUrl;
        //                    category.IsActive = objcat.IsActive;
        //                    category.Deleted = objcat.Deleted;
        //                    //Categorys.Attach(category);
        //                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();

        //                    res.Message = "Category Update successfully";
        //                    res.Status = true;
        //                    res.Data = category;
        //                }
        //                else
        //                {
        //                    res.Message = "Category Not Found";
        //                    res.Status = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // //logger.Error("Error in addCategory " + ex.Message);
        //                //  //logger.Info("End  addCategory: ");
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        //[ResponseType(typeof(CategoryHindmt))]
        //        //[Authorize]
        //        //[Route("DeleteCategory")]
        //        //[AcceptVerbs("Delete")]
        //        //public async Task<ResponseBodyModel> Remove(int id)
        //        //{
        //        //    ResponseBodyModel res = new ResponseBodyModel();
        //        //    try
        //        //    {
        //        //        var identity = User.Identity as ClaimsIdentity;
        //        //        int compid = 0, userid = 0;

        //        //        CategoryHindmt category = db.CategorysHindmt.Where(x => x.Categoryid == id && x.Deleted == false).FirstOrDefault();
        //        //        if (category != null)
        //        //        {
        //        //            category.Deleted = true;
        //        //            category.IsActive = false;
        //        //            db.CategorysHindmt.Attach(category);

        //        //            db.Entry(category).State = System.Data.Entity.EntityState.Modified;
        //        //            db.SaveChanges();

        //        //            CommonHelper.refreshCategory();
        //        //            res.Status = true;
        //        //            res.Message = "Category Deleted Successfully!";

        //        //            //  //logger.Info("End  delete Category: ");
        //        //        }
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        //   //logger.Error("Error in del Category " + ex.Message);
        //        //    }
        //        //    return res;
        //        //}
        //    }

        //    //#region Api For Delete Supplier
        //    ///// <summary>
        //    ///// API >> Delete >> api/category/deletcategory
        //    ///// Created By Nayan Pancholi on 22-03-2022
        //    ///// </summary>
        //    ///// <param name="catId"></param>
        //    ///// <returns></returns>
        //    //[HttpDelete]
        //    //[Route("deletcategory")]
        //    //public async Task<ResponseBodyModel> DeletCategory(int catId)
        //    //{
        //    //    ResponseBodyModel res = new ResponseBodyModel();
        //    //    try
        //    //    {
        //    //        using (var _db = new ApplicationDbContext())
        //    //        {
        //    //            var CatDelete = await _db.CategorysHindmt.Where(x => x.Deleted == false && x.Categoryid == catId).FirstOrDefaultAsync();
        //    //            if (CatDelete != null)
        //    //            {
        //    //                CatDelete.Deleted = true;
        //    //                CatDelete.IsActive = false;
        //    //                _db.Entry(CatDelete).State = System.Data.Entity.EntityState.Modified;

        //    //                _db.SaveChanges();

        //    //                CommonHelper.refreshCategory();
        //    //                res.Status = true;
        //    //                res.Message = "Record Delete Successfully";
        //    //                res.Data = CatDelete;
        //    //            }
        //    //            else
        //    //            {
        //    //                res.Status = false;
        //    //                res.Message = "Please select the record";
        //    //                res.Data = CatDelete;
        //    //            }
        //    //        }

        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        res.Message = ex.Message;
        //    //        res.Status = false;
        //    //        ////logger.Error("Error in addCategory " + ex.Message);
        //    //        ////logger.Info("End  addCategory: ");
        //    //    }
        //    //    return res;
        //    //}
        //    //#endregion

        //    //#region Get all active category

        //    //[Authorize]
        //    //[Route("activeCat")]
        //    //[HttpGet]
        //    //public async Task<ResponseBodyModel> GetactiveCat()
        //    //{
        //    //    ResponseBodyModel res = new ResponseBodyModel();
        //    //    try
        //    //    {
        //    //        var identity = User.Identity as ClaimsIdentity;
        //    //        int compid = 0, userid = 0;

        //    //        var ass = db.CategorysHindmt.Where(x => x.Deleted == false).ToList();
        //    //        //ass = context.Categorys.Where(x => x.Deleted == false && x.IsActive == true).ToList();
        //    //        //  //logger.Info("End  Category: ");
        //    //        if (ass != null)
        //    //        {
        //    //            res.Status = true;
        //    //            res.Message = "activeCat Successfully!";

        //    //            //  //logger.Info("End  delete Category: ");
        //    //        }
        //    //        else
        //    //        {
        //    //            res.Message = "activeCat Not Found";
        //    //            res.Status = false;
        //    //        }

        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        res.Message = ex.Message;
        //    //        res.Status = false;
        //    //        ////logger.Error("Error in addCategory " + ex.Message);
        //    //        ////logger.Info("End  addCategory: ");
        //    //    }
        //    //    return res;
        //    //}

        //    //#endregion

        //    //#region get Category by base category Id
        //    //[Authorize]
        //    //[Route("GetCategoryByBasecateId")]
        //    //[HttpGet]
        //    //public async Task<CataegoryResDto> GetCategoryByBasecateId(int basecategoryId)
        //    //{
        //    //    CataegoryResDto resDto = new CataegoryResDto();
        //    //    try
        //    //    {
        //    //        //using (var unitwork = new UnitOfWork())
        //    //        //{
        //    //        // var identity = User.Identity as ClaimsIdentity;
        //    //        int userid = 0;

        //    //        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //    //        //    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);

        //    //        var data = db.CategorysHindmt.Where(x => x.BaseCategoryId == basecategoryId && x.IsActive == true && x.Deleted == false).ToList();
        //    //        if (data.Count > 0)
        //    //        {
        //    //            resDto.Message = "Success";
        //    //            resDto.Flag = true;
        //    //            resDto.List = data;
        //    //        }
        //    //        else
        //    //        {
        //    //            resDto.Message = "Data does not exist";
        //    //            resDto.Flag = false;
        //    //            resDto.List = null;
        //    //        }
        //    //        //}
        //    //        return resDto;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        resDto.Message = "Error";
        //    //        resDto.Flag = false;
        //    //        resDto.List = null;

        //    //        return resDto;
        //    //    }
        //    //}

        //    //#endregion

        //    //#region Api for get Category whose items are added
        //    //[Authorize]
        //    //[HttpGet]
        //    //[Route("getCategoryWithItem")]
        //    //public async Task<categoryvenDTO> getCategoryWithItem(int vendorId)
        //    //{
        //    //    categoryvenDTO onjRes = new categoryvenDTO();
        //    //    try
        //    //    {
        //    //        using (var unit = new UnitOfWork())
        //    //        {
        //    //            var data = await unit.CategoryRepository.getCategoryWithItem(vendorId);
        //    //            if (data.Count > 0)
        //    //            {
        //    //                onjRes.Message = "Successful";
        //    //                onjRes.Flag = true;
        //    //                onjRes.List = data;
        //    //            }
        //    //            else
        //    //            {
        //    //                onjRes.Message = "data don't exist";
        //    //                onjRes.Flag = false;
        //    //                onjRes.List = null;
        //    //            }
        //    //        }
        //    //        return onjRes;
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        onjRes.Message = "Successful";
        //    //        onjRes.Flag = false;
        //    //        onjRes.List = null;
        //    //        return onjRes;
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Api for  Category by  Type
        //    //[Authorize]
        //    //[Route("getCategoryByType")]
        //    //[HttpGet]
        //    //public async Task<CataegoryResDto> getCategoryByType(string Type)
        //    //{
        //    //    CataegoryResDto objRes = new CataegoryResDto();
        //    //    try
        //    //    {
        //    //        using (var unit = new UnitOfWork())
        //    //        {
        //    //            var data = await unit.CategoryRepository.getCategoryByType(Type);
        //    //            if (data.Count > 0)
        //    //            {
        //    //                objRes.List = data;
        //    //                objRes.Message = "Successful";
        //    //                objRes.Flag = true;
        //    //            }
        //    //            else
        //    //            {
        //    //                objRes.List = null;
        //    //                objRes.Message = "item doesn't exist";
        //    //                objRes.Flag = false;
        //    //            }
        //    //        }
        //    //        return objRes;
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        objRes.List = null;
        //    //        objRes.Message = e.Message;
        //    //        objRes.Flag = false;
        //    //        return objRes;
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Api for get Category by base categoryId whose items are added
        //    //[Authorize]
        //    //[HttpGet]
        //    //[Route("getCatbyBasecatItem")]
        //    //public async Task<customeritemsNew> getCategorybyBasecatItem(int baseCategoryId)
        //    //{
        //    //    customeritemsNew objRes = new customeritemsNew();
        //    //    List<Categoriess> categories = new List<Categoriess>();
        //    //    try
        //    //    {
        //    //        using (var unit = new UnitOfWork())
        //    //        {
        //    //            // using (var db = new AuthContext())
        //    //            {
        //    //                objRes.Basecats = await unit.baseCategoryRepository.getBasecategoryWithId(baseCategoryId);

        //    //                categories = await unit.CategoryRepository.getCategorybyBasecatWithItem(baseCategoryId);
        //    //                if (categories.Count > 0)
        //    //                {
        //    //                    foreach (var basecat in objRes.Basecats)
        //    //                    {
        //    //                        basecat.Categories = categories;
        //    //                    }

        //    //                }
        //    //                else
        //    //                {
        //    //                    foreach (var basecat in objRes.Basecats)
        //    //                    {
        //    //                        basecat.Categories = null;
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //        return objRes;
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        objRes.Basecats = null;
        //    //        return objRes;
        //    //    }
        //    //}
        //    //#endregion

        //    //*********************************** Start Here Bind Categorys Create By Mohit ****************

        //    //[Route("BindCategorys")]
        //    //public IEnumerable<CategoryHindmt> Get()
        //    //{
        //    //    //  using (var context = new AuthContext())
        //    //    {
        //    //        // //logger.Info("start Category: ");
        //    //        List<CategoryHindmt> ass = new List<CategoryHindmt>();
        //    //        try
        //    //        {
        //    //            var identity = User.Identity as ClaimsIdentity;
        //    //            int compid = 0, userid = 0;
        //    //            // Access claims
        //    //            //foreach (Claim claim in identity.Claims)
        //    //            //{
        //    //            //    if (claim.Type == "compid")
        //    //            //    {
        //    //            //        compid = int.Parse(claim.Value);
        //    //            //    }
        //    //            //    if (claim.Type == "userid")
        //    //            //    {
        //    //            //        userid = int.Parse(claim.Value);
        //    //            //    }
        //    //            //}
        //    //            ////logger.Info("User ID : {0} , Company Id : {1}", compid, userid);
        //    //            ass = db.CategorysHindmt.Where(x => x.Deleted == false).ToList();
        //    //            // //logger.Info("End  Category: ");
        //    //            return ass;
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            // //logger.Error("Error in Categorys " + ex.Message);
        //    //            // //logger.Info("End  Category: ");
        //    //            return null;
        //    //        }
        //    //    }
    }
    //*********************************** Start Here Bind Categorys Create By Mohit ****************
}