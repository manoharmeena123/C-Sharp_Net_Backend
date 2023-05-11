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

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ItemMasterWarehouse")]
    public class ItemMasterWarehouseController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        #region Api to Add Warehouse

        /// <summary>
        /// Api for Edit Warehouse
        /// Created by Nayan Pancholi
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddItemMasterWarehouse")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> AddItemMasterWarehouse(ItemMasterWarehouse item)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (item != null)
                {
                    item.CreatedOn = indianTime;
                    item.UpdatedOn = indianTime;
                    item.IsActive = true;
                    item.IsDeleted = false;
                    item.CompanyId = claims.companyId;
                    item.OrgId = claims.orgId;

                    db.ItemMasterWarehouses.Add(item);
                    int id = db.SaveChanges();

                    res.Message = "Warehouse created successfully";
                    res.Status = true;
                    res.Data = item;
                }
                else
                {
                    res.Message = "Warehouse Not Added";
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

        #endregion Api to Add Warehouse

        #region Api for Get Warehouse

        /// <summary>
        /// Api to Get Warehouse
        /// Created by Nayan Pancholi
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetItemMasterWarehouse")]
        public async Task<ResponseBodyModel> GetItemMasterWarehouse()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            // List<GetItemMaster> list = new List<GetItemMaster>();
            try
            {
                var ItemMasterData = await db.ItemMasterWarehouses.Where(x => x.CompanyId == claims.companyId
                      && x.OrgId == claims.orgId && x.IsDeleted == false && x.IsActive == true)
                      .Select(x => new
                      {
                          WarehouseId = x.WarehouseId,
                          WarehouseName = x.WarehouseName,
                          WarehouseDescription = x.WarehouseDescription,
                          WarehouseAddress = x.WarehouseAddress,
                          TotalItems = db.ItemMaster.Where(s => s.IsDeleted == false && s.IsActive == true && s.WarehouseId == x.WarehouseId).ToList().Count,
                      }).ToListAsync();

                if (ItemMasterData != null)
                {
                    res.Message = "ItemMasterDataWarehouse Found";
                    res.Status = true;
                    res.Data = ItemMasterData;
                }
                else
                {
                    res.Message = "ItemMasterDataWarehouse Not Found";
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

        #endregion Api for Get Warehouse

        #region Api for Edit Warehouse

        /// <summary>
        /// Api for Edit Warehouse
        /// Created by Nayan Pancholi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPut]
        [Route("EditItemMasterWarehouse")]
        public async Task<ResponseBodyModel> EditItemMasterWarehouse(ItemMasterWarehouse model)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var editwarehouse = await db.ItemMasterWarehouses.FirstOrDefaultAsync(x => x.WarehouseId == model.WarehouseId);
                if (editwarehouse != null)
                {
                    editwarehouse.WarehouseId = model.WarehouseId;
                    editwarehouse.WarehouseName = model.WarehouseName;
                    editwarehouse.WarehouseDescription = model.WarehouseDescription;
                    editwarehouse.WarehouseAddress = model.WarehouseAddress;

                    editwarehouse.UpdatedOn = DateTime.Now;
                    db.Entry(editwarehouse).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    res.Message = "Warehouse Updated";
                    res.Status = true;
                    res.Data = editwarehouse;
                }
                else
                {
                    res.Message = "Warehouse Not Found";
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

        #endregion Api for Edit Warehouse

        #region Api for Delete Warehouse

        /// <summary>
        /// API to Delete Warehouse
        /// Created by Nayan Pancholi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteItemMasterWarehouse")]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<ResponseBodyModel> DeleteItemMasterWarehouse(int id)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                ItemMasterWarehouse category = db.ItemMasterWarehouses.Where(x => x.WarehouseId == id && x.IsDeleted == false).FirstOrDefault();
                if (category != null)
                {
                    category.IsDeleted = true;
                    category.IsActive = false;
                    category.UpdatedOn = indianTime;
                    db.ItemMasterWarehouses.Attach(category);
                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    res.Message = "Warehouse Deleted successfully";
                    res.Status = true;
                    res.Data = category;
                }
                else
                {
                    res.Message = "Warehouse Not Found";
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

        #endregion Api for Delete Warehouse

        #region Helper Model Class

        public class GetItemMaster
        {
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public string WarehouseAddress { get; set; }
            public string WarehouseDescription { get; set; }
            public int? TotalItems { get; set; }
        }

        #endregion Helper Model Class
    }
}