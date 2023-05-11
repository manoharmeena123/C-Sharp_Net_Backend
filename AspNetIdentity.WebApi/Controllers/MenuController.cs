using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/menu")]
    public class menuController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("GetMenuById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetMenuById(int Id)
        {
            try
            {
                //Base response = new Base();
                menuData depData = new menuData();
                var menuData = db.MenuItem.Where(x => x.MenuId == Id).FirstOrDefault();
                if (menuData != null)
                {
                    depData.Status = "OK";
                    depData.Message = "menu Found";
                    depData.menu = menuData;
                }
                else
                {
                    depData.Status = "Error";
                    depData.Message = "No menu Found!!";
                    depData.menu = null;
                }
                return Ok(depData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetAllMenu")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetAllmenu()
        {
            try
            {
                menuDataList dep = new menuDataList();
                var menuData = db.MenuItem.Where(x => x.Tittle.Trim() != "" && x.IsActive == true).ToList();
                if (menuData.Count != 0)
                {
                    dep.Status = "OK";
                    dep.Message = "menu list Found";
                    dep.menuList = menuData;
                }
                else
                {
                    dep.Status = "Not OK";
                    dep.Message = "No menu list Found";
                    dep.menuList = null;
                }
                return Ok(dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CreateMenu")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult Createmenu(MenuItem createmenu)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //Base response = new Base();
                var tmenuData = db.MenuItem.Where(x => x.Tittle.Trim().ToUpper() == createmenu.Tittle.Trim().ToUpper()).FirstOrDefault();
                menuData res = new menuData();
                MenuItem newmenu = new MenuItem();
                if (tmenuData == null)
                {
                    newmenu.Tittle = createmenu.Tittle;
                    newmenu.MenuIcon = createmenu.MenuIcon;
                    newmenu.CompanyId = claims.companyId;
                    newmenu.OrgId = claims.orgId;
                    newmenu.IsActive = true;
                    newmenu.IsDeleted = false;
                    db.MenuItem.Add(newmenu);
                    db.SaveChanges();

                    res.Status = "OK";
                    res.Message = "menu added Successfully!";
                    res.menu = newmenu;
                    return Ok(res);
                }
                else
                {
                    res.Status = "Error";
                    res.Message = "Menu Tittle already exists!";
                    res.menu = newmenu;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateMenu")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult Updatemenu(MenuItem updateDep)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Base response = new Base();
                var updateDepData = db.MenuItem.Where(x => x.MenuId == updateDep.MenuId).FirstOrDefault();
                if (updateDepData != null)
                {
                    updateDepData.Tittle = updateDep.Tittle;
                    updateDepData.MenuIcon = updateDep.MenuIcon;
                    updateDepData.CompanyId = claims.companyId;
                    updateDepData.OrgId = claims.orgId;
                    db.SaveChanges();

                    response.StatusReason = true;
                    response.Message = "Menu Updated Successfully!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteMenu")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteMenu(int menuId)
        {
            try
            {
                Base response = new Base();
                var deleteData = db.MenuItem.Where(x => x.MenuId == menuId).FirstOrDefault();
                if (deleteData != null)
                {
                    deleteData.IsActive = false;
                    deleteData.IsDeleted = true;

                    db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.StatusReason = true;
                    response.Message = "Menu Deleted Successfully!";
                }
                else
                {
                    response.StatusReason = false;
                    response.Message = "No menu Found!!";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class menuData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public MenuItem menu { get; set; }
        }

        public class menuDataList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<MenuItem> menuList { get; set; }
        }
    }
}