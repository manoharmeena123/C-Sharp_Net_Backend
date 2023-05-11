using AspNetIdentity.WebApi.Infrastructure;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/warehouse")]
    public class WarehouseController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        //#region Api To AddWarehouse

        ///// <summary>
        ///// APi >> Post >> api/extramaster/addlocation
        ///// Created By Ankit jain on 24-03-02022
        ///// </summary>
        ///// <returns></returns>
        ///// [Authorize]
        //[HttpPost]
        //[Route("addwarehouse")]
        //public async Task<ResponseBodyModel> adduserratingskill(Warehouse model)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        if (model != null)
        //        {
        //            var city = await db.City.FirstOrDefaultAsync(x => x.CityId == model.CityId);
        //            var state = await db.State.FirstOrDefaultAsync(x => x.StateId == model.StateId);
        //            if (city != null && state != null)
        //            {
        //                model.WarehouseName = model.WarehouseName;
        //                model.GSTinNumber = model.GSTinNumber;
        //                model.WarehouseAddress = model.WarehouseAddress;
        //                model.WarehouseEMailId = model.WarehouseEMailId;
        //                model.WarehousePhoneNumber = model.WarehousePhoneNumber;

        //                model.CityId = city.CityId;
        //                model.CityName = city.CityName;

        //                model.StateId = state.StateId;
        //                model.StateName = state.StateName;

        //                model.TaxGorupId = model.TaxGorupId;

        //                model.IsActive = true;
        //                model.IsDeleted = false;
        //                model.CreatedOn = DateTime.Now;
        //                model.CreatedBy = claims.userid;
        //                model.CompanyId = claims.companyid;
        //                model.OrgId = claims.orgid;

        //                db.Warehouses.Add(model);
        //                await db.SaveChangesAsync();

        //                res.Message = "Warehouse Added";
        //                res.Status = true;
        //                res.Data = model;
        //            }
        //            else
        //            {
        //                var condition = (city == null && state == null) ? "City and State "
        //                            : (city == null ? "City " : "State ");
        //                res.Message = condition + "Not Found";
        //                res.Status = false;
        //            }
        //        }
        //        else
        //        {
        //            res.Message = "Model Is Invalid";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api To AddWarehouse


        //#region getallWarehouse
        //[Authorize]
        //[HttpGet]
        //[Route("getallWarehouse")]
        //public async Task<ResponseBodyModel> GetAllWarehouse()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var Warehouse = await db.Warehouses.Select(x => new
        //        {
        //            x.WarehouseId,
        //            x.WarehouseName,
        //            x.GSTinNumber,
        //            x.WarehouseAddress,
        //            x.WarehouseEMailId,
        //            x.WarehousePhoneNumber,
        //            x.StateId,
        //            x.CityId,
        //            x.TaxGorupId,
        //            x.StateName,
        //            x.CityName,
        //            x.CreatedOn,
        //        }).ToListAsync();
        //        if (Warehouse.Count > 0)
        //        {
        //            res.Message = "Warehouse List";
        //            res.Status = true;
        //            res.Data = Warehouse;
        //        }
        //        else
        //        {
        //            res.Message = "List is Empty";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //        #region Api To warehouseupdate

        //        [Authorize]
        //        [HttpPut]
        //        [Route("updateWarehouse")]
        //        public async Task<ResponseBodyModel> Updatewarehouse(Warehouse model)
        //        {
        //            ResponseBodyModel res = new ResponseBodyModel();
        //            try
        //            {
        //                var identity = User.Identity as ClaimsIdentity;
        //                int userid = 0;
        //                int compid = 0;
        //                int orgid = 0;
        //                //Access claims
        //#pragma warning disable CS0642 // Possible mistaken empty statement
        //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid")) ;
        //#pragma warning restore CS0642 // Possible mistaken empty statement
        //                userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //                    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //                if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //                    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //                if (model == null)
        //                {
        //                    res.Message = "Model is Invalid";
        //                    res.Status = false;
        //                }
        //                var UpdateData = await db.Warehouses.FirstOrDefaultAsync(x => x.WarehouseId == model.WarehouseId);
        //                if (UpdateData != null)
        //                {
        //                    //var city = db.City.Where(x => x.Deleted == false && x.CityId==UpdateData.CityId).ToList();
        //                    var city = await db.City.FirstOrDefaultAsync(x => x.CityId == UpdateData.CityId);
        //                    var state = await db.State.FirstOrDefaultAsync(x => x.StateId == UpdateData.StateId);
        //                    UpdateData.WarehouseId = model.WarehouseId;
        //                    UpdateData.WarehouseName = model.WarehouseName;
        //                    UpdateData.GSTinNumber = model.GSTinNumber;
        //                    UpdateData.WarehouseAddress = model.WarehouseAddress;
        //                    UpdateData.WarehouseEMailId = model.WarehouseEMailId;
        //                    UpdateData.WarehousePhoneNumber = model.WarehousePhoneNumber;
        //                    UpdateData.StateId = model.StateId;
        //                    UpdateData.CityId = model.CityId;
        //                    UpdateData.StateName = state.StateName;
        //                    UpdateData.CityName = city.CityName;

        //                    UpdateData.TaxGorupId = model.TaxGorupId;
        //                    UpdateData.UpdatedBy = userid;
        //                    UpdateData.UpdatedOn = DateTime.Now;

        //                    db.Entry(UpdateData).State = System.Data.Entity.EntityState.Modified;
        //                    await db.SaveChangesAsync();

        //                    res.Message = "warehouse Updated successfully";
        //                    res.Status = true;
        //                    res.Data = UpdateData;
        //                }
        //                else
        //                {
        //                    res.Message = "warehouse Not Found";
        //                    res.Status = false;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                res.Message = ex.Message;
        //                res.Status = false;
        //            }
        //            return res;
        //        }

        //        #endregion Api To warehouseupdate

        //#region Api to Delete

        //[Route("DeleteWarehouse")]
        //[HttpDelete]
        //[Authorize]
        //public async Task<ResponseBodyModel> RemoveWarehose(int Id)

        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var WareDelete = await db.Warehouses.FirstOrDefaultAsync(x => x.WarehouseId == Id && x.IsDeleted == false);
        //        if (WareDelete != null)
        //        {
        //            // WareDelete.DeletedBy = claims.employeeid;
        //            WareDelete.DeletedOn = DateTime.Now;
        //            WareDelete.IsDeleted = true;
        //            WareDelete.IsActive = false;
        //            db.Entry(WareDelete).State = System.Data.Entity.EntityState.Modified;
        //            await db.SaveChangesAsync();

        //            res.Message = "Warehouse Deleted successfully ";
        //            res.Status = true;
        //            res.Data = WareDelete;
        //        }
        //        else
        //        {
        //            res.Message = "warehouse Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api to Delete






    }
}