//using AngularJSAuthentication.Model;
//using AspNetIdentity.WebApi.Infrastructure;
//using System.Data;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Web.Http;
//using System.Web.Http.Description;

//namespace AngularJSAuthentication.API.Controllers
//{
//    public class WarehouseCatApiController : ApiController
//    {
//        // GET: api/WarehouseCatApi
//        public IQueryable<WarehouseCategoryHindmt> GetDbWarehouseCategory()
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                var items = from i in db.DbWarehouseCategory.Include("ItemMaster") select i;
//                return items;
//            }
//            //return db.DbWarehouseCategory;
//        }

//        // GET: api/WarehouseCatApi/5
//        [ResponseType(typeof(WarehouseCategoryHindmt))]
//        public IHttpActionResult GetWarehouseCategory(int id)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                WarehouseCategoryHindmt warehouseCategory = db.DbWarehouseCategory.Find(id);
//                if (warehouseCategory == null)
//                {
//                    return NotFound();
//                }

//                return Ok(warehouseCategory);
//            }
//        }

//        // PUT: api/WarehouseCatApi/5
//        [ResponseType(typeof(void))]
//        public IHttpActionResult PutWarehouseCategory(int id, WarehouseCategoryHindmt warehouseCategory)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                if (id != warehouseCategory.WhCategoryid)
//                {
//                    return BadRequest();
//                }

//                db.Entry(warehouseCategory).State = System.Data.Entity.EntityState.Modified;

//                try
//                {
//                    db.SaveChanges();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!WarehouseCategoryExists(id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }

//                return StatusCode(HttpStatusCode.NoContent);
//            }
//        }

//        // POST: api/WarehouseCatApi
//        [ResponseType(typeof(WarehouseCategoryHindmt))]
//        public IHttpActionResult PostWarehouseCategory(WarehouseCategoryHindmt warehouseCategory)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                db.DbWarehouseCategory.Add(warehouseCategory);
//                db.SaveChanges();

//                return CreatedAtRoute("DefaultApi", new { id = warehouseCategory.WhCategoryid }, warehouseCategory);
//            }
//        }

//        // DELETE: api/WarehouseCatApi/5
//        [ResponseType(typeof(WarehouseCategoryHindmt))]
//        public IHttpActionResult DeleteWarehouseCategory(int id)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                WarehouseCategoryHindmt warehouseCategory = db.DbWarehouseCategory.Find(id);
//                if (warehouseCategory == null)
//                {
//                    return NotFound();
//                }

//                db.DbWarehouseCategory.Remove(warehouseCategory);
//                db.SaveChanges();

//                return Ok(warehouseCategory);
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                if (disposing)
//                {
//                    db.Dispose();
//                }
//                base.Dispose(disposing);
//            }
//        }

//        private bool WarehouseCategoryExists(int id)
//        {
//            using (ApplicationDbContext db = new ApplicationDbContext())
//            {
//                return db.DbWarehouseCategory.Count(e => e.WhCategoryid == id) > 0;
//            }
//        }
//    }
//}