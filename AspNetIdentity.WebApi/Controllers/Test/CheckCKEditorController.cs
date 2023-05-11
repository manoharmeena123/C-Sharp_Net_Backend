using AspNetIdentity.WebApi.Infrastructure;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Test
{
    [RoutePrefix("api/testckeditor")]
    public class CheckCKEditorController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create(TestCKEditor model)
        {
            try
            {
                _db.testCKEditors.Add(model);
                await _db.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IHttpActionResult> GetAll()
        {
            var data = _db.testCKEditors.ToList();
            return Ok(data);
        }

        [HttpGet]
        [Route("getbyid")]
        public async Task<IHttpActionResult> GetAll(Guid id)
        {
            var data = await _db.testCKEditors.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(data);
        }
    }
}
